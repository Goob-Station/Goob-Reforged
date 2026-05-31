using Content.Shared.Whitelist;
using Robust.Shared.GameStates;

namespace Content.Goobstation.Shared.StepTrigger.Components;

/// <summary>
/// Component that makes an entity immune to some step triggers.
/// </summary>
[RegisterComponent, NetworkedComponent]
[Access(typeof(GoobStepTriggerSystem))]
public sealed partial class StepTriggerImmuneComponent : Component
{
    [DataField]
    public EntityWhitelist? Whitelist;

    [DataField]
    public EntityWhitelist? Blacklist;
}
