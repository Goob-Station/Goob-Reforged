// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using Content.Goobstation.Shared.Supermatter.Components;
using Content.Shared.Atmos;
using Content.Shared.Radiation.Components;

namespace Content.Goobstation.Server.Supermatter.Systems;

public sealed partial class SupermatterSystem
{
    /// <summary>Handle power and radiation output depending on atmospheric things.</summary>
    /// <param name="ent">Entity to process atmos for.</param>
    private void ProcessAtmos(Entity<SupermatterComponent> ent)
    {
        var sm = ent.Comp;

        #region Get gas mix and modifiers

        if (!_atmosphere.TryGetContainingMixture(out var mix, ent))
        {
            return;
        }

        using var absorbed = new GasWrapper(mix, sm.GasEfficiency, _atmosphere);

        var (exergyModifier, heatModifier, wasteModifier, _) = absorbed.Gas.GetGasModifiers();

        var co2Modifier = GetCo2Modifier(sm, absorbed);

        #endregion Get gas mix and modifiers

        #region Add power to crystal

        ConsumeMatterPower(sm);
        ConsumeAmmonia(sm, absorbed);

        // Increase power from temperature (Since it can be <0, do the simple check)
        // Negligible unless heatmod is high (one emitter = 3 power per second, for comparison)
        sm.Power += Math.Max(absorbed.Gas.Temperature * heatModifier / Atmospherics.T0C, 0);

        #endregion Add power to crystal

        #region Generate outputs

        // Radiate stuff
        if (TryComp<RadiationSourceComponent>(ent, out var rad))
        {
            rad.Intensity = sm.Power * exergyModifier * sm.RadiationOutputFactor;
        }

        // Convert power to energy
        var energy = sm.Power * sm.ReactionPowerModifier;
        var exhaustGases = new GasMixture();
        // Release the waste. Both are scaled by modifier and energy, but o2 also scales with temperatures.
        exhaustGases.AdjustMoles(Gas.Oxygen, Math.Max(wasteModifier * (energy + absorbed.Gas.Temperature - Atmospherics.T0C) * sm.OxygenReleaseEfficiencyModifier, 0f));
        exhaustGases.AdjustMoles(Gas.Plasma, Math.Max(wasteModifier * sm.PlasmaReleaseModifier * energy, 0f));
        // Increase temperature
        exhaustGases.Temperature = absorbed.Gas.Temperature + energy * sm.ThermalReleaseModifier;
        // Add exhausts back to the mix
        _atmosphere.Merge(absorbed.Gas, exhaustGases);

        #endregion Generate outputs

        // Scale down pwr
        sm.Power -= sm.PowerToRemove() * co2Modifier;
    }

    private static void ConsumeMatterPower(SupermatterComponent sm)
    {
        if (sm.MatterPower <= 0)
        {
            return;
        }
        // Get how much matter power to transfer
        var removedMatter = Math.Clamp(sm.MatterPower, 0f, sm.MatterPowerConsumedPerCycle * sm.MatterPowerConversion);
        // And transfer it around
        sm.Power += removedMatter;
        sm.MatterPower -= removedMatter;
    }

    private static void ConsumeAmmonia(SupermatterComponent sm, in GasWrapper gas)
    {
        // Yeah, it consumes all ammonia in one tick since we have the percentage anyways.
        var ammoniaGasMoles = gas.Gas.GetMoles(Gas.Ammonia);
        gas.Gas.SetMoles(Gas.Ammonia, 0f);
        sm.Power += ammoniaGasMoles * sm.AmmoniaEnergyPerMole;
    }

    private static float GetCo2Modifier(SupermatterComponent sm, in GasWrapper absorbed)
    {
        var co2Ratio = absorbed.Gas.GetGasMolarPercentage(Gas.CarbonDioxide);
        var underThresholdScaler = Math.Min(
            Math.Clamp(co2Ratio / sm.Co2PercentageForPowerInhibition, 0, 1),
            Math.Clamp(absorbed.Gas.TotalMoles / sm.MoleCountForPowerInhibition, 0, 1)
            );
        var moleBoost = Math.Clamp(absorbed.Gas.TotalMoles / sm.MoleCountForPowerInhibitionBoost, 1f, 1.5f);

        // Apply CO2 ratio if thresholds are met, otherwise limit the ratio according to how far away we are from thresholds
        var powerlossDynamicScaling = co2Ratio * underThresholdScaler;
        return Math.Clamp(1f - powerlossDynamicScaling * moleBoost, 0f, 1f);
    }
}
