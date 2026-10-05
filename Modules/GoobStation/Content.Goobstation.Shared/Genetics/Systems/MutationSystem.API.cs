// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using Content.Goobstation.Shared.Genetics.Components;
using Content.Goobstation.Shared.Genetics.Prototypes;
using Content.Goobstation.Shared.Genetics.Types;

namespace Content.Goobstation.Shared.Genetics.Systems;

public sealed partial class MutationSystem
{
    public bool TryAddMutation(EntityUid? uid,
    MutationPrototype proto, bool predicted = true)
    {
        if (!_mutableQuery.TryComp(uid, out var comp))
            return false;

        return AddMutation((uid.Value, comp), proto, predicted);
    }

    public bool EnsureMutation(Entity<MutableComponent> ent,
    MutationPrototype proto, bool predicted = true)
    {
        if (HasMutation(ent, proto))
            return false;

        return TryAddMutation(ent, proto, predicted);
    }

    public bool RemoveMutation(Entity<MutableComponent> ent,
    MutationPrototype proto, bool predicted = true)
    {
        return RemMutation(ent, proto, predicted);
    }

    public bool HasMutation(EntityUid? uid, MutationPrototype proto)
    {
        if (!_mutableQuery.TryComp(uid, out var comp))
            return false;

        return comp.ActiveMutations.Contains(proto);
    }

    public MutationStatus GetMutationStatus(EntityUid? uid, MutationPrototype proto)
    {
        var active = false;
        var hasDormant = false;

        if (_mutableQuery.TryComp(uid, out var comp))
        {
            active = comp.ActiveMutations.Contains(proto);
            hasDormant = comp.InitialMutations.Contains(proto);
        }

        return new MutationStatus(
            active,
            hasDormant
        );
    }

    /*public bool TryGetActiveMutationData(EntityUid? uid,
    MutationPrototype proto, [NotNullWhen(true)] out ActiveMutationData? data)
    {
        data = null;
        if (!_mutableQuery.TryComp(uid, out var comp))
            return false;

        data = comp.ActiveMutations.FirstOrNull(x => x.MutationId == proto);
        return data != null;
    }*/
}
