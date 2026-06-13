using Robust.Shared.GameStates;

namespace Content.Goobstation.Shared.Alert.Components;

/// <summary>
/// Generic component for alerts that have needs to update when some value in some component changes.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ValueRelatedAlertComponent : Component
{
    /// <summary>
    /// Amount of levels in ViewSprite
    /// </summary>
    [DataField]
    public short Levels = 0;

    /// <summary>
    /// Used to select sprite states name. Basically, system looks for states like "0.png"
    /// </summary>
    [DataField]
    public string IconPrefix = "";
}
