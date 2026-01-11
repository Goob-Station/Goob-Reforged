// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: MIT-WIZARDS

using System.Collections.Generic;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Markings;
using Content.WoundMod.Shared.Body.Part;
using Content.WoundMod.Shared.Body.Systems;
using Robust.Client.GameObjects;
using Robust.Shared.Utility;

namespace Content.WoundMod.Client.Body.Systems;

/// <summary>
/// This handles markings mostly for the body
/// </summary>
public sealed class WMBodySystem : SharedWMBodySystem
{
    [Dependency] private readonly MarkingManager _markingManager = default!;
    [Dependency] private readonly SpriteSystem _sprite = default!;

    private void ApplyMarkingToPart(MarkingPrototype markingPrototype,
        IReadOnlyList<Color>? colors,
        bool visible,
        EntityUid sprite)
    {
        for (var j = 0; j < markingPrototype.Sprites.Count; j++)
        {
            var markingSprite = markingPrototype.Sprites[j];

            if (markingSprite is not SpriteSpecifier.Rsi rsi)
                continue;

            var layerId = $"{markingPrototype.ID}-{rsi.RsiState}";

            if (!_sprite.LayerMapTryGet(sprite,layerId, out _, false))
            {
                var layer = _sprite.AddLayer(sprite,markingSprite, j + 1);
                _sprite.LayerMapSet(sprite,layerId, layer);
                _sprite.LayerSetSprite(sprite,layerId, rsi);
            }

            _sprite.LayerSetVisible(sprite,layerId, visible);

            if (!visible)
                continue;

            // Okay so if the marking prototype is modified but we load old marking data this may no longer be valid
            // and we need to check the index is correct. So if that happens just default to white?
            if (colors != null && j < colors.Count)
                _sprite.LayerSetColor(sprite,layerId, colors[j]);
            else
                _sprite.LayerSetColor(sprite,layerId, Color.White);
        }
    }

    protected override void ApplyPartMarkings(EntityUid target, BodyPartAppearanceComponent component)
    {
        if (component.Color != null)
            _sprite.SetColor(target,component.Color.Value);

        foreach (var (_, markingList) in component.Markings)
            foreach (var marking in markingList)
            {
                if (!_markingManager.TryGetMarking(marking, out var markingPrototype))
                    continue;

                ApplyMarkingToPart(markingPrototype, marking.MarkingColors, marking.Visible, target);
            }
    }

    protected override void RemoveBodyMarkings(EntityUid target, BodyPartAppearanceComponent partAppearance, HumanoidAppearanceComponent bodyAppearance)
    {
        return;
    }
}
