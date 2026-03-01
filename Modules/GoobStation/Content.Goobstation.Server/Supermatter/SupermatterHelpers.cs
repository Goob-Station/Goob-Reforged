// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using Content.Goobstation.Shared.Supermatter.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;
using static Content.Goobstation.Shared.Supermatter.Systems.SharedSupermatterSystem;
namespace Content.Goobstation.Server.Supermatter;

/// <summary>
/// <para>Simple <see cref="IDisposable"/> wrapper around <see cref="GasMixture"/></para>
/// <para>Splits off a part of gas, and merges them together later</para>
/// <para><see cref="AtmosphereSystem.Merge(GasMixture, GasMixture)"/> is automatically called at the end -
/// use <see cref="GasMixture"/> instead if you want to handle it manually</para>
/// </summary>
internal readonly struct GasWrapper(GasMixture surroundingMix, float ratio, AtmosphereSystem atmosphere) : IDisposable
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

internal static partial class SupermatterExtensions
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
            foreach (Gas gas in Enum.GetValues<Gas>())
            {
                var proportion = gasMix.GetGasMolarPercentage(gas);

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

        public float GetGasMolarPercentage(Gas gas)
        {
            if (!(gasMix.TotalMoles > 0f))
                return 0f;
            return gasMix.GetMoles(gas) / gasMix.TotalMoles;
        }

        public float GetGasMolarPercentage(int gas)
        {
            if (!(gasMix.TotalMoles > 0f))
                return 0f;
            return gasMix.GetMoles(gas) / gasMix.TotalMoles;
        }
    }

    extension(Entity<SupermatterComponent> ent)
    {
        /// <summary>
        ///     Decide on how to delaminate.
        /// </summary>
        public DelamType ChooseDelamType(AtmosphereSystem atmosphereContext)
        {
            var mix = atmosphereContext.GetContainingMixture(ent.Owner, true, true);

            if (mix is { })
            {
                var moles = mix.TotalMoles;

                if (moles >= ent.Comp.MolePenaltyThreshold)
                    return DelamType.Singulo;
            }

            if (ent.Comp.Power >= ent.Comp.PowerPenaltyThreshold)
                return DelamType.Tesla;

            return DelamType.Explosion;
        }
    }
}
