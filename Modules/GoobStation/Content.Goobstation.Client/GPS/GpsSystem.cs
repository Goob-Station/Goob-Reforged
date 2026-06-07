using Content.GoobStation.Shared.GPS;
using Content.GoobStation.Shared.GPS.Components;

namespace Content.GoobStation.Client.GPS;

public sealed class GpsSystem : SharedGpsSystem
{
    protected override void UpdateUi(Entity<GPSComponent> ent)
    {
        if (UiSystem.TryGetOpenUi<GpsBoundUserInterface>(ent.Owner,
                GpsUiKey.Key,
                out var bui))
            bui.UpdateWindow();
    }
}
