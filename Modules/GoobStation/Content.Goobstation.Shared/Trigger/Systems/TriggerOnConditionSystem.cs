using Content.Goobstation.Shared.Trigger.Components.Counter;
using Content.Shared.EntityConditions;
using Content.Shared.Trigger;
using Content.Shared.Trigger.Systems;

namespace Content.Goobstation.Shared.Trigger.Systems;

public sealed partial class TriggerOnConditionSystem : XOnTriggerSystem<TriggerOnConditionComponent>
{
    [Dependency] private SharedEntityConditionsSystem _conditions = default!;
    [Dependency] private TriggerSystem _trigger = default!;

    protected override void OnTrigger(Entity<TriggerOnConditionComponent> ent, EntityUid target, ref TriggerEvent args)
    {
        if (args.Key != null && !ent.Comp.KeysIn.Contains(args.Key)
            || !TryComp(ent.Owner, out TriggerCounterComponent? counter))
            return;

        foreach (var (keyOut, conditions) in ent.Comp.Triggers)
        {
            if (!_conditions.TryConditions(target, conditions))
                continue;

            _trigger.Trigger(ent.Owner, args.User, keyOut, args.Predicted);
        }
    }
}
