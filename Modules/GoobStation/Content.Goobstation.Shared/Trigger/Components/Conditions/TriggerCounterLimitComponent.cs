using Content.Shared.Trigger.Systems;
using Robust.Shared.GameStates;

namespace Content.Goobstation.Shared.Trigger.Components.Conditions;

/// <summary>
/// Allows the trigger to actually activate only when the
/// total amount of triggers is within a certain range.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class TriggerCounterLimitComponent : Component
{
    /// <summary>
    /// Max amount of each trigger key activation.
    /// </summary>
    [DataField, AutoNetworkedField]
    public Dictionary<string, int> MaxCounts = new()
    {
        [TriggerSystem.DefaultTriggerKey] = 1,
    };
}
