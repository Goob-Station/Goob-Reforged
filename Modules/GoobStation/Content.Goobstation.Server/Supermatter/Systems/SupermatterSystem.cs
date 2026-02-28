// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using Content.Goobstation.Shared.Supermatter.Components;
using Content.Goobstation.Shared.Supermatter.Systems;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Physics.Events;
using Robust.Shared.Timing;

namespace Content.Goobstation.Server.Supermatter.Systems;

public sealed partial class SupermatterSystem : SharedSupermatterSystem
{
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;

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
            if (!comp.Activated)
                continue;

            comp.UpdateAccumulator += frameTime;

            if (comp.UpdateAccumulator >= comp.UpdateTimer)
            {
                comp.UpdateAccumulator -= comp.UpdateTimer;
                Cycle(entity, comp);
            }
        }
    }

    public void Cycle(EntityUid ent, SupermatterComponent comp)
    {
        comp.ZapAccumulator++;
        comp.YellAccumulator++;

        ProcessAtmos(ent, comp);
        HandleDamage(ent, comp);

        if (comp.Damage >= comp.DelaminationPoint || comp.Delamming)
        {
            HandleDelam(ent, comp);
        }

        HandleSoundLoop(comp);

        if (comp.ZapAccumulator >= comp.ZapTimer)
        {
            comp.ZapAccumulator -= comp.ZapTimer;
            HandleZap(ent, comp);
        }

        if (comp.YellAccumulator >= comp.YellTimer)
        {
            comp.YellAccumulator -= comp.YellTimer;
            HandleAnnouncements(ent, comp);
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
