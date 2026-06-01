using Content.Goobstation.Shared.Trigger.Components.Counter;
using Content.Goobstation.Shared.Trigger.Components.Effects;
using Content.Shared.Trigger;

namespace Content.Goobstation.Shared.Trigger.Systems;

public sealed partial class ResetCounterOnTriggerSystem : XOnTriggerSystem<ResetCounterOnTriggerComponent>
{
    protected override void OnTrigger(Entity<ResetCounterOnTriggerComponent> ent, EntityUid target, ref TriggerEvent args)
    {
        if (!TryComp(ent.Owner, out TriggerCounterComponent? counter))
            return;

        // Reset everything if it's a general key.
        if (args.Key == null)
        {
            foreach (var key in counter.CountKeys)
            {
                counter.Counts[key] = 0;
            }
            return;
        }

        foreach (var (key, reset) in ent.Comp.Reset)
        {
            if (key != args.Key)
                continue;

            foreach (var keyValue in reset)
            {
                if (!counter.Counts.ContainsKey(keyValue))
                    continue;

                counter.Counts[keyValue] = 0;
            }
        }
    }
}
