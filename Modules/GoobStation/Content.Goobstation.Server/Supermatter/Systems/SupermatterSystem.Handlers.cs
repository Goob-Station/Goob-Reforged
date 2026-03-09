// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using Content.Goobstation.Shared.Supermatter.Components;
using Content.Shared.Atmos;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Kitchen.Components;
using Content.Shared.Mobs.Components;
using Content.Shared.Projectiles;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Events;

namespace Content.Goobstation.Server.Supermatter.Systems;

public sealed partial class SupermatterSystem
{
    private void OnCollideEvent(Entity<SupermatterComponent> ent, ref StartCollideEvent args)
    {
        var sm = ent.Comp;
        var target = args.OtherEntity;

        // Stop immune entities from activating the sm.
        if (args.OtherBody.BodyType is BodyType.Static
            || HasComp<SupermatterImmuneComponent>(target)
            || string.Equals(MetaData(target).EntityPrototype?.ID, sm.AshPrototypeId, StringComparison.Ordinal)
            || _container.IsEntityInContainer(ent))
        {
            return;
        }

        // Enable SM
        if (!sm.Activated)
        {
            var activator = ToPrettyString(args.OtherEntity);

            _sharedChat.SendAdminAlert($"Supermatter activated by {activator} at {Transform(ent).Coordinates}");

            _adminLog.Add(LogType.Action,
                LogImpact.High,
                $"Supermatter activated by {activator} at {Transform(ent).Coordinates}");

            sm.Activated = true;
        }

        // Gain power
        sm.Power += GetPowerFromEntity(ent);

        sm.MatterPower += HasComp<MobStateComponent>(target) ? 10 : 0;

        // Consume
        if (!HasComp<ProjectileComponent>(target))
        {
            _adminLog.Add(LogType.EntityDelete, LogImpact.Medium, $"Supermatter {ToPrettyString(ent)} has consumed {ToPrettyString(target)}");
            EntityManager.SpawnAttachedTo(sm.AshPrototypeId, Transform(target).Coordinates);
            _audio.PlayPvs(sm.DustSound, ent);
        }

        EntityManager.QueueDeleteEntity(target);
    }

    private float GetPowerFromEntity(in EntityUid target)
    {
        if (TryComp<SupermatterFoodComponent>(target, out var food))
        {
            return food.Energy;
        }
        return TryComp<ProjectileComponent>(target, out var projectile) ? (float)projectile.Damage.GetTotal() : 1f;
    }

    private void OnHandInteract(Entity<SupermatterComponent> ent, ref InteractHandEvent args)
    {
        var target = args.User;
        var sm = ent.Comp;
        if (HasComp<SupermatterImmuneComponent>(target))
        {
            return;
        }

        if (!sm.Activated)
        {
            sm.Activated = true;
        }

        sm.MatterPower += 10;

        EntityManager.SpawnEntity(sm.AshPrototypeId, Transform(target).Coordinates);
        _audio.PlayPvs(sm.DustSound, ent);
        EntityManager.QueueDeleteEntity(target);
    }

    private void OnItemInteract(Entity<SupermatterComponent> ent, ref InteractUsingEvent args)
    {
        var sm = ent.Comp;

        // Can we remove it?
        if (sm.SliverRemoved || !HasComp<SharpComponent>(args.Used))
        {
            return;
        }

        if (!sm.Activated)
        {
            sm.Activated = true;
        }

        var dae = new DoAfterArgs(EntityManager, args.User, 30f, new SupermatterDoAfterEvent(), ent)
        {
            BreakOnDamage = true,
            BreakOnHandChange = false,
            BreakOnMove = true,
            BreakOnWeightlessMove = false,
            NeedHand = true,
            RequireCanInteract = true,
        };

        _doAfter.TryStartDoAfter(dae);
    }

    private void OnGetSliver(Entity<SupermatterComponent> ent, ref SupermatterDoAfterEvent args)
    {
        if (args.Cancelled)
        {
            return;
        }
        var sm = ent.Comp;

        sm.SliverRemoved = true;
        // 10% of total durability
        sm.Damage += sm.DelaminationPoint / 10f;
        sm.DamageDelta += sm.DelaminationPoint / 10f;

        _chat.DispatchSupermatterAnnouncement(ent, Loc.GetString("supermatter-announcement-cc-tamper", ("integrity", sm.IntegrityString)), global: true, "Central Command");

        Spawn(sm.SliverPrototypeId, _transform.GetMapCoordinates(args.User));

        if (sm.DelamTimer <= 30f)
        {
            return;
        }
        sm.DelamTimer -= 10f;
    }

    private void OnExamine(Entity<SupermatterComponent> ent, ref ExaminedEvent args)
    {
        // get all close and personal to it
        if (!args.IsInDetailsRange)
        {
            return;
        }
        args.PushMarkup(Loc.GetString("supermatter-examine-integrity", ("integrity", ent.Comp.IntegrityString)));
    }

    private void OnComponentRemove(Entity<SupermatterComponent> ent, ref ComponentRemove args)
    {
        // turn off any ambient if component is removed (ex. entity deleted)
        _ambient.SetAmbience(ent, value: false);
        ent.Comp.AudioStream = _audio.Stop(ent.Comp.AudioStream);
    }

    private void OnMapInit(Entity<SupermatterComponent> ent, ref MapInitEvent args)
    {
        // Set the Sound
        _ambient.SetAmbience(ent, value: true);

        // Add Air to the initialized SM in the Map so it doesn't delam on default
        if (!_atmosphere.TryGetContainingMixture(out var mix, ent))
        {
            return;
        }
        mix.AdjustMoles(Gas.Oxygen, Atmospherics.OxygenMolesStandard);
        mix.AdjustMoles(Gas.Nitrogen, Atmospherics.NitrogenMolesStandard);
    }
}
