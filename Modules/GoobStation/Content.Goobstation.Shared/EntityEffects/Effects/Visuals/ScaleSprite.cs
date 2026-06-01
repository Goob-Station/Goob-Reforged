using System.Numerics;
using Content.Shared.EntityEffects;

namespace Content.Goobstation.Shared.EntityEffects.Effects.Visuals;

public sealed partial class ScaleSprite : EntityEffectBase<ScaleSprite>
{
    [DataField]
    public Vector2 Scale;
}
