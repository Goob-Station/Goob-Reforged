using Robust.Shared.GameStates;

namespace Content.Goobstation.Shared.Ghost;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class GhostColorComponent : Component
{
    [DataField, AutoNetworkedField]
    public Color? Color;
}
