// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using Content.Goobstation.Shared.Genetics.Components;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Goobstation.Shared.Genetics.Systems;

public sealed partial class MutationSystem : EntitySystem
{
    [SubscribeLocalEvent]
    private void OnMapInit(Entity<MutableComponent> ent, ref MapInitEvent args)
    {
        // initialize dormant mutations n shit
        var pool = _proto.Index(ent.Comp.InitialMutationsPool).Mutations.ShallowClone();

        for (var i = 0; i < ent.Comp.InitialMutationCount; i++)
        {
            var id = _random.PickAndTake(pool);
            ent.Comp.InitialMutations.Add(id);
        }
    }

    [SubscribeLocalEvent]
    private void OnComponentRemove(Entity<MutableComponent> ent, ref ComponentRemove args)
    {
        // undo all the mutations when we're removing mutable component
        foreach (var mutation in ent.Comp.ActiveMutations)
        {
            RemoveMutation(ent, _proto.Index(mutation));
        }
    }

    /*private void OnGetState(Entity<MutableComponent> ent, ref ComponentGetState args)
    {
        args.State = new MutableComponentState(
            ent.Comp.InitialMutations, ent.Comp.ActiveMutations
        );
    }

    private void OnHandleState(Entity<MutableComponent> ent, ref ComponentHandleState args)
    {
        if (args.Current is not MutableComponentState state)
            return;

        var removingMutations = new List<ActiveMutationData>();
        var addingMutations = new List<ActiveMutationData>();

        ent.Comp.InitialMutations = state.InitialMutations;

        foreach (var newMut in state.ActiveMutations)
        {
            var mut = state.ActiveMutations.FirstOrNull(x => x.MutationId == newMut.MutationId);

            // we already have this
            if (mut != null)
                continue;

            addingMutations.Add(newMut);
            Log.Error(newMut.MutationId);
        }

        foreach (var mut in addingMutations)
        {
            ent.Comp.ActiveMutations.Add(mut);
            var ev = new MutationAddedEvent(ent, mut, Predicted: false);
            RaiseLocalEvent(ent, ref ev);
        }

        foreach (var oldMut in ent.Comp.ActiveMutations)
        {
            var mut = ent.Comp.ActiveMutations.FirstOrNull(x => x.MutationId == oldMut.MutationId);

            // we already dont have this
            if (mut is null)
                continue;

            removingMutations.Add(oldMut);
            Log.Error(oldMut.MutationId);
        }

        foreach (var mut in addingMutations)
        {
            ent.Comp.ActiveMutations.Remove(mut);
            var ev = new MutationRemovedEvent(ent, mut, Predicted: false);
            RaiseLocalEvent(ent, ref ev);
        }
    }*/
}
