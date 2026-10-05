// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using Robust.Shared.Prototypes;

namespace Content.Goobstation.Shared.Genetics.Prototypes;

[Prototype]
public sealed partial class MutationRecipePrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// <summary>
    /// Result of the recipe.
    /// </summary>
    [DataField(required: true)]
    public ProtoId<MutationPrototype> Result { get; private set; }

    /// <summary>
    /// Mutation prototypes part of the recipe.
    /// </summary>
    [DataField(required: true)]
    public List<ProtoId<MutationPrototype>> Required { get; private set; } = [];
}
