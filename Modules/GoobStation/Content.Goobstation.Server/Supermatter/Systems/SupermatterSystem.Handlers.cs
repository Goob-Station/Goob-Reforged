// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using Content.Goobstation.Shared.Supermatter.Components;
using Content.Goobstation.Shared.Supermatter.Systems;
using Content.Server.Audio;
using Content.Server.DoAfter;
using Content.Shared.Administration.Logs;
using Content.Shared.Atmos;
using Content.Shared.Chat;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Kitchen.Components;
using Content.Shared.Mobs.Components;
using Content.Shared.Projectiles;
using Robust.Shared.Containers;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Events;

namespace Content.Goobstation.Server.Supermatter.Systems;

public sealed partial class SupermatterSystem : SharedSupermatterSystem
{
    [Dependency] private readonly AmbientSoundSystem _ambient = default!;
    [Dependency] private readonly DoAfterSystem _doAfter = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly ISharedAdminLogManager _adminLog = default!;
    [Dependency] private readonly ISharedChatManager _sharedChat = default!;
    [Dependency] private readonly SharedContainerSystem _container = default!;

    private void OnCollideEvent(EntityUid uid, SupermatterComponent sm, ref StartCollideEvent args)
    {
        var target = args.OtherEntity;

        // Stop immune entities from activating the sm.
        if (args.OtherBody.BodyType == BodyType.Static
            || HasComp<SupermatterImmuneComponent>(target)
            || MetaData(target).EntityPrototype?.ID == sm.AshPrototypeId
            || _container.IsEntityInContainer(uid))
            return;

        if (!sm.Activated)
        {
            // Extra logging for supermatter
            var activator = ToPrettyString(args.OtherEntity);

            _sharedChat.SendAdminAlert($"Supermatter activated by {activator} at {Transform(uid).Coordinates}");

            _adminLog.Add(LogType.Action, LogImpact.High,
                $"Supermatter activated by {activator} at {Transform(uid).Coordinates}");

            sm.Activated = true;
        }

        if (TryComp<SupermatterFoodComponent>(target, out var food))
            sm.Power += food.Energy;
        else if (TryComp<ProjectileComponent>(target, out var projectile))
            sm.Power += (float)projectile.Damage.GetTotal();
        else
            sm.Power++;

        sm.MatterPower += HasComp<MobStateComponent>(target) ? 10 : 0;

        if (!HasComp<ProjectileComponent>(target))
        {
            _adminLog.Add(LogType.EntityDelete, LogImpact.Medium, $"Supermatter {ToPrettyString(uid)} has consumed {ToPrettyString(target)}");
            EntityManager.SpawnAttachedTo(sm.AshPrototypeId, Transform(target).Coordinates);
            _audio.PlayPvs(sm.DustSound, uid);
        }

        EntityManager.QueueDeleteEntity(target);
    }

    private void OnHandInteract(EntityUid uid, SupermatterComponent sm, ref InteractHandEvent args)
    {
        var target = args.User;

        if (HasComp<SupermatterImmuneComponent>(target))
            return;

        if (!sm.Activated)
            sm.Activated = true;

        sm.MatterPower += 10;

        EntityManager.SpawnEntity(sm.AshPrototypeId, Transform(target).Coordinates);
        _audio.PlayPvs(sm.DustSound, uid);
        EntityManager.QueueDeleteEntity(target);
    }

    private void OnItemInteract(EntityUid uid, SupermatterComponent sm, ref InteractUsingEvent args)
    {
        if (!HasComp<SupermatterImmuneComponent>(args.User))
            return;

        if (!sm.Activated)
            sm.Activated = true;

        if (sm.SliverRemoved)
            return;

        if (!HasComp<SharpComponent>(args.Used))
            return;

        var dae = new DoAfterArgs(EntityManager, args.User, 30f, new SupermatterDoAfterEvent(), uid)
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

    private void OnGetSliver(EntityUid uid, SupermatterComponent sm, ref SupermatterDoAfterEvent args)
    {
        if (args.Cancelled)
            return;

        // your criminal actions will not go unnoticed
        sm.Damage += sm.DelaminationPoint / 10;
        sm.DamageDelta += sm.DelaminationPoint / 10;

        var integrity = sm.GetIntegrity().ToString("0.00");
        _chat.SupermatterAnnouncement(uid, Loc.GetString("supermatter-announcement-cc-tamper", ("integrity", integrity)), true, "Central Command");

        Spawn(sm.SliverPrototypeId, _transform.GetMapCoordinates(args.User));

        if (sm.DelamTimer > 30f)
            sm.DelamTimer -= 10f;
    }

    private void OnExamine(EntityUid uid, SupermatterComponent sm, ref ExaminedEvent args)
    {
        // get all close and personal to it
        if (args.IsInDetailsRange)
        {
            args.PushMarkup(Loc.GetString("supermatter-examine-integrity", ("integrity", sm.GetIntegrity().ToString("0.00"))));
        }
    }

    private void OnComponentRemove(EntityUid uid, SupermatterComponent component, ComponentRemove args)
    {
        // turn off any ambient if component is removed (ex. entity deleted)
        _ambient.SetAmbience(uid, false);
        component.AudioStream = _audio.Stop(component.AudioStream);
    }

    private void OnMapInit(EntityUid uid, SupermatterComponent component, MapInitEvent args)
    {
        // Set the Sound
        _ambient.SetAmbience(uid, true);

        //Add Air to the initialized SM in the Map so it doesnt delam on default
        var mix = _atmosphere.GetContainingMixture(uid, true, true);
        mix?.AdjustMoles(Gas.Oxygen, Atmospherics.OxygenMolesStandard);
        mix?.AdjustMoles(Gas.Nitrogen, Atmospherics.NitrogenMolesStandard);
    }
}
