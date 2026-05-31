using Content.Goobstation.Shared.StepTrigger.Components;
using Content.Shared.StepTrigger.Systems;
using Content.Shared.Whitelist;

namespace Content.Goobstation.Shared.StepTrigger;

public sealed partial class GoobStepTriggerSystem : EntitySystem
{
    [Dependency] private EntityWhitelistSystem _whitelist = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<StepTriggerImmuneComponent, StepTriggerAttemptEvent>(OnImmuneTrigger);
        SubscribeLocalEvent<StepTriggerExceptComponent, StepTriggerAttemptEvent>(OnExceptTrigger);
    }

    private void OnImmuneTrigger(Entity<StepTriggerImmuneComponent> ent, ref StepTriggerAttemptEvent args)
    {
        if (_whitelist.CheckBoth(args.Tripper, ent.Comp.Blacklist, ent.Comp.Whitelist))
            args.Cancelled = true;
    }

    private void OnExceptTrigger(Entity<StepTriggerExceptComponent> ent, ref StepTriggerAttemptEvent args)
    {
        if (_whitelist.CheckBoth(args.Tripper, ent.Comp.Blacklist, ent.Comp.Whitelist))
            args.Cancelled = true;
    }
}
