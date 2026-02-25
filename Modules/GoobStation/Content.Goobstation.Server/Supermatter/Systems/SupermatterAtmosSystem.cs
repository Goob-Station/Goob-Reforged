using Content.Goobstation.Shared.Supermatter.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Radiation.Components;

namespace Content.Goobstation.Server.Supermatter.Systems;

public sealed class SupermatterAtmosSystem : EntitySystem
{
    [Dependency] private readonly AtmosphereSystem _atmosphere = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SupermatterComponent, SupermatterTickEvent>(ProcessAtmos);
    }

    /// <summary>
    ///     Handle power and radiation output depending on atmospheric things.
    /// </summary>
    private void ProcessAtmos(EntityUid uid, SupermatterComponent sm, SupermatterTickEvent ev)
    {
        #region Get gas mix

        var mix = _atmosphere.GetContainingMixture(uid, true, true);

        if (mix is not { })
            return;

        using var absorbed = new GasWrapper(mix, sm.GasEfficiency, _atmosphere);

        var moles = absorbed.Gas.TotalMoles;

        if (!(moles > 0f))
            return;

        #endregion

        var (radModifier, zapModifier, moleModifier, heatModifier, heatResistModifier) = absorbed.Gas.GetGasModifiers();

        #region Calculate CO2 powerloss inhibition effect

        var co2Ratio = absorbed.Gas.GetGasMolarPercentage(Gas.CarbonDioxide);
        var underThresholdScaler = Math.Min(
            Math.Clamp(co2Ratio / sm.PowerlossInhibitionGasThreshold, 0, 1),
            Math.Clamp(moles / sm.PowerlossInhibitionMoleThreshold, 0, 1)
            );

        // Apply CO2 ratio if thresholds are met, otherwise limit the ratio according to how far away we are from thresholds
        sm.PowerlossDynamicScaling = co2Ratio * underThresholdScaler;

        // 
        var moleBoost = Math.Clamp(moles / sm.PowerlossInhibitionMoleBoostThreshold, 1f, 1.5f);
        var powerlossInhibitor = Math.Clamp(1f - sm.PowerlossDynamicScaling * moleBoost, 0f, 1f);

        #endregion

        #region Add power to crystal

        // Transfer matter power to power
        if (sm.MatterPower != 0)
        {
            // Get how much matter power to transfer
            var removedMatter = Math.Clamp(sm.MatterPower, 0f, 1f * sm.MatterPowerConversion);

            sm.Power = Math.Max(sm.Power + removedMatter, 0);
            sm.MatterPower = Math.Max(sm.MatterPower - removedMatter, 0);
        }

        // Increase power from temperature
        sm.Power = Math.Max(absorbed.Gas.Temperature * heatModifier / Atmospherics.T0C + sm.Power, 0);

        // Yeah, it consumes all ammonia in one tick cuz it's funny af
        sm.Power = Math.Max(absorbed.Gas.GetMoles(Gas.Ammonia) * sm.AmmoniaEnergyPerMole + sm.Power, 0);
        absorbed.Gas.SetMoles(Gas.Ammonia, 0f);

        #endregion

        #region Generate outputs

        //Radiate stuff
        if (TryComp<RadiationSourceComponent>(uid, out var rad))
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

        #endregion

        #region Scale down power

        // I'd recommend plotting these two if you want to get it
        // but in general this lets it need less input to stay under 10 power than above
        // Below 10 power it substracts very little, and above it substracts 1/10
        // 10f (and 0.9f) hardcoded to discourage yaml majors messing with it since it impacts a lot
        // (And would require massive structural changes, all to minuscule benefit)
        var powerReduction = (float)Math.Pow(sm.Power / 5f, 3f);

        // After this point power is lowered
        // This wraps around to the begining of the function
        sm.Power = Math.Max(sm.Power - Math.Min(powerReduction, sm.Power * 0.8f) * powerlossInhibitor, 0f);

        #endregion
    }
}
