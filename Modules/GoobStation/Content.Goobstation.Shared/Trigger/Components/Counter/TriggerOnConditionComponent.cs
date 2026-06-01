using Content.Shared.EntityConditions;
using Content.Shared.Trigger.Components.Effects;
using Robust.Shared.GameStates;

namespace Content.Goobstation.Shared.Trigger.Components.Counter;

/// <summary>
/// Triggers a key when the trigger count reaches a certain range.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class TriggerOnConditionComponent : BaseXOnTriggerComponent
{
    /// <summary>
    /// Amount of triggers per each trigger key.
    /// </summary>
    [DataField(required: true)]
    public Dictionary<string, EntityCondition[]> Triggers;
}
