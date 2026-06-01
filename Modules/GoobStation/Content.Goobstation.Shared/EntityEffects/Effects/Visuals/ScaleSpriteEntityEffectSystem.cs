using Content.Shared.EntityEffects;
using Content.Shared.Sprite;

namespace Content.Goobstation.Shared.EntityEffects.Effects.Visuals;

public sealed partial class ScaleSpriteEntityEffectSystem : EntityEffectSystem<TransformComponent, ScaleSprite>
{
    [Dependency] private SharedScaleVisualsSystem _scale = default!;

    protected override void Effect(Entity<TransformComponent> entity, ref EntityEffectEvent<ScaleSprite> args)
    {
        _scale.SetSpriteScale(entity.Owner, _scale.GetSpriteScale(entity.Owner) * args.Effect.Scale);
    }
}
