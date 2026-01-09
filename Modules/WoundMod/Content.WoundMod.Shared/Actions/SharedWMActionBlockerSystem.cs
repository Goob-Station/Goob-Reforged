using Content.Shared.Interaction.Events;

namespace Content.WoundMod.Shared.Actions;

public sealed class SharedWMActionBlockerSystem : EntitySystem
{
    public bool CanInstrumentInteract(EntityUid user, EntityUid used, EntityUid? target)
    {
        var ev = new InteractionAttemptEvent(user, target);
        RaiseLocalEvent(used, ref ev);

        return !ev.Cancelled;
    }
}
