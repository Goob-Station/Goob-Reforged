using Content.Shared.EntityEffects;
using Content.Shared.Sprite;
using Robust.Shared.Network;

namespace Content.Goobstation.Shared.EntityEffects.Effects.Visuals;

public sealed partial class ScaleSpriteEntityEffectSystem : EntityEffectSystem<MetaDataComponent, ScaleSprite>
{
    [Dependency] private SharedScaleVisualsSystem _scale = default!;
    [Dependency] private INetManager _net = default!;

    protected override void Effect(Entity<MetaDataComponent> entity, ref EntityEffectEvent<ScaleSprite> args)
    {
        if (!_net.IsServer)
            return; // It breaks in prediction

        // TODO add proper scale scaling. I mean... the EntityEffects scale scaling the vector scale. You know.
        _scale.SetSpriteScale(entity.Owner, _scale.GetSpriteScale(entity.Owner) * args.Effect.Scale);
    }
}
