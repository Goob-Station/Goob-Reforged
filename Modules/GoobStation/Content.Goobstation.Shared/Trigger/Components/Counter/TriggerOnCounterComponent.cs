using Content.Shared.Destructible.Thresholds;
using Content.Shared.Trigger.Components.Triggers;
using Robust.Shared.GameStates;

namespace Content.Goobstation.Shared.Trigger.Components.Counter;

/// <summary>
/// Triggers a key when the trigger count reaches a certain range.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class TriggerOnCounterComponent : BaseTriggerOnXComponent
{
    /// <summary>
    /// Amount of triggers per each trigger key.
    /// </summary>
    [DataField(required: true), AutoNetworkedField]
    public Dictionary<string, MinMax> Ranges;
}
