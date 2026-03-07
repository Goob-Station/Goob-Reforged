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
internal readonly struct GasWrapper(GasMixture mix, float ratio, AtmosphereSystem atmosphereContext) : IDisposable
{
    private readonly GasMixture _surrounding = mix;

    /// <summary>
    /// The split off part of your gas.
    /// </summary>
    public readonly GasMixture Gas = mix.RemoveRatio(ratio);

    public void Dispose()
    {
        atmosphereContext.Merge(_surrounding, Gas);
    }
}

internal static class SupermatterExtensions
{
    extension(GasMixture gasMix)
    {
        /// <summary>
        /// Get SM related data about a provided gas mix.
        /// </summary>
        /// <returns>A selection of values, check <see cref="SupermatterComponent.GasDataFields(Gas)"/></returns>
        public (float radModifier, float zapModifier, float moleModifier, float heatModifier, float heatResistModifier) GetGasModifiers()
        {
            var totalMoles = gasMix.TotalMoles;

            // Safety check: Prevent a divide-by-zero NaN cascade if the mix is completely empty
            if (totalMoles <= 0f)
            {
                return (1f, 1f, 1f, 1f, 1f);
            }

            var radModifier = 1f;
            var zapModifier = 1f;
            var moleModifier = 1f;
            var heatModifier = 1f;
            var heatResistModifier = 1f;

            // Safely iterate through the actual enum values, regardless of their integer backing
            foreach (var gas in Enum.GetValues<Gas>())
            {
                var proportion = gasMix.GetGasMolarPercentage(gas);

                // Skip doing math if there's none of this gas in the mix
                if (proportion <= 0f)
                    continue;

                var (radMod, zapMod, heatMod, moleMod, heatResistMod) = SupermatterComponent.GasDataFields(gas);

                radModifier += proportion * radMod;
                zapModifier += proportion * zapMod;
                moleModifier += proportion * moleMod;
                heatModifier += proportion * heatMod;
                heatResistModifier += proportion * heatResistMod;
            }

            // Ensure we don't do something stupid later
            return (
                Math.Max(radModifier, 0f),
                Math.Max(zapModifier, 0f),
                Math.Max(moleModifier, 0f),
                Math.Max(heatModifier, 0f),
                Math.Max(heatResistModifier, 0f)
            );
        }

        public float GetGasMolarPercentage(Gas gas)
        {
            if (gasMix.TotalMoles <= 0f)
                return 0f;
            return gasMix.GetMoles(gas) / gasMix.TotalMoles;
        }

        public float GetGasMolarPercentage(int gas)
        {
            if (gasMix.TotalMoles <= 0f)
                return 0f;
            return gasMix.GetMoles(gas) / gasMix.TotalMoles;
        }
    }

    extension(AtmosphereSystem atmosContext)
    {
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
        public bool TryGetContainingMixture([NotNullWhen(true)] out GasMixture? mix, EntityUid ent, bool ignoreExposed = true, bool excite = true)
        {
            mix = atmosContext.GetContainingMixture(ent, ignoreExposed, excite);

            if (mix is not { })
                return false;

            if (mix.TotalMoles <= 0f)
                return false;
            return false;
        }
    }

    extension(ChatSystem chatContext)
    {
        /// <summary>
        ///     Help the SM announce something.
        /// </summary>
        /// <param name="uid">Supermatter to say the announcement from.</param>
        /// <param name="message">Message to be sent</param>
        /// <param name="global">If true, does the station announcement.</param>
        /// <param name="customSender">Sender for when global is true.</param>
        public void DispatchSupermatterAnnouncement(EntityUid uid, string message, bool global = false, string? customSender = null)
        {
            if (global)
            {
                var sender = customSender ?? Loc.GetString("supermatter-announcer");
                chatContext.DispatchStationAnnouncement(uid, message, sender, colorOverride: Color.Yellow);
                return;
            }
            chatContext.TrySendInGameICMessage(uid, message, InGameICChatType.Speak, hideChat: false, checkRadioPrefix: true);
        }
    }
}
