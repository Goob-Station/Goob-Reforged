// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using Content.Goobstation.Shared.Supermatter.Components;
using Content.Server.Explosion.EntitySystems;
using Content.Server.Lightning;

namespace Content.Goobstation.Server.Supermatter.Systems;

public sealed partial class SupermatterSystem
{
    /// <summary>
    ///     Shoot lightning bolts depensing on accumulated power.
    /// </summary>
    private void HandleZap(EntityUid uid, SupermatterComponent sm)
    {
        // This isn't DRY but erm whatever. Alternatively I can surface this. And add a few params or some weird struct.
        // (Also I can't cleanly run it on top level anyway since damage is independent)
        if (!_atmosphere.TryGetContainingMixture(out var mix, uid))
        {
            return;
        }

        var (_, zapModifier, _, _, _) = mix.GetGasModifiers();

        // Divide power by its threshold to get a value from 0 to 1, then multiply by the amount of possible lightnings
        // Makes it pretty obvious that if SM is shooting out red lightnings something is wrong.
        // And if it shoots too weak lightnings it means that it's underfed. Feed the SM :godo:
        var zapPower = sm.Power * zapModifier / sm.PowerPenaltyThreshold * sm.LightningPrototypes.Length;
        var zapPowerNorm = (int)Math.Clamp(zapPower, 0, sm.LightningPrototypes.Length - 1);
        _lightning.ShootRandomLightnings(uid, 3.5f, sm.Power > sm.PowerPenaltyThreshold ? 3 : 1, sm.LightningPrototypes[zapPowerNorm]);
    }

    /// <summary>
    ///     Handle the end of the station.
    /// </summary>
    private void HandleDelam(Entity<SupermatterComponent> ent)
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
        if(_atmosphere.TryGetContainingMixture(out var mix, ent.Owner))
        {
            if (mix.TotalMoles >= ent.Comp.MolePenaltyThreshold)
                return DelamType.Singulo;
        }
        if (ent.Comp.Power >= ent.Comp.PowerPenaltyThreshold)
            return DelamType.Tesla;

        return DelamType.Explosion;
    }
}
