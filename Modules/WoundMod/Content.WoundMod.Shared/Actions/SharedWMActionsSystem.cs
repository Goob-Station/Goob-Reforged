using Content.Shared.Actions.Components;

namespace Content.WoundMod.Shared.Actions;

public sealed class SharedWMActionsSystem : EntitySystem
{
    public EntityUid[] HideActions(Entity<ActionsComponent?> ent)
    {
        if (!Resolve(ent, ref ent.Comp))
            return [];

        var actions = ent.Comp.Actions.ToArray();
        ent.Comp.Actions.Clear();
        Dirty(ent);
        return actions;
    }

    public void UnHideActions(EntityUid performer, EntityUid[] actions, ActionsComponent? comp = null)
    {
        if (!Resolve(performer, ref comp))
            return;

        foreach (var action in actions)
            comp.Actions.Add(action);
        Dirty(performer, comp);
    }
}
