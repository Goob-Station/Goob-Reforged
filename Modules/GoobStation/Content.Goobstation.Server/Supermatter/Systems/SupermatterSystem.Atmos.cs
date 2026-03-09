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

        var (radModifier, _, moleModifier, heatModifier, _) = absorbed.Gas.GetGasModifiers();

        var co2Modifier = GetCo2Modifier(sm, absorbed);

        #endregion Get gas mix and modifiers

        #region Add power to crystal

        ConsumeMatterPower(sm);
        ConsumeAmmonia(sm, absorbed);

        // Increase power from temperature (Since it can be <0, do the simple check)
        sm.Power = Math.Max(absorbed.Gas.Temperature * heatModifier / Atmospherics.T0C + sm.Power, 0);

        #endregion Add power to crystal

        #region Generate outputs

        // Radiate stuff
        if (TryComp<RadiationSourceComponent>(ent, out var rad))
        {
            rad.Intensity = sm.Power * radModifier * sm.RadiationOutputFactor;
        }

        // Convert power to energy
        var energy = sm.Power * sm.ReactionPowerModifier;

        // Release the waste. Both are scaled by modifier and energy, but o2 also scales with temperatures.
        absorbed.Gas.AdjustMoles(Gas.Oxygen, Math.Max(moleModifier * (energy + absorbed.Gas.Temperature - Atmospherics.T0C) * sm.OxygenReleaseEfficiencyModifier, 0f));
        absorbed.Gas.AdjustMoles(Gas.Plasma, Math.Max(moleModifier * sm.PlasmaReleaseModifier * energy, 0f));

        // Increase temperature
        absorbed.Gas.Temperature += energy * sm.ThermalReleaseModifier;

        #endregion Generate outputs

        #region Scale down power

        // I'd recommend plotting these two if you want to get it but in general this lets it need less input to stay under power threshold/scaler than above
        // Hardcoded to discourage YAML majors
        const float powerReductionScaler = 5f;
        var powerReduction = float.Pow(sm.Power / powerReductionScaler, 3f);

        // Atp power is lowered
        sm.Power = Math.Max(sm.Power - Math.Min(powerReduction, sm.Power * (1 - 1 / powerReductionScaler)) * co2Modifier, 0f);

        #endregion Scale down power
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
        // Yeah, it consumes all ammonia in one tick cuz it's funny af
        var ammoniaGasMoles = gas.Gas.GetMoles(Gas.Ammonia);
        gas.Gas.SetMoles(Gas.Ammonia, 0f);
        sm.Power += ammoniaGasMoles * sm.AmmoniaEnergyPerMole;
    }

    private static float GetCo2Modifier(SupermatterComponent sm, in GasWrapper absorbed)
    {
        var co2Ratio = absorbed.Gas.GetGasMolarPercentage(Gas.CarbonDioxide);
        var underThresholdScaler = Math.Min(
            Math.Clamp(co2Ratio / sm.PowerlossInhibitionGasThreshold, 0, 1),
            Math.Clamp(absorbed.Gas.TotalMoles / sm.PowerlossInhibitionMoleThreshold, 0, 1)
            );
        var moleBoost = Math.Clamp(absorbed.Gas.TotalMoles / sm.PowerlossInhibitionMoleBoostThreshold, 1f, 1.5f);

        // Apply CO2 ratio if thresholds are met, otherwise limit the ratio according to how far away we are from thresholds
        var powerlossDynamicScaling = co2Ratio * underThresholdScaler;
        return Math.Clamp(1f - powerlossDynamicScaling * moleBoost, 0f, 1f);
    }
}
