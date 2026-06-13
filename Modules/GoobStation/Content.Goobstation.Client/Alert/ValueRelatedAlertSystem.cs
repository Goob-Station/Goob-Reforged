using Content.Client.Alerts;
using Content.Client.UserInterface.Systems.Alerts.Controls;
using Content.Goobstation.Shared.Alert.Components;
using Content.Goobstation.Shared.Alert.Events;
using Content.Shared.Rounding;
using Robust.Client.GameObjects;

namespace Content.Goobstation.Client.Alert.EntitySystems;

public sealed partial class ValueRelatedAlertSystem : EntitySystem
{
    [Dependency] private SpriteSystem _spriteSystem = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<ValueRelatedAlertComponent, UpdateAlertSpriteEvent>(OnAlertSpriteUpdate);
    }

    private void OnAlertSpriteUpdate(Entity<ValueRelatedAlertComponent> alert, ref UpdateAlertSpriteEvent args)
    {
        var sprite = args.SpriteViewEnt;

        var ev = new GetValueRelatedAlertValuesEvent(args.Alert);
        RaiseLocalEvent(args.ViewerEnt, ref ev);

        if (!ev.Handled || ev.MaxValue == null || ev.MaxValue == 0 || ev.CurrentValue == null)
            return;

        var severity = ContentHelpers.RoundToLevels(MathF.Max(0f, ev.CurrentValue.Value), ev.MaxValue.Value, alert.Comp.Levels);

        var rsiString = (string.IsNullOrEmpty(alert.Comp.IconPrefix) ? "" : $"{alert.Comp.IconPrefix}") + $"{severity}";

        _spriteSystem.LayerSetRsiState(sprite.Owner, AlertVisualLayers.Base, rsiString);
    }
}
