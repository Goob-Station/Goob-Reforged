// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using Content.Goobstation.Shared.Genetics.Types;
using Content.Shared.StatusEffectNew.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;

namespace Content.Goobstation.Shared.Genetics.Prototypes;

[Prototype]
public sealed partial class MutationPrototype : IPrototype, IInheritingPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// <inheritdoc/>
    [ParentDataField(typeof(AbstractPrototypeIdArraySerializer<MutationPrototype>))]
    public string[]? Parents { get; private set; }

    /// <inheritdoc/>
    [NeverPushInheritance]
    [AbstractDataField]
    public bool Abstract { get; private set; }

    [DataField]
    public string? Name;

    /// <summary>
    /// Description about the mutation.
    /// </summary>
    [DataField]
    public string? Description;

    /// <summary>
    /// Instability added to the mutated entity while this mutation is present.
    /// </summary>
    [DataField]
    public MutationInstability Instability = MutationInstability.NegativeNeutral;

    /// <summary>
    /// Larp 'Quality' of this mutation
    /// </summary>
    [DataField]
    public MutationQuality Quality = MutationQuality.MinorNegative;

    /// <summary>
    /// Flavor text when you gain this mutation.
    /// </summary>
    [DataField]
    public string? FlavorGain;

    /// <summary>
    /// Flavor text when you lose this mutation.
    /// </summary>
    [DataField]
    public string? FlavorLoss;

    /// <summary>
    /// Medical examination text.
    /// </summary>
    [DataField]
    public string? MedicalExamination;

    [DataField]
    [AlwaysPushInheritance]
    public List<EntProtoId<StatusEffectComponent>>? StatusEffects;

    [DataField]
    [AlwaysPushInheritance]
    public List<ProtoId<MutationPrototype>>? MutuallyExclusive;
}
