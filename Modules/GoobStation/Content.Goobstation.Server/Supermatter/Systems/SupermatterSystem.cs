// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using Content.Goobstation.Shared.Supermatter.Components;
using Content.Goobstation.Shared.Supermatter.Systems;
using Content.Server.AlertLevel;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Audio;
using Content.Server.Chat.Systems;
using Content.Server.DoAfter;
using Content.Server.Explosion.EntitySystems;
using Content.Server.Lightning;
using Content.Server.Station.Systems;
using Content.Shared.Administration.Logs;
using Content.Shared.Chat;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.Physics.Events;
using Robust.Shared.Timing;

namespace Content.Goobstation.Server.Supermatter.Systems;

public sealed partial class SupermatterSystem : SharedSupermatterSystem
{
    [Dependency] private readonly SharedAudioSystem _audio = null!;
    [Dependency] private readonly IGameTiming _gameTiming = null!;
    [Dependency] private readonly LightningSystem _lightning = null!;
    [Dependency] private readonly ExplosionSystem _explosion = null!;
    [Dependency] private readonly ChatSystem _chat = null!;
    [Dependency] private readonly AlertLevelSystem _alert = null!;
    [Dependency] private readonly StationSystem _station = null!;
    [Dependency] private readonly AtmosphereSystem _atmosphere = null!;
    [Dependency] private readonly AmbientSoundSystem _ambient = null!;
    [Dependency] private readonly DoAfterSystem _doAfter = null!;
    [Dependency] private readonly SharedTransformSystem _transform = null!;
    [Dependency] private readonly ISharedAdminLogManager _adminLog = null!;
    [Dependency] private readonly ISharedChatManager _sharedChat = null!;
    [Dependency] private readonly SharedContainerSystem _container = null!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SupermatterComponent, ComponentRemove>(OnComponentRemove);
        SubscribeLocalEvent<SupermatterComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<SupermatterComponent, StartCollideEvent>(OnCollideEvent);
        SubscribeLocalEvent<SupermatterComponent, InteractHandEvent>(OnHandInteract);
        SubscribeLocalEvent<SupermatterComponent, InteractUsingEvent>(OnItemInteract);
        SubscribeLocalEvent<SupermatterComponent, ExaminedEvent>(OnExamine);
        SubscribeLocalEvent<SupermatterComponent, SupermatterDoAfterEvent>(OnGetSliver);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (!_gameTiming.IsFirstTimePredicted)
            return;

        var smEqe = EntityManager.EntityQueryEnumerator<SupermatterComponent>();

        while (smEqe.MoveNext(out var entity, out var comp))
        {
            var ent = (entity, comp);
            if (!comp.Activated)
                continue;

            comp.UpdateAccumulator += frameTime;

            if (comp.UpdateAccumulator >= comp.UpdateTimer)
            {
                comp.UpdateAccumulator -= comp.UpdateTimer;
                Cycle(ent);
            }
        }
    }

    public void Cycle(Entity<SupermatterComponent> ent)
    {
        ent.Comp.ZapAccumulator++;
        ent.Comp.YellAccumulator++;

        ProcessAtmos(ent.Owner, ent.Comp);
        HandleDamage(ent.Owner, ent.Comp);

        if (ent.Comp.Damage >= ent.Comp.DelaminationPoint || ent.Comp.Delamming)
        {
            Delam(ent);
        }

        HandleSoundLoop(ent.Comp);

        if (ent.Comp.ZapAccumulator >= ent.Comp.ZapTimer)
        {
            ent.Comp.ZapAccumulator -= ent.Comp.ZapTimer;
            Zap(ent);
        }

        if (ent.Comp.YellAccumulator >= ent.Comp.YellTimer)
        {
            ent.Comp.YellAccumulator -= ent.Comp.YellTimer;
            HandleAnnouncements(ent);
        }
    }

    private void HandleSoundLoop(SupermatterComponent sm)
    {
        var isAggressive = sm.Damage > SupermatterComponent.WarningPoint;
        var isDelamming = sm.Damage > sm.DelaminationPoint;

        if (!isAggressive && !isDelamming)
        {
            sm.AudioStream = _audio.Stop(sm.AudioStream);
            return;
        }

        var smSound = isDelamming ? SuperMatterSound.Delam : SuperMatterSound.Aggressive;

        if (sm.SmSound == smSound)
            return;

        sm.AudioStream = _audio.Stop(sm.AudioStream);
        sm.SmSound = smSound;
    }
}
