using Robust.Shared.Containers;
using Robust.Shared.Timing;

namespace Content.Goobstation.Shared.Containers;

public sealed partial class GoobContainerSystem : EntitySystem
{
    [Dependency] private IGameTiming _timing = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ContainerComponentGrantComponent, EntInsertedIntoContainerMessage>(OnInsert);
        SubscribeLocalEvent<ContainerComponentGrantComponent, EntRemovedFromContainerMessage>(OnRemoved);
    }

    private void OnInsert(Entity<ContainerComponentGrantComponent> ent, ref EntInsertedIntoContainerMessage args)
    {
        if (!_timing.ApplyingState)
            EntityManager.AddComponents(args.Entity, ent.Comp.Components, false);
    }

    private void OnRemoved(Entity<ContainerComponentGrantComponent> ent, ref EntRemovedFromContainerMessage args)
    {
        if (!_timing.ApplyingState)
            EntityManager.RemoveComponents(args.Entity, ent.Comp.Components);
    }
}
