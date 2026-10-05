// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using Content.Goobstation.Shared.Genetics.Components;
using Content.Goobstation.Shared.Genetics.Events;
using Content.Goobstation.Shared.Genetics.Prototypes;
using Content.Shared.StatusEffectNew;
using Robust.Shared.Prototypes;

namespace Content.Goobstation.Shared.Genetics.Systems;

// GOOB GENETICS TODO:
// ok so we need a way to add and remove a status effect on mutation add/remove
// but also we need to make sure you cant just add and remove a mutation to
// remove an undesirable side effect caused by something else..
// Ok so ideally probably we have some sort of status effect type for tracked components
// Although i have no idea for making generic stuff for status effects that would have datafields and stuff
// Because then if we have two copies of advanced generic status effect, they could have different datafield definitions
// And it isnt really clear how to handle it. I figure we have to make a hyper specific system for all that stuff,
// dunno....

/// <summary>
/// Handles the direct effects of mutations being added/removed.
/// </summary>
public sealed partial class MutationEffectsSystem : EntitySystem
{
    [Dependency] private IPrototypeManager _proto = default!;
    [Dependency] private MutationSystem _mutation = default!;
    [Dependency] private StatusEffectsSystem _status = default!;

    // TODO!!!

    [SubscribeLocalEvent]
    private void OnAdded(Entity<MutableComponent> ent, ref MutationAddedEvent args)
    {
        var mutation = _proto.Index(args.MutationId);

        HandleAddEffects(ent, mutation);
        HandleStabilityChange(ent);
    }

    [SubscribeLocalEvent]
    private void OnRemoved(Entity<MutableComponent> ent, ref MutationRemovedEvent args)
    {
        var mutation = _proto.Index(args.MutationId);

        HandleRemoveEffects(ent, mutation);
        HandleStabilityChange(ent);
    }

    private void HandleAddEffects(Entity<MutableComponent> ent, MutationPrototype mutation)
    {
        if (mutation.StatusEffects is null)
            return;

        Log.Error(mutation.StatusEffects.Count.ToString());
        foreach (var effect in mutation.StatusEffects)
        {
            Log.Error(effect);
            _status.TrySetStatusEffectDuration(ent, effect);
        }
    }

    private void HandleRemoveEffects(Entity<MutableComponent> ent, MutationPrototype mutation)
    {
        if (mutation.StatusEffects is null)
            return;

        Log.Error(mutation.StatusEffects.Count.ToString());
        foreach (var effect in mutation.StatusEffects)
        {
            Log.Error(effect);
            _status.TrySetStatusEffectDuration(ent, effect);
        }
    }
}
