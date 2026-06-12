using Content.Server.Atmos.Reactions;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;

namespace Content.Goobstation.Server.Atmos;

/// <summary>
/// Gas chamber that allows only specific gas reactions inside it
/// </summary>
[RegisterComponent]
public sealed partial class GasReactionChamberComponent : Component
{
    /// <summary>
    /// List of allowed reactions. Reactions are not allowed if empty.
    /// </summary>
    [DataField(customTypeSerializer: typeof(PrototypeIdHashSetSerializer<GasReactionPrototype>))]
    public HashSet<string>? Reactions = null;
}
