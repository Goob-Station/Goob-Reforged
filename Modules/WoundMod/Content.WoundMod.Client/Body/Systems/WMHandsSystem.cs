using Content.Shared.Body.Part;
using Content.Shared.Hands.Components;
using Content.WoundMod.Shared.Body.Events;
using Content.WoundMod.Shared.Body.Systems;
using Robust.Client.GameObjects;

namespace Content.WoundMod.Client.Body.Systems;

/// <summary>
/// This handles hiding hands on destruction.
/// </summary>
public sealed class WMHandsSystem : SharedWMHandsSystem
{
    [Dependency] private readonly SpriteSystem _sprite = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<HandsComponent, BodyPartRemovedEvent>(HandleBodyPartRemoved);
        SubscribeLocalEvent<HandsComponent, BodyPartDisabledEvent>(HandleBodyPartDisabled);
    }

    #region Event Handling
    private void HandleBodyPartRemoved(Entity<HandsComponent> ent, ref BodyPartRemovedEvent args) => HideLayers(ent, args.Part);
    private void HandleBodyPartDisabled(Entity<HandsComponent> ent, ref BodyPartDisabledEvent args) => HideLayers(ent, args.Part);
    #endregion
    #region Layer Management
    private void HideLayers(Entity<HandsComponent> ent, Entity<BodyPartComponent> part, SpriteComponent? sprite = null)
    {
        if (part.Comp.PartType != BodyPartType.Hand || !Resolve(ent, ref sprite, logMissing: false))
            return;

        var location = part.Comp.Symmetry switch
        {
            BodyPartSymmetry.None => HandLocation.Middle,
            BodyPartSymmetry.Left => HandLocation.Left,
            BodyPartSymmetry.Right => HandLocation.Right,
            _ => throw new ArgumentOutOfRangeException(nameof(part.Comp.Symmetry)),
        };

        var copiedLayers = ent.Comp.RevealedLayers;
        if (!copiedLayers.TryGetValue(location, out var revealedLayers))
            return;
        foreach (var key in revealedLayers)
            _sprite.RemoveLayer(ent.Owner,key);

        revealedLayers.Clear();
    }
    #endregion
}
