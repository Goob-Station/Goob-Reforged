using Content.Shared.Body.Components;
using Content.Shared.Body.Events;
using Content.Shared.Body.Systems;
using Content.Shared.Ghost;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Content.Shared.Mobs.Components;
using Content.Shared.Pointing;
using Content.WoundMod.Shared.Body.Components;
using Content.WoundMod.Shared.Body.Organ;

namespace Content.WoundMod.Shared.Body.Systems;

public sealed class SharedWMBrainSystem : EntitySystem
{
    [Dependency] private readonly SharedMindSystem _mindSystem = default!;
    [Dependency] private readonly SharedBodySystem _bodySystem = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<BrainComponent, ComponentStartup>(OnBrainStartup);
        SubscribeLocalEvent<WMBrainComponent, OrganRemovedFromBodyEvent>(HandleRemoval);
        SubscribeLocalEvent<WMBrainComponent, OrganAddedToBodyEvent>(HandleAddition);
    }

    private void OnBrainStartup(Entity<BrainComponent> ent, ref ComponentStartup args)
    {
        EnsureComp<WMBrainComponent>(ent);
    }

    private void HandleRemoval(Entity<WMBrainComponent> ent, ref OrganRemovedFromBodyEvent args)
    {
        if (TerminatingOrDeleted(ent) || TerminatingOrDeleted(args.OldBody))
            return;

        ent.Comp.Active = false;

        if (!CheckOtherBrains(args.OldBody))
        {
            EnsureComp<DebrainedComponent>(args.OldBody);
            HandleMind(ent, args.OldBody);
        }

        Dirty(ent, ent.Comp);
    }

    private void HandleAddition(Entity<WMBrainComponent> ent, ref OrganAddedToBodyEvent args)
    {
        if (TerminatingOrDeleted(ent) || TerminatingOrDeleted(args.Body))
            return;

        ent.Comp.Active = true;

        if (!CheckOtherBrains(args.Body, excluding: ent))
        {
            RemComp<DebrainedComponent>(args.Body);
            HandleMind(args.Body, ent);
        }

        Dirty(ent, ent.Comp);
    }

    private void HandleMind(EntityUid newEntity, EntityUid oldEntity)
    {
        if (TerminatingOrDeleted(newEntity) || TerminatingOrDeleted(oldEntity))
            return;

        EnsureComp<MindContainerComponent>(newEntity);
        EnsureComp<MindContainerComponent>(oldEntity);

        var ghostOnMove = EnsureComp<GhostOnMoveComponent>(newEntity);
        ghostOnMove.MustBeDead = HasComp<MobStateComponent>(newEntity);

        if (!_mindSystem.TryGetMind(oldEntity, out var mindId, out var mind))
            return;

        _mindSystem.TransferTo(mindId, newEntity, mind: mind);
    }

    private bool CheckOtherBrains(EntityUid entity, EntityUid? excluding = null)
    {
        // Check if the body itself is a brain
        if (TryComp<BrainComponent>(entity, out _)
            && TryComp<WMBrainComponent>(entity, out var bodyWmBrain)
            && bodyWmBrain.Active
            && entity != excluding)
            return true;

        if (!TryComp<BodyComponent>(entity, out var body))
            return false;

        foreach (var (organ, _) in _bodySystem.GetBodyOrgans(entity, body))
        {
            if (organ == excluding)
                continue;

            if (!TryComp<WMBrainComponent>(organ, out var wmBrain) || !wmBrain.Active)
                continue;

            return true;
        }

        return false;
    }
}
