using Content.Server.Atmos.Reactions;
using Robust.Shared.Prototypes;

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
    [DataField]
    public HashSet<ProtoId<GasReactionPrototype>> Reactions = new();
}
