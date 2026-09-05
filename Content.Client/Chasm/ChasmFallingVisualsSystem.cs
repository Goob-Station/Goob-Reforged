using Content.Shared.Chasm.Components;
using Content.Shared.Chasm.Events;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.Animations;

namespace Content.Client.Chasm;

/// <summary>
/// Handles the falling animation for entities that fall into an entity with <see cref="ChasmComponent"/>.
/// </summary>
public sealed partial class ChasmFallingVisualsSystem : EntitySystem
{
    [Dependency] private AnimationPlayerSystem _anim = default!;
    [Dependency] private SpriteSystem _sprite = default!;

    [Dependency] private EntityQuery<AnimationPlayerComponent> _animationPlayerQuery;
    [Dependency] private EntityQuery<SpriteComponent> _spriteQuery;

    private const string ChasmFallAnimationKey = "chasm_fall";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ChasmFallingComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<ChasmFallingVisualsComponent, StartedFallingIntoChasmEvent>(OnStartFalling); // Goob
        SubscribeLocalEvent<ChasmFallingVisualsComponent, ResetChasmVisualsEvent>(OnResetVisuals); // Goob
    }

    // todo marty ask rouden what the fuck was going on here and on upstream merging their shit remove these comments
    // dlso this is why we dont early merge OPEN prs

    private void OnComponentInit(Entity<ChasmFallingComponent> entity, ref ComponentInit args)
    {
        // Goobstation Start
        var visuals = EnsureComp<ChasmFallingVisualsComponent>(entity.Owner);
        visuals.AnimationTime = entity.Comp.AnimationTime;
        // Goobstation End
    }

    // Goobstation Start
    private void OnStartFalling(Entity<ChasmFallingVisualsComponent> entity, ref StartedFallingIntoChasmEvent args)
    {
        if (!_spriteQuery.TryComp(entity, out var sprite) ||
            TerminatingOrDeleted(entity))
        {
            return;
        }

        entity.Comp.OriginalScale = sprite.Scale;

        if (!_animationPlayerQuery.TryComp(entity, out var player) ||
            _anim.HasRunningAnimation(player, ChasmFallAnimationKey))
        {
            return;
        }

        _anim.Play((entity, player), GetFallingAnimation(entity.Comp), ChasmFallAnimationKey);
    }
    // Goobstation End

    // Goobstation Start
    private void OnResetVisuals(Entity<ChasmFallingVisualsComponent> entity, ref ResetChasmVisualsEvent args)
    {
        if (!_spriteQuery.TryComp(entity, out var sprite))
        {
            return;
        }

        if (entity.Comp.OriginalScale != null)
            _sprite.SetScale((entity, sprite), entity.Comp.OriginalScale.Value);

        if (!_animationPlayerQuery.TryComp(entity, out var player) ||
            !_anim.HasRunningAnimation(player, ChasmFallAnimationKey))
        {
            return;
        }

        _anim.Stop((entity, player), ChasmFallAnimationKey);
    }
    // Goobstation End

    private static Animation GetFallingAnimation(ChasmFallingVisualsComponent component) // Goob
    {
        return new Animation
        {
            Length = component.AnimationTime,
            AnimationTracks =
            {
                new AnimationTrackComponentProperty
                {
                    ComponentType = typeof(SpriteComponent),
                    Property = nameof(SpriteComponent.Scale),
                    KeyFrames =
                    {
                        new AnimationTrackProperty.KeyFrame(component.OriginalScale!, 0.0f), // Goob
                        new AnimationTrackProperty.KeyFrame(component.AnimationScale, component.AnimationTime.Seconds),
                    },
                    InterpolationMode = AnimationInterpolationMode.Cubic,
                },
            },
        };
    }
}
