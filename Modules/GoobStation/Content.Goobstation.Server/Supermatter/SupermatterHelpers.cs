using Content.Goobstation.Shared.Supermatter.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Chat.Systems;
using Content.Shared.Atmos;
using Content.Shared.Chat;
using Robust.Shared.Maths;
namespace Content.Goobstation.Server.Supermatter;

/// <summary>
/// <para>Simple <see cref="IDisposable"/> wrapper around <see cref="GasMixture"/></para>
/// <para>Splits off a part of gas, and merges them together later</para>
/// <para><see cref="AtmosphereSystem.Merge(GasMixture, GasMixture)"/> is automatically called at the end -
/// use <see cref="GasMixture"/> instead if you want to handle it manually</para>
/// </summary>
readonly struct GasWrapper(GasMixture surroundingMix, float ratio, AtmosphereSystem atmosphere) : IDisposable
{
    private readonly GasMixture _surrounding = surroundingMix;

    /// <summary>
    /// The split off part of your gas. 
    /// </summary>
    public readonly GasMixture Gas = surroundingMix.RemoveRatio(ratio);

    public void Dispose()
    {
        atmosphere.Merge(_surrounding, Gas);
    }
}

public static class SmExtensions
{
    /// <summary>
    /// Get SM related data about a provided gas mix.
    /// </summary>
    /// <param name="absorbedGas">Mix to be parsed</param>
    /// <returns>A selection of values, check <see cref="SupermatterComponent.GasDataFields(Gas)"/></returns>
    public static (float radModifier, float zapModifier, float moleModifier, float heatModifier, float heatResistModifier) GetGasModifiers(this GasMixture absorbedGas)
    {
        var totalMoles = absorbedGas.TotalMoles;

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
        foreach (Gas gas in Enum.GetValues<Gas>())
        {
            var proportion = absorbedGas.GetGasMolarPercentage(gas);

            // Skip doing math if there's none of this gas in the mix
            if (proportion <= 0f) continue;

            var facts = SupermatterComponent.GasDataFields(gas);

            radModifier += proportion * facts.RadMod;
            zapModifier += proportion * facts.ZapMod;
            moleModifier += proportion * facts.MoleMod;
            heatModifier += proportion * facts.HeatMod;
            heatResistModifier += proportion * facts.HeatResistMod;
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

    public static float GetGasMolarPercentage(this GasMixture gasMix, Gas gas)
    {
        if (!(gasMix.TotalMoles > 0f))
            return 0f;
        return gasMix.GetMoles(gas) / gasMix.TotalMoles;
    }

    public static float GetGasMolarPercentage(this GasMixture gasMix, int gas)
    {
        if (!(gasMix.TotalMoles > 0f))
            return 0f;
        return gasMix.GetMoles(gas) / gasMix.TotalMoles;
    }

    /// <summary>
    ///     Help the SM announce something.
    /// </summary>
    /// <param name="global">If true, does the station announcement.</param>
    /// <param name="customSender">If true, sends the announcement from Central Command.</param>
    public static void SupermatterAnnouncement(this ChatSystem chat, EntityUid uid, string message, bool global = false, string? customSender = null)
    {
        if (global)
        {
            var sender = customSender ?? Loc.GetString("supermatter-announcer");
            chat.DispatchStationAnnouncement(uid, message, sender, colorOverride: Color.Yellow);
            return;
        }
        chat.TrySendInGameICMessage(uid, message, InGameICChatType.Speak, hideChat: false, checkRadioPrefix: true);
    }
}


[ByRefEvent]
public record struct SupermatterTickEvent(SupermatterComponent Component);

[ByRefEvent]
public record struct SupermatterYapEvent(SupermatterComponent Component);

[ByRefEvent]
public record struct SupermatterZapEvent(SupermatterComponent Component);

[ByRefEvent]
public record struct SupermatterDelamEvent(SupermatterComponent Component);
