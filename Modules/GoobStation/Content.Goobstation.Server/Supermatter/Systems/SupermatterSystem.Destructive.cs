// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using Content.Goobstation.Shared.Supermatter.Components;
using Robust.Shared.Maths;

namespace Content.Goobstation.Server.Supermatter.Systems;

public sealed partial class SupermatterSystem
{
    /// <summary>Shoot lightning bolts depensing on accumulated power.</summary>
    /// <param name="ent">Entity to shoot lightning from.</param>
    private void Zap(Entity<SupermatterComponent> ent)
    {
        var zapModifier = 1f;
        if (_atmosphere.TryGetContainingMixture(out var mix, ent))
        {
            (_, zapModifier, _, _, _) = mix.GetGasModifiers();
        }

        // Divide power by its threshold to get a value from 0 to 1, then multiply by the amount of possible lightnings
        // Makes it pretty obvious that if SM is shooting out red lightnings something is wrong.
        // And if it shoots too weak lightnings it means that it's underfed. Feed the SM :godo:
        var comp = ent.Comp;
        var powerRatio = comp.Power * zapModifier / comp.PowerPenaltyThreshold;
        var clampedRatio = MathHelper.Clamp01(powerRatio);
        var zapPowerNorm = Convert.ToInt32((comp.LightningPrototypes.Count - 1) * clampedRatio);

        _lightning.ShootRandomLightnings(ent, 3.5f, comp.Power > comp.PowerPenaltyThreshold ? 3 : 1, comp.LightningPrototypes[zapPowerNorm]);
    }

    /// <summary>Handle the end of the station.</summary>
    /// <param name="ent">Entity to use as basis.</param>
    private void Delam(Entity<SupermatterComponent> ent)
    {
        var comp = ent.Comp;

        // If not delamming, then start. If delamming and under delam point, cancel.
        comp.Delamming = !comp.Delamming || comp.Damage >= comp.DelaminationPoint;

        // In both cases let everyone know
        if (!comp.Delamming || comp.Damage < comp.DelaminationPoint)
        {
            HandleAnnouncements(ent);
        }

        comp.DelamTimer++;

        if (comp.DelamTimer < comp.DelamDuration)
        {
            return;
        }
        var coords = Transform(ent).Coordinates;
        switch (GetDelamType(ent))
        {
            // Also catches DelamType.Explosion
            default:
                _explosion.TriggerExplosive(ent);
                break;

            case DelamType.Singulo:
                Spawn(comp.SingularityPrototypeId, coords);
                break;

            case DelamType.Tesla:
                Spawn(comp.TeslaPrototypeId, coords);
                break;

            case DelamType.Cascade:
                Spawn(comp.SupermatterKudzuPrototypeId, coords);
                break;
        }
    }

    /// <summary>Decide on how to delaminate.</summary>
    /// <param name="ent">Entity to retrieve the delam type for.</param>
    internal DelamType GetDelamType(Entity<SupermatterComponent> ent)
    {
        var comp = ent.Comp;
        if (_atmosphere.TryGetContainingMixture(out var mix, ent.Owner) && mix.TotalMoles >= comp.MolePenaltyThreshold)
        {
            return DelamType.Singulo;
        }

        return comp.Power >= comp.PowerPenaltyThreshold ? DelamType.Tesla : DelamType.Explosion;
    }
}
