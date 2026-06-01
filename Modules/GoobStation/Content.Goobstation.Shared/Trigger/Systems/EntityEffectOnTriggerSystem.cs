using Content.Goobstation.Shared.Trigger.Components.Effects;
using Content.Shared.EntityEffects;
using Content.Shared.Teleportation.Components;
using Content.Shared.Trigger;

namespace Content.Goobstation.Shared.Trigger.Systems;

public sealed partial class LinkedEntityEffectOnTriggerSystem : XOnTriggerSystem<LinkedEntityEffectOnTriggerComponent>
{
    [Dependency] private SharedEntityEffectsSystem _effects = default!;

    protected override void OnTrigger(Entity<LinkedEntityEffectOnTriggerComponent> ent, EntityUid target, ref TriggerEvent args)
    {
        if (!TryComp(ent.Owner, out LinkedEntityComponent? linkedEntity))
            return;

        foreach (var entity in linkedEntity.LinkedEntities)
        {
            if (TerminatingOrDeleted(entity) || !Exists(entity))
                continue;

            _effects.ApplyEffects(entity, ent.Comp.Effects, ent.Comp.Scale);
        }

        args.Handled = true;
    }
}
