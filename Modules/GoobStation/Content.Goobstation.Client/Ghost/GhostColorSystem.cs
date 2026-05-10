using Content.Goobstation.Shared.Ghost;
using Robust.Client.GameObjects;

namespace Content.Goobstation.Client.Ghost;

public sealed partial class GhostColorSystem : EntitySystem
{
    [Dependency] private SpriteSystem _sprite = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<GhostColorComponent, ComponentInit>(OnGhostColorInit);
    }

    private void OnGhostColorInit(Entity<GhostColorComponent> ent, ref ComponentInit args)
    {
        if (ent.Comp.Color != null)
            _sprite.SetColor(ent.Owner, ent.Comp.Color.Value);
    }
}
