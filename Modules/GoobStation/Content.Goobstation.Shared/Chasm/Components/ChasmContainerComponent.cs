using Robust.Shared.Containers;

namespace Content.Goobstation.Shared.Chasm.Components;

/// <summary>
/// Makes this chasm store entities when they fall inside.
/// </summary>
[RegisterComponent]
public sealed partial class ChasmContainerComponent : Component
{
    /// <summary>
    /// Contained entities of this chasm.
    /// </summary>
    [ViewVariables]
    public Container Container = default!;

    /// <summary>
    /// If true, stuns the mobs that fall inside, so they can't do anything themselves.
    /// </summary>
    [DataField]
    public bool DoStun = true;
}
