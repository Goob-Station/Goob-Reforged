// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using Content.Goobstation.Shared.Genetics.Prototypes;
using Content.Goobstation.Shared.Genetics.Systems;
using Content.Shared.StatusEffectNew.Components;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Goobstation.Shared.Genetics.Components;

[RegisterComponent]
[NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(MutationSystem), typeof(MutationEffectsSystem), typeof(HandheldGeneticsScannerSystem))]
public sealed partial class MutableComponent : Component
{
    /// <summary>
    /// Maximum instability. If this is exceeded, bad things happen!!
    /// </summary>
    [DataField(required: true)]
    public int MaxInstability = 90;

    /// <summary>
    /// Current instability.
    /// </summary>
    [DataField(required: true)]
    public int Instability = 0;

    /// <summary>
    /// Status effect applied when instability threshold passed.
    /// </summary>
    [DataField(required: true)]
    public EntProtoId<StatusEffectComponent> SomethingBadStatusEffect = "StatusEffectDnaMelting";

    /// <summary>
    /// How many dormant mutations this entity will spawn with.
    /// </summary>
    [DataField]
    public int InitialMutationCount = 6;

    [DataField(required: true)]
    public ProtoId<MutationPoolPrototype> InitialMutationsPool = "default";

    [DataField]
    [AutoNetworkedField]
    public List<ProtoId<MutationPrototype>> InitialMutations = [];

    [DataField]
    [AutoNetworkedField]
    public List<ProtoId<MutationPrototype>> ActiveMutations = [];
};
