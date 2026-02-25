using Content.Goobstation.Shared.Supermatter.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;

namespace Content.Goobstation.Server.Supermatter.Systems;

public sealed class SupermatterDamageSystem : EntitySystem
{
    [Dependency] private readonly AtmosphereSystem _atmosphere = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SupermatterComponent, SupermatterTickEvent>(HandleDamage);
    }

    /// <summary>
    ///     Handles environmental damage.
    /// </summary>
    private void HandleDamage(EntityUid uid, SupermatterComponent sm, SupermatterTickEvent ev)
    {
        var damageArchived = sm.Damage;

        #region Get gas info

        var mix = _atmosphere.GetContainingMixture(uid, true, true);

        // We're in space or there is no gas to process
        if (mix is not { } || mix.TotalMoles == 0f)
        {
            sm.Damage += Math.Max(sm.Power / 1000 * sm.DamageIncreaseMultiplier, 0.1f);
            return;
        }

        // Absorbed gas from surrounding area
        using var surrounding = new GasWrapper(mix, sm.GasEfficiency, _atmosphere);
        var moles = surrounding.Gas.TotalMoles;
        var (_, _, _, _, heatResistModifier) = surrounding.Gas.GetGasModifiers();

        #endregion

        var totalDamage = 0f;

        var tempThreshold = (Atmospherics.T0C + sm.HeatPenaltyThreshold) * heatResistModifier;

        // Temperature start to have a positive effect on damage after 350
        var tempDamage = Math.Max(Math.Clamp(moles / 200f, .5f, 1f) * surrounding.Gas.Temperature - tempThreshold, 0f) * sm.MoleHeatThreshold / 150f * sm.DamageIncreaseMultiplier;
        totalDamage += tempDamage;

        // Power only starts affecting damage when it is above 5000
        var powerDamage = Math.Max(sm.Power - sm.PowerPenaltyThreshold, 0f) / 500f * sm.DamageIncreaseMultiplier;
        totalDamage += powerDamage;

        // Molar count only starts affecting damage when it is above 1800
        var moleDamage = Math.Max(moles - sm.MolePenaltyThreshold, 0) / 80 * sm.DamageIncreaseMultiplier;
        totalDamage += moleDamage;

        // Healing damage
        if (moles < sm.MolePenaltyThreshold)
        {
            // left there a very small float value so that it doesn't eventually divide by 0.
            var healHeatDamage = Math.Min(surrounding.Gas.Temperature - tempThreshold, 0.001f) / 150;
            totalDamage += healHeatDamage;
        }

        sm.Damage = Math.Min(damageArchived + sm.DamageHardcap * sm.DelaminationPoint, totalDamage);
        sm.DamageDelta = sm.Damage - damageArchived;
    }
}
