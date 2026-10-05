// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using Content.Goobstation.Shared.Genetics.Components;
using Content.Goobstation.Shared.Genetics.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Goobstation.Shared.Genetics.Events;

/// <summary>
/// Before after a mutation has been added to a mutable entity.
/// </summary>
/// <param name="Target"></param>
/// <param name="MutationId"></param>
[ByRefEvent]
public record struct MutationAddingEvent(Entity<MutableComponent> Target,
    ProtoId<MutationPrototype> MutationId, bool Predicted, bool Cancelled = false);

/// <summary>
/// Raised after a mutation has been added to a mutable entity.
/// </summary>
/// <param name="Target"></param>
/// <param name="MutationId"></param>
[ByRefEvent]
public record struct MutationAddedEvent(Entity<MutableComponent> Target,
    ProtoId<MutationPrototype> MutationId, bool Predicted);

/// <summary>
/// Raised before a mutation is removed from a mutable entity.
/// </summary>
/// <param name="Target"></param>
/// <param name="MutationId"></param>
[ByRefEvent]
public record struct MutationRemovingEvent(Entity<MutableComponent> Target,
    ProtoId<MutationPrototype> MutationId, bool Predicted, bool Cancelled = false);

/// <summary>
/// Raised after a mutation has been removed from a mutable entity.
/// </summary>
/// <param name="Target"></param>
/// <param name="Mutation"></param>
[ByRefEvent]
public record struct MutationRemovedEvent(Entity<MutableComponent> Target,
    ProtoId<MutationPrototype> MutationId, bool Predicted);
