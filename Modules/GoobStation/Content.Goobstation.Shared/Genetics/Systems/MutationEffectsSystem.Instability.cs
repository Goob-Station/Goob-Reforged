// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using Content.Goobstation.Shared.Genetics.Components;
using Content.Goobstation.Shared.Genetics.Events;
using Content.Shared.Gibbing;

namespace Content.Goobstation.Shared.Genetics.Systems;

public sealed partial class MutationEffectsSystem : EntitySystem
{
    [Dependency] private GibbingSystem _gib = default!;

    private void HandleStabilityChange(Entity<MutableComponent> ent)
    {
        var delta = RecalculateInstability(ent);

        // * Nothing changed.
        if (delta == 0)
            return;

        var somethingcrazy = _proto.Index(ent.Comp.SomethingBadStatusEffect);

        if (ent.Comp.Instability > ent.Comp.MaxInstability)
        {
            _status.TrySetStatusEffectDuration(ent, somethingcrazy);
            _gib.Gib(ent); // TODO LMAo
        }
        else
        {
            _status.TryRemoveStatusEffect(ent, somethingcrazy);
        }
    }

    [SubscribeLocalEvent]
    private void OnMutationRemoving(Entity<MutableComponent> ent, ref MutationRemovingEvent args)
    {
        if (args.Cancelled)
            return;

        args.Cancelled = _status.HasStatusEffect(
            ent,
            _proto.Index(ent.Comp.SomethingBadStatusEffect)
        );
    }

    private int RecalculateInstability(Entity<MutableComponent> ent)
    {
        var cur = ent.Comp.Instability;
        var instability = 0;

        foreach (var activeMutation in ent.Comp.ActiveMutations)
        {
            var mutation = _proto.Index(activeMutation);
            var status = _mutation.GetMutationStatus(ent, mutation);

            if (status.HasMutationDormant)
                continue;

            instability += (int)mutation.Instability;
        }

        ent.Comp.Instability = instability;

        return instability - cur;
    }
}
