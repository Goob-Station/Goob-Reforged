// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using Content.Goobstation.Shared.Supermatter.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Chat.Systems;
using Content.Shared.Atmos;
using Content.Shared.Chat;
using Robust.Shared.Maths;
using System.Diagnostics.CodeAnalysis;

namespace Content.Goobstation.Server.Supermatter;

/// <summary>
/// <para>Simple <see cref="IDisposable"/> wrapper around <see cref="GasMixture"/></para>
/// <para>Splits off a part of gas, and merges them together later</para>
/// <para><see cref="AtmosphereSystem.Merge(GasMixture, GasMixture)"/> is automatically called at the end -
/// use <see cref="GasMixture"/> instead if you want to handle it manually</para>
/// </summary>
/// <param name="mix">Mix to use.</param>
/// <param name="ratio">How much do you want split off.</param>
/// <param name="atmosphereContext">Atmosphere system to use for re-merging</param>
internal readonly struct GasWrapper(GasMixture mix, float ratio, AtmosphereSystem atmosphereContext) : IDisposable, IEquatable<GasWrapper>
{
    private readonly GasMixture _surrounding = mix;

    /// <summary>
    /// The split off part of your gas.
    /// </summary>
    internal readonly GasMixture Gas = mix.RemoveRatio(ratio);

    public void Dispose()
    {
        atmosphereContext.Merge(_surrounding, Gas);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is GasWrapper wrapper && Equals(wrapper);
    }

    public bool Equals(GasWrapper other)
    {
        return EqualityComparer<GasMixture>.Default.Equals(_surrounding, other._surrounding) &&
               EqualityComparer<GasMixture>.Default.Equals(Gas, other.Gas);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_surrounding, Gas);
    }
}

internal static class SupermatterExtensions
{
    /// <summary>Get SM related data about a provided gas mix.</summary>
    /// <returns>A selection of values, check <see cref="SupermatterComponent.GasDataFields(Gas)"/></returns>
    internal static (float exergy, float heatModifier, float wasteModifier, float heatResistModifier) GetGasModifiers(this GasMixture gasMix)
    {
        var totalMoles = gasMix.TotalMoles;

        // Safety check: Prevent a divide-by-zero NaN cascade if the mix is completely empty
        if (totalMoles <= 0f)
        {
            return (1f, 1f, 1f, 1f);
        }

        var exergyModifier = 1f;
        var heatModifier = 1f;
        var wasteModifier = 1f;
        var heatResistModifier = 1f;

        // Safely iterate through the actual enum values, regardless of their integer backing
        foreach (var gas in Enum.GetValues<Gas>())
        {
            var proportion = gasMix.GetGasMolarPercentage(gas);

            // Skip doing math if there's none of this gas in the mix
            if (proportion <= 0f)
            {
                continue;
            }

            var (exergyMod, heatMod, wasteMod, heatResistMod) = SupermatterComponent.GasDataFields(gas);

            exergyModifier += proportion * exergyMod;
            heatModifier += proportion * heatMod;
            wasteModifier += proportion * wasteMod;
            heatResistModifier += proportion * heatResistMod;
        }

        // Ensure we don't do something stupid later
        return (
            Math.Max(exergyModifier, 0f),
            Math.Max(heatModifier, 0f),
            Math.Max(wasteModifier, 0.5f), // At least *some* gases to always have that risk.
            Math.Max(heatResistModifier, 0.5f) // Even with some crazy gases you are safe if cold.
        );
    }

    internal static float GetGasMolarPercentage(this GasMixture gasMix, Gas gas)
    {
        return gasMix.TotalMoles <= 0f ? 0f : gasMix.GetMoles(gas) / gasMix.TotalMoles;
    }

    internal static float GetGasMolarPercentage(this GasMixture gasMix, int gas)
    {
        return gasMix.TotalMoles <= 0f ? 0f : gasMix.GetMoles(gas) / gasMix.TotalMoles;
    }

    /// <summary>
    ///     Opinionated "Try" wrapper around <see cref="AtmosphereSystem.GetContainingMixture(Entity{TransformComponent?}, bool, bool)"/>,
    ///     with different defaults and some implicit behavior to cover edge cases
    /// </summary>
    /// <param name="mix">A <see cref="GasMixture"/> if one could be found, null otherwise.</param>
    /// <param name="ent">The entity to get the mixture for.</param>
    /// <param name="ignoreExposed">If true, will ignore mixtures that the entity is contained in
    /// (ex. lockers and cryopods) and just get the tile mixture. True by default!</param>
    /// <param name="excite">If true, will mark the tile as active for atmosphereContext processing. True by default!</param>
    /// <returns>True when a mix has been found, false otherwise</returns>
    /// <remarks>Non-obvious behavior - it'll also return false when mix is <= 0 moles</remarks>
    internal static bool TryGetContainingMixture(
        this AtmosphereSystem atmosContext,
        [NotNullWhen(true)] out GasMixture? mix,
        in EntityUid ent,
        bool ignoreExposed = true,
        bool excite = true)
    {
        mix = atmosContext.GetContainingMixture(ent, ignoreExposed, excite);
        return mix?.TotalMoles > 0f;
    }

    /// <summary>Help the SM announce something.</summary>
    /// <param name="uid">Supermatter to say the announcement from.</param>
    /// <param name="message">Message to be sent</param>
    /// <param name="global">If true, does the station announcement.</param>
    /// <param name="customSender">Sender for when global is true.</param>
    internal static void DispatchSupermatterAnnouncement(
        this ChatSystem chatContext,
        in EntityUid uid,
        string message,
        bool global = false,
        string? customSender = null)
    {
        if (global)
        {
                var sender = customSender ?? Loc.GetString("supermatter-announcer");
                chatContext.DispatchStationAnnouncement(uid, message, sender, colorOverride: Color.Yellow);
                return;
        }
        chatContext.TrySendInGameICMessage(uid, message, InGameICChatType.Speak, hideChat: false);
    }
}
