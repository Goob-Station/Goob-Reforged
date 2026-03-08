// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using Content.Goobstation.Shared.Supermatter.Components;
using Content.Shared.Atmos;
using System;

namespace Content.Goobstation.Server.Supermatter.Systems;

public sealed partial class SupermatterSystem
{
    /// <summary>
    ///     Handles environmental damage.
    /// </summary>
    private void HandleDamage(Entity<SupermatterComponent> ent)
    {
        var sm = ent.Comp;
        var damageArchived = sm.Damage;

        // Vacuum bypass
        if (!_atmosphere.TryGetContainingMixture(out var mix, ent.Owner))
        {
            sm.Damage += Math.Max(sm.Power / 1000 * sm.DamageIncreaseMultiplier, 0.1f);
            return;
        }

        // Absorbed gas from surrounding area
        using var surrounding = new GasWrapper(mix, sm.GasEfficiency, _atmosphere);
        var moles = surrounding.Gas.TotalMoles;
        var (_, _, _, _, heatResistModifier) = surrounding.Gas.GetGasModifiers();

        var totalDamage = 0f;

        var tempThreshold = (Atmospherics.T0C + sm.HeatPenaltyThreshold) * heatResistModifier;

        // Scale down the hot gas damage for low molar counts
        totalDamage += Math.Max(Math.Clamp(moles / 200f, .5f, 1f) * surrounding.Gas.Temperature - tempThreshold, 0f) * sm.MoleHeatPenalty / 150f;

        totalDamage += Math.Max(sm.Power - sm.PowerPenaltyThreshold, 0f) / 500f;

        totalDamage += Math.Max(moles - sm.MolePenaltyThreshold, 0) / 80f;

        totalDamage *= sm.DamageIncreaseMultiplier;

        // Healing damage
        if (moles < sm.MolePenaltyThreshold)
        {
            var healHeatDamage = Math.Min(surrounding.Gas.Temperature - tempThreshold, 0f) / 150;
            totalDamage += healHeatDamage;
        }

        // Cap damage per cycle
        sm.Damage = Math.Min(damageArchived + sm.DamageHardcapPercentage * sm.DelaminationPoint, totalDamage);

        sm.DamageDelta = sm.Damage - damageArchived;
    }
}
