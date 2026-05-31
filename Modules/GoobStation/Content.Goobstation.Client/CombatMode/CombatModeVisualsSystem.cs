using Content.Shared.CombatMode;

namespace Content.Goobstation.Client.CombatMode;

public sealed partial class CombatModeVisualsSystem : EntitySystem
{
    [Dependency] private SharedAppearanceSystem _appearance = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<CombatModeVisualsComponent, ToggleCombatActionEvent>(OnCombatToggle);
        SubscribeLocalEvent<CombatModeVisualsComponent, ComponentStartup>(OnCombatStartup);
    }

    private void OnCombatToggle(Entity<CombatModeVisualsComponent> ent, ref ToggleCombatActionEvent args)
        => UpdateAppearance(ent.Owner);

    private void OnCombatStartup(Entity<CombatModeVisualsComponent> ent, ref ComponentStartup args)
        => UpdateAppearance(ent.Owner);

    private void UpdateAppearance(EntityUid uid)
    {
        if (!TryComp<CombatModeComponent>(uid, out var combat))
            return;

        _appearance.SetData(uid, CombatModeVisuals.Combat, combat.IsInCombatMode);
    }
}
