using Content.Goobstation.Client.Fishing.Overlays;
using Content.Goobstation.Shared.Fishing.Systems;
using Robust.Client.Graphics;
using Robust.Client.Player;

namespace Content.Goobstation.Client.Fishing;

public sealed class FishingSystem : SharedFishingSystem
{
    [Dependency] private readonly IOverlayManager _overlay = default!;
    [Dependency] private readonly IPlayerManager _player = default!;

    public override void Initialize()
    {
        base.Initialize();
        _overlay.AddOverlay(new FishingOverlay(EntityManager, _player));
    }

    public override void Shutdown()
    {
        base.Shutdown();
        _overlay.RemoveOverlay<FishingOverlay>();
    }
}
