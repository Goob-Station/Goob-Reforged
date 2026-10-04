using Content.Goobstation.Shared.Genetics.Components;
using Content.Goobstation.Shared.Genetics.Events;
using Content.Goobstation.Shared.Genetics.Prototypes;
using Content.Shared.Popups;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Goobstation.Shared.Genetics.Systems;

/// <summary>
/// System for managing the presence of mutations on a mutable entity.
/// </summary>
public sealed partial class MutationSystem : EntitySystem
{
    [Dependency] private IPrototypeManager _proto = default!;
    [Dependency] private IRobustRandom _random = default!;
    [Dependency] private INetManager _net = default!;
    [Dependency] private SharedPopupSystem _popup = default!;

    [Dependency] private EntityQuery<MutableComponent> _mutableQuery;

    /// <summary>
    /// Adds a mutation. Can be cancelled by intercepting <see cref="MutationAddingEvent"/>.
    /// </summary>
    private bool AddMutation(Entity<MutableComponent> ent,
    MutationPrototype proto, bool predicted = true)
    {
        if (!predicted && _net.IsClient)
            throw new InvalidOperationException("Called from client when not predicted??");

        var addingEv = new MutationAddingEvent(ent, proto, predicted);
        RaiseLocalEvent(ent, ref addingEv);

        if (addingEv.Cancelled)
            return false;

        ent.Comp.ActiveMutations.Add(proto);

        _popup.PopupEntity(proto.FlavorGain, ent, ent);

        var addedEv = new MutationAddedEvent(ent, proto, predicted);
        RaiseLocalEvent(ent, ref addedEv);

        Dirty(ent);

        return true;
    }

    /// <summary>
    /// Removes a mutation. Can be cancelled by intercepting <see cref="MutationRemovingEvent"/>.
    /// </summary>
    private bool RemMutation(Entity<MutableComponent> ent,
        MutationPrototype proto, bool predicted = true)
    {
        if (!predicted && _net.IsClient)
            throw new InvalidOperationException("Called from client when not predicted??");

        var removingEv = new MutationRemovingEvent(ent, proto, predicted);
        RaiseLocalEvent(ent, ref removingEv);

        if (removingEv.Cancelled)
            return false;

        ent.Comp.ActiveMutations.Remove(proto);

        var removedEv = new MutationRemovedEvent(ent, proto, predicted);
        RaiseLocalEvent(ent, ref removedEv);

        Dirty(ent);

        return true;
    }
}
