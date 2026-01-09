namespace Content.WoundMod.Shared.Body.Components;

/// <summary>
/// This is used to sidecar to the regular brain component.
/// </summary>
[RegisterComponent]
public sealed partial class WMBrainComponent : Component
{
    // Is the brain controlling the entity?
    [DataField]
    public bool Active = true;
}
