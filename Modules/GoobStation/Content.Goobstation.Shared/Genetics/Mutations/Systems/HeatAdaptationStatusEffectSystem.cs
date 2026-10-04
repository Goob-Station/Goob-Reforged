using Content.Shared.Chat;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Popups;
using Content.Shared.StatusEffectNew;
using Content.Goobstation.Common.Temperature;
using Content.Goobstation.Shared.Genetics.Mutations.Components;

namespace Content.Goobstation.Shared.Genetics.Mutations.Systems;

public sealed partial class HeatAdaptationStatusEffectSystem : EntitySystem
{
    [SubscribeLocalEvent]
    private void OnHeatOverride(Entity<HeatAdaptationStatusEffectComponent> ent,
        ref StatusEffectRelayedEvent<TemperatureDamageThresholdOverrideEvent> args)
    {
        // cant modify directly ig?? stupid but ok
        var evArgs = args.Args;

        if (ent.Comp.ColdDamageThreshold is { } c)
            evArgs.ColdDamageThreshold = c;
        if (ent.Comp.HeatDamageThreshold is { } h)
            evArgs.HeatDamageThreshold = h;

        args.Args = evArgs;
    }
}
