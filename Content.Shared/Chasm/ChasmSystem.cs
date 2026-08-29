using Content.Shared.ActionBlocker;
using Content.Shared.Chasm.Components;
using Content.Shared.Chasm.Events;
using Content.Shared.Chat;
using Content.Shared.Interaction;
using Content.Shared.Movement.Events;
using Content.Shared.StepTrigger.Systems;
using Content.Shared.Weapons.Misc;
using Content.Shared.Whitelist;
using JetBrains.Annotations;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared.Chasm;

/// <summary>
/// Handles making entities fall into chasms when stepped on.
/// </summary>
public sealed partial class ChasmSystem : EntitySystem
{
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private ActionBlockerSystem _blocker = default!;
    [Dependency] private EntityWhitelistSystem _whitelist = default!;
    [Dependency] private SharedAudioSystem _audio = default!;
    [Dependency] private SharedChatSystem _chat = default!;
    [Dependency] private SharedGrapplingGunSystem _grapple = default!;
    [Dependency] private SharedContainerSystem _container = default!; // Goob

    [Dependency] private EntityQuery<ChasmComponent> _chasmQuery;
    [Dependency] private EntityQuery<ChasmFallingComponent> _chasmFallingQuery;

    // todo marty ask rouden what the fuck was going on here and on upstream merging their shit remove these comments
    // dlso this is why we dont early merge OPEN prs

    /// <inheritdoc />
    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<ChasmFallingComponent>();
        while (query.MoveNext(out var uid, out var chasm))
        {
            // Goobstation Start
            if (_timing.CurTime < chasm.NextEffectsTime)
                continue;

            EndFalling((uid, chasm));
            // Goobstation End
        }
    }

    #region Event Handlers
    [SubscribeLocalEvent]
    private void OnStepTriggered(Entity<ChasmComponent> entity, ref StepTriggeredOffEvent args)
    {
        // already doomed
        if (_chasmFallingQuery.HasComp(args.Tripper))
            return;

        // Check the white-/blacklists and inform on rejection.
        if (!(entity.Comp.Whitelist == null && entity.Comp.Blacklist == null ||
              _whitelist.CheckBoth(args.Tripper, entity.Comp.Blacklist, entity.Comp.Whitelist)))
        {
            var rejected = new FallerRejectedByChasmEvent(args.Tripper);
            RaiseLocalEvent(entity, ref rejected);
            return;
        }

        // Give an opportunity to cancel the fall for whatever reason.
        var checkEvent = new EntityStartFallingAttemptEvent(args.Tripper);
        RaiseLocalEvent(entity, ref checkEvent);
        if (checkEvent.Cancelled)
            return;

        StartFalling(entity.AsNullable(), args.Tripper);
    }

    [SubscribeLocalEvent]
    private void OnStepTriggerAttempt(Entity<ChasmComponent> entity, ref StepTriggerAttemptEvent args)
    {
        if (_grapple.IsEntityHooked(args.Tripper))
        {
            args.Cancelled = true;
            return;
        }

        args.Continue = true;
    }

    [SubscribeLocalEvent]
    private void OnShutdown(Entity<ChasmComponent> entity, ref ComponentShutdown args)
    {
        // Goobstation Start
        foreach (var uid in entity.Comp.FallingEntities)
        {
            if (TerminatingOrDeleted(uid) || !Exists(uid))
                continue;

            var resetVisualsEv = new ResetChasmVisualsEvent();
            RaiseLocalEvent(uid, ref resetVisualsEv);

            RemCompDeferred<ChasmFallingComponent>(uid);
            _blocker.UpdateCanMove(uid);
        }
        // Goobstation End
    }

    [SubscribeLocalEvent]
    private static void OnUpdateCanMove(Entity<ChasmFallingComponent> entity, ref UpdateCanMoveEvent args)
    {
        args.Cancel();
    }

    // Goobstation Start
    [SubscribeLocalEvent]
    private void OnFallingDelete(Entity<ChasmFallingComponent> ent, ref EntityTerminatingEvent args)
    {
        if (ent.Comp.FallChasm is { } chasmUid &&
            _chasmQuery.TryComp(chasmUid, out var chasm))
        {
            chasm.FallingEntities.Remove(ent.Owner);
            DirtyField(chasmUid, chasm, nameof(ChasmComponent.FallingEntities));
        }
    }

    [SubscribeLocalEvent]
    private void OnBeforeInteract(Entity<ChasmFallingComponent> ent, ref InteractHandEvent args)
    {
        args.Handled = true;
    }

    [SubscribeLocalEvent]
    private void OnDeleteFall(Entity<ChasmDeleteComponent> ent, ref ChasmFallEffectsEvent args)
    {
        PredictedQueueDel(args.Entity);
    }

    [SubscribeLocalEvent]
    private void OnContainerFall(Entity<ChasmContainerComponent> ent, ref ChasmFallEffectsEvent args)
    {
        if (!_container.TryGetContainer(ent.Owner, ent.Comp.ContainerId, out var container))
            return;

        _container.Insert(args.Entity, container);
    }
    // Goobstation End

    #endregion Event Handlers

    #region Public API
    /// <summary>
    /// Causes <paramref name="tripper"/> to fall into <paramref name="chasm"/>: starts a falling animation, optionally
    /// plays a sound, and eventually deletes <paramref name="tripper"/>.
    /// If <paramref name="chasm"/> does not have a <see cref="ChasmComponent"/> component, does nothing and returns null.
    /// </summary>
    /// <param name="playSound">Whether or not the chasm should play a sound when the entity falls in.</param>
    /// <param name="playEmote">Whether or not <paramref name="tripper"/> should try to emote when falling into the chasm.</param>
    /// <returns>
    /// <paramref name="tripper"/> with its new <see cref="ChasmFallingComponent"/>, if the entity did start falling, null otherwise.
    /// </returns>
    [PublicAPI]
    public Entity<ChasmFallingComponent>? StartFalling(
        Entity<ChasmComponent?> chasm,
        EntityUid tripper,
        bool playSound = true,
        bool playEmote = true
    )
    {
        if (!_chasmQuery.Resolve(chasm, ref chasm.Comp, logMissing: false))
            return null;

        var falling = AddComp<ChasmFallingComponent>(tripper);
        falling.FallChasm = chasm.Owner; // Goob

        // Goobstation Start
        falling.NextEffectsTime = _timing.CurTime + falling.EffectsTime;
        chasm.Comp.FallingEntities.Add(tripper);
        // Goobstation End

        _blocker.UpdateCanMove(tripper);

        if (playSound)
            _audio.PlayPredicted(chasm.Comp.FallingSound, chasm, tripper);

        if (playEmote && chasm.Comp.Emote is { } emote)
            _chat.TryEmoteWithChat(tripper, emote);

        var chasmEvent = new EntityStartedFallingIntoChasmEvent((tripper, falling));
        RaiseLocalEvent(chasm, ref chasmEvent);
        var tripperEvent = new StartedFallingIntoChasmEvent((chasm, chasm.Comp));
        RaiseLocalEvent(tripper, ref tripperEvent);

        Entity<ChasmFallingComponent> ret = (tripper, falling);
        Dirty(ret);
        DirtyField(chasm, chasm.Comp, nameof(ChasmComponent.FallingEntities)); // Goob
        return ret;
    }

    // Goobstation Start
    /// <summary>
    /// Immediately ends the falling of an entity into a chasm.
    /// </summary>
    /// <param name="tripper">The currently falling entity.</param>
    [PublicAPI]
    public void EndFalling(Entity<ChasmFallingComponent?> tripper)
    {
        if (!_chasmFallingQuery.Resolve(tripper.Owner, ref tripper.Comp, logMissing: false))
            return;

        if (tripper.Comp.FallChasm is not { } chasm)
            return;

        var resetVisualsEv = new ResetChasmVisualsEvent();
        RaiseLocalEvent(tripper.Owner, ref resetVisualsEv);

        var beforeEv = new BeforeChasmFallEvent(chasm);
        RaiseLocalEvent(tripper.Owner, ref beforeEv);
        if (beforeEv.Cancelled)
            return;

        var chasmEvent = new EntityCompletedFallingIntoChasmEvent((tripper.Owner, tripper.Comp));
        RaiseLocalEvent(chasm, ref chasmEvent);

        if (_chasmQuery.TryComp(chasm, out var chasmComp))
        {
            chasmComp.FallingEntities.Remove(tripper.Owner);

            var tripperEvent = new CompletedFallingIntoChasmEvent((chasm, chasmComp));
            RaiseLocalEvent(tripper.Owner, ref tripperEvent);

            DirtyField(chasm, chasmComp, nameof(ChasmComponent.FallingEntities));
        }
        else
        {
            DebugTools.Assert($"{ToPrettyString(chasm)} is missing {nameof(ChasmComponent)} when an entity fell into it!");
        }

        var effectsEv = new ChasmFallEffectsEvent(tripper.Owner);
        RaiseLocalEvent(chasm, ref effectsEv);

        RemComp(tripper.Owner, tripper.Comp);
        _blocker.UpdateCanMove(tripper.Owner);
    }
    // Goobstation End

    #endregion Public API
}
