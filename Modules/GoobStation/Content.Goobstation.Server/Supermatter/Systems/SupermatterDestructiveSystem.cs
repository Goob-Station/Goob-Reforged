// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using Content.Goobstation.Shared.Supermatter.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Explosion.EntitySystems;
using Content.Server.Lightning;
using static Content.Goobstation.Shared.Supermatter.Systems.SharedSupermatterSystem;

namespace Content.Goobstation.Server.Supermatter.Systems;

public sealed class SupermatterDestructiveSystem : EntitySystem
{
    [Dependency] private readonly AtmosphereSystem _atmosphere = default!;
    [Dependency] private readonly LightningSystem _lightning = default!;
    [Dependency] private readonly ExplosionSystem _explosion = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SupermatterComponent, SupermatterZapEvent>(HandleZap);
        SubscribeLocalEvent<SupermatterComponent, SupermatterDelamEvent>(HandleDelam);
    }

    /// <summary>
    ///     Shoot lightning bolts depensing on accumulated power.
    /// </summary>
    private void HandleZap(EntityUid uid, SupermatterComponent sm, SupermatterZapEvent ev)
    {
        // This isn't DRY but erm whatever. Alternatively I can surface this. And add a few params or some weird struct.
        // (Also I can't cleanly run it on top level anyways since damage is independent

        var mix = _atmosphere.GetContainingMixture(uid, true, true);

        if (mix is not { })
            return;

        var gas = mix.Clone();
        var moles = gas.TotalMoles;

        if (!(moles > 0f))
            return;

        var (_, zapModifier, _, _, _) = gas.GetGasModifiers();

        // Divide power by it's threshold to get a value from 0 to 1, then multiply by the amount of possible lightnings
        // Makes it pretty obvious that if SM is shooting out red lightnings something is wrong.
        // And if it shoots too weak lightnings it means that it's underfed. Feed the SM :godo:
        var zapPower = sm.Power * zapModifier / sm.PowerPenaltyThreshold * sm.LightningPrototypes.Length;
        var zapPowerNorm = (int)Math.Clamp(zapPower, 0, sm.LightningPrototypes.Length - 1);
        _lightning.ShootRandomLightnings(uid, 3.5f, sm.Power > sm.PowerPenaltyThreshold ? 3 : 1, sm.LightningPrototypes[zapPowerNorm]);
    }


    /// <summary>
    ///     Decide on how to delaminate.
    /// </summary>
    public DelamType ChooseDelamType(EntityUid uid, SupermatterComponent sm)
    {
        var mix = _atmosphere.GetContainingMixture(uid, true, true);

        if (mix is { })
        {
            var moles = mix.TotalMoles;

            if (moles >= sm.MolePenaltyThreshold)
                return DelamType.Singulo;
        }

        if (sm.Power >= sm.PowerPenaltyThreshold)
            return DelamType.Tesla;

        return DelamType.Explosion;
    }

    /// <summary>
    ///     Handle the end of the station.
    /// </summary>
    private void HandleDelam(EntityUid uid, SupermatterComponent sm, SupermatterDelamEvent ev)
    {
        var xform = Transform(uid);

        sm.DelamType = ChooseDelamType(uid, sm);

        if (!sm.Delamming)
        {
            sm.Delamming = true;
            var evYap = new SupermatterYapEvent(sm);
            RaiseLocalEvent<SupermatterYapEvent>(ref evYap);
        }
        if (sm.Damage < sm.DelaminationPoint && sm.Delamming)
        {
            sm.Delamming = false;
            var evYap = new SupermatterYapEvent(sm);
            RaiseLocalEvent<SupermatterYapEvent>(ref evYap);
        }

        sm.DelamTimerAccumulator++;

        if (sm.DelamTimer > sm.DelamTimerAccumulator)
            return;

        switch (sm.DelamType)
        {
            case DelamType.Explosion:
            default:
                _explosion.TriggerExplosive(uid);
                break;

            case DelamType.Singulo:
                Spawn(sm.SingularityPrototypeId, xform.Coordinates);
                break;

            case DelamType.Tesla:
                Spawn(sm.TeslaPrototypeId, xform.Coordinates);
                break;

            case DelamType.Cascade:
                Spawn(sm.SupermatterKudzuPrototypeId, xform.Coordinates);
                break;
        }
    }
}
