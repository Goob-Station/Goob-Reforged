// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using Content.Goobstation.Shared.Supermatter.Components;
using Robust.Shared.Maths;

namespace Content.Goobstation.Server.Supermatter.Systems;

public sealed partial class SupermatterSystem
{
    /// <summary>
    ///     Shoot lightning bolts depensing on accumulated power.
    /// </summary>
    private void Zap(Entity<SupermatterComponent> ent)
    {
        if (!_atmosphere.TryGetContainingMixture(out var mix, ent))
        {
            return;
        }

        var (_, zapModifier, _, _, _) = mix.GetGasModifiers();

        // Divide power by its threshold to get a value from 0 to 1, then multiply by the amount of possible lightnings
        // Makes it pretty obvious that if SM is shooting out red lightnings something is wrong.
        // And if it shoots too weak lightnings it means that it's underfed. Feed the SM :godo:
        var zapPowerNorm = (int)(ent.Comp.LightningPrototypes.Length * MathHelper.Clamp01(ent.Comp.Power * zapModifier / ent.Comp.PowerPenaltyThreshold));
        _lightning.ShootRandomLightnings(ent, 3.5f, ent.Comp.Power > ent.Comp.PowerPenaltyThreshold ? 3 : 1, ent.Comp.LightningPrototypes[zapPowerNorm - 1]);
    }

    /// <summary>
    ///     Handle the end of the station.
    /// </summary>
    private void Delam(Entity<SupermatterComponent> ent)
    {
        var xform = Transform(ent.Owner);

        var delamType = GetDelamType(ent);

        if (!ent.Comp.Delamming)
        {
            ent.Comp.Delamming = true;
            HandleAnnouncements(ent);
        }
        if (ent.Comp.Damage < ent.Comp.DelaminationPoint && ent.Comp.Delamming)
        {
            ent.Comp.Delamming = false;
            HandleAnnouncements(ent);
        }

        ent.Comp.DelamTimerAccumulator++;

        if (ent.Comp.DelamTimer > ent.Comp.DelamTimerAccumulator)
            return;

        switch (delamType)
        {
            case DelamType.Explosion:
            default:
                _explosion.TriggerExplosive(ent.Owner);
                break;

            case DelamType.Singulo:
                Spawn(ent.Comp.SingularityPrototypeId, xform.Coordinates);
                break;

            case DelamType.Tesla:
                Spawn(ent.Comp.TeslaPrototypeId, xform.Coordinates);
                break;

            case DelamType.Cascade:
                Spawn(ent.Comp.SupermatterKudzuPrototypeId, xform.Coordinates);
                break;
        }
    }

    /// <summary>
    ///     Decide on how to delaminate.
    /// </summary>
    public DelamType GetDelamType(Entity<SupermatterComponent> ent)
    {
        if (_atmosphere.TryGetContainingMixture(out var mix, ent.Owner))
        {
            if (mix.TotalMoles >= ent.Comp.MolePenaltyThreshold)
                return DelamType.Singulo;
        }
        if (ent.Comp.Power >= ent.Comp.PowerPenaltyThreshold)
            return DelamType.Tesla;

        return DelamType.Explosion;
    }
}
