using Content.Goobstation.Shared.Trigger.Components.Counter;
using Content.Shared.Destructible.Thresholds;
using Content.Shared.EntityConditions;
using Robust.Shared.Prototypes;

namespace Content.Goobstation.Shared.EntityConditions.Conditions;

public sealed partial class TriggerCountConditionSystem : EntityConditionSystem<TriggerCounterComponent, TriggerCountCondition>
{
    protected override void Condition(Entity<TriggerCounterComponent> entity, ref EntityConditionEvent<TriggerCountCondition> args)
    {
        if (entity.Comp.Counts.TryGetValue(args.Condition.Key, out var count)
            && args.Condition.Count.Min <= count
            && args.Condition.Count.Max >= count)
            args.Result = true;
    }
}

public sealed partial class TriggerCountCondition : EntityConditionBase<TriggerCountCondition>
{
    [DataField(required: true)]
    public MinMax Count;

    [DataField]
    public string Key;

    public override string EntityConditionGuidebookText(IPrototypeManager prototype) => "";
}
