using Content.Shared.Destructible.Thresholds;
using Content.Shared.Trigger.Components.Effects;
using Robust.Shared.GameStates;

namespace Content.Goobstation.Shared.Trigger.Components.Counter;

/// <summary>
/// Triggers a key when the trigger count reaches a certain range.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class TriggerOnCounterComponent : BaseXOnTriggerComponent
{
    /// <summary>
    /// Amount of triggers per each trigger key.
    /// </summary>
    [DataField(required: true)]
    public List<TriggerOnCounterEntry> Counts;
}

[DataRecord]
public partial record struct TriggerOnCounterEntry(string KeyOut, TriggerCounterRangeEntry Count);

[DataRecord]
public partial record struct TriggerCounterRangeEntry(string Key, MinMax Range, bool Reset = false);
