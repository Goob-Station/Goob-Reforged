using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Goobstation.Shared.Containers;

/// <summary>
/// Adds components to any entity that is inserted into the container.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ContainerComponentGrantComponent : Component
{
    [DataField(required: true)]
    public ComponentRegistry Components;
}
