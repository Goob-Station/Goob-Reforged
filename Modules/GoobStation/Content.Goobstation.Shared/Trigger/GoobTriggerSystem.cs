using System.Numerics;
using Content.Goobstation.Shared.Trigger.Components.Conditions;
using Content.Goobstation.Shared.Trigger.Components.Counter;
using Content.Goobstation.Shared.Trigger.Components.Effects;
using Content.Shared.Maps;
using Content.Shared.Random.Helpers;
using Content.Shared.Trigger;
using Content.Shared.Trigger.Systems;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Goobstation.Shared.Trigger;

public sealed partial class GoobTriggerSystem : EntitySystem
{
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private ITileDefinitionManager _tiledef = default!;
    [Dependency] private TileSystem _tile = default!;
    [Dependency] private TurfSystem _turf = default!;
    [Dependency] private TriggerSystem _trigger = default!;
    [Dependency] private SharedMapSystem _map = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<TriggerCounterComponent, TriggerEvent>(OnTriggerCounter);
        SubscribeLocalEvent<TileReplaceOnTriggerComponent, TriggerEvent>(OnTriggerTileReplace);
        SubscribeLocalEvent<TriggerOnTriggerComponent, TriggerEvent>(OnTriggerTrigger);
        SubscribeLocalEvent<TriggerCounterLimitComponent, AttemptTriggerEvent>(OnTriggerLimitCounter);
    }

    private void OnTriggerCounter(Entity<TriggerCounterComponent> ent, ref TriggerEvent args)
    {
        if (!_timing.IsFirstTimePredicted
            || args.Key != null
            && !ent.Comp.KeysIn.Contains(args.Key))
            return;

        foreach (var key in ent.Comp.CountKeys)
        {
            ent.Comp.Counts.TryAdd(key, 0);
            ent.Comp.Counts[key] += 1;
        }
    }

    private void OnTriggerTileReplace(Entity<TileReplaceOnTriggerComponent> ent, ref TriggerEvent args)
    {
        var tgtPos = Transform(ent.Owner);
        if (tgtPos.GridUid is not { } gridUid || !TryComp(gridUid, out MapGridComponent? mapGrid))
            return;
        var radius = ent.Comp.Radius;

        if (args.Key != null
            && ent.Comp.RadiusCounterScaling
            && TryComp(ent.Owner, out TriggerCounterComponent? counter)
            && counter.Counts.TryGetValue(args.Key, out var count))
            radius *= count * ent.Comp.RadiusCounterModifier;

        var box = new Box2(
            tgtPos.Coordinates.Position + new Vector2(-radius, -radius),
            tgtPos.Coordinates.Position + new Vector2(radius, radius));

        var tileEnumerator = _map.GetLocalTilesEnumerator(gridUid, mapGrid, box);
        var convertTile = (ContentTileDefinition) _tiledef[ent.Comp.Tile];

        while (tileEnumerator.MoveNext(out var tile))
        {
            var random = SharedRandomExtensions.PredictedRandom(
                _timing,
                new NetEntity(SharedRandomExtensions.HashCodeCombine(tile.X, tile.Y, tile.Tile.TypeId)));

            if (tile.Tile.TypeId == convertTile.TileId
                || _turf.GetContentTileDefinition(tile).Name == convertTile.Name
                || !random.Prob(ent.Comp.Prob))
                continue;

            _tile.ReplaceTile(tile, convertTile);
            _tile.PickVariant(convertTile, random);
        }
    }

    private void OnTriggerLimitCounter(Entity<TriggerCounterLimitComponent> ent, ref AttemptTriggerEvent args)
    {
        if (args.Key != null && !ent.Comp.MaxCounts.ContainsKey(args.Key))
            return;

        if (!TryComp(ent.Owner, out TriggerCounterComponent? comp))
            return;

        var key = args.Key ?? TriggerSystem.DefaultTriggerKey;
        if (comp.Counts[key] >= ent.Comp.MaxCounts[key])
            args.Cancelled = true;
    }

    private void OnTriggerTrigger(Entity<TriggerOnTriggerComponent> ent, ref TriggerEvent args)
    {
        if (ent.Comp.KeysOut.Contains(ent.Comp.KeyIn))
        {
            Log.Error($"{nameof(TriggerOnTriggerComponent)} on entity {ToPrettyString(ent.Owner)} had referenced its {nameof(TriggerOnTriggerComponent.KeyIn)} in the {nameof(TriggerOnTriggerComponent.KeysOut)} field, causing a self-reference!");
            ent.Comp.KeysOut.Remove(ent.Comp.KeyIn);
            return;
        }

        if (args.Key != ent.Comp.KeyIn)
            return;

        foreach (var key in ent.Comp.KeysOut)
        {
            _trigger.Trigger(ent.Owner, args.User, key, args.Predicted);
        }
    }
}
