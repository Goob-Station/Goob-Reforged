using Content.Shared.Whitelist;
using Robust.Shared.GameStates;

namespace Content.Goobstation.Shared.StepTrigger.Components;

/// <summary>
/// Component that makes a step trigger ignore some entities.
/// </summary>
/// <remarks>
/// Usually you want to use <see cref="StepTriggerImmuneComponent"/> on the stepper instead of this component.
/// </remarks>
[RegisterComponent, NetworkedComponent]
[Access(typeof(GoobStepTriggerSystem))]
public sealed partial class StepTriggerExceptComponent : Component
{
    [DataField]
    public EntityWhitelist? Whitelist;

    [DataField]
    public EntityWhitelist? Blacklist;
}
