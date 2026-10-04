using Robust.Shared.Prototypes;

namespace Content.Goobstation.Shared.Genetics.Prototypes;

[Prototype]
public sealed partial class MutationPoolPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField]
    public List<ProtoId<MutationPrototype>> Mutations { get; private set; } = [];
}
