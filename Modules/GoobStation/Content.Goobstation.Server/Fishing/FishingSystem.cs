using System.Linq;
using System.Numerics;
using Content.Goobstation.Shared.Fishing.Components;
using Content.Goobstation.Shared.Fishing.Systems;
using Content.Shared.EntityTable;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Movement.Pulling.Components;
using Robust.Server.GameObjects;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Events;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Goobstation.Server.Fishing;

public sealed partial class FishingSystem : SharedFishingSystem
{
    [Dependency] private IComponentFactory _compFactory = default!;
    [Dependency] private IPrototypeManager _proto = default!;
    [Dependency] private IRobustRandom _random = default!;
    [Dependency] private PhysicsSystem _physics = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<FishingLureComponent, StartCollideEvent>(OnFloatCollide);
        SubscribeLocalEvent<FishingRodComponent, UseInHandEvent>(OnFishingInteract);
    }

    // Apparently StartCollideEvent works junk-y on clientside, so we can't really predict fishing collisions.
    private void OnFloatCollide(Entity<FishingLureComponent> ent, ref StartCollideEvent args)
    {
        var attachedEnt = args.OtherEntity;

        if (HasComp<ActiveFishingSpotComponent>(attachedEnt))
            return;

        if (!FishSpotQuery.TryComp(attachedEnt, out var spotComp))
        {
            if (args.OtherBody.BodyType == BodyType.Static)
                return;

            Anchor(ent, attachedEnt);
            return;
        }

        Anchor(ent, attachedEnt);

        // TODO Currently doesn't support multiple loots
        var fish = spotComp.FishList.GetSpawns(_random.GetRandom(), EntityManager, _proto, new EntityTableContext()).First();

        // Fsh difficulty
        _proto.Index(fish).TryGetComponent(out FishComponent? fishComp, _compFactory);

        // Assign things that depend on the fish
        var activeFishSpot = EnsureComp<ActiveFishingSpotComponent>(attachedEnt);
        activeFishSpot.Fish = fish;
        activeFishSpot.FishDifficulty = fishComp?.FishDifficulty ?? FishComponent.DefaultDifficulty;

        // Assign things that depend on the spot
        var time = spotComp.FishDefaultTimer + _random.NextFloat(-spotComp.FishTimerVariety, spotComp.FishTimerVariety);
        activeFishSpot.FishingStartTime = Timing.CurTime + TimeSpan.FromSeconds(time);
        activeFishSpot.AttachedFishingLure = ent;

        Dirty(attachedEnt, activeFishSpot);
    }

    // UseInHands event sometimes gets declined on Server side, and it desyncs, so we can't predict that
    private void OnFishingInteract(EntityUid uid, FishingRodComponent component, UseInHandEvent args)
    {
        if (!FisherQuery.TryComp(args.User, out var fisherComp) || fisherComp.TotalProgress == null || args.Handled || !Timing.IsFirstTimePredicted)
            return;

        fisherComp.TotalProgress += fisherComp.ProgressPerUse * component.Efficiency;
        Dirty(args.User, fisherComp); // That's a bit evil, but we want to keep numbers real.

        args.Handled = true;
    }

    private void Anchor(Entity<FishingLureComponent> ent, EntityUid attachedEnt)
    {
        var spotPosition = Xform.GetWorldPosition(attachedEnt);
        Xform.SetWorldPosition(ent, spotPosition);
        Xform.SetParent(ent, attachedEnt);
        _physics.SetLinearVelocity(ent, Vector2.Zero);
        _physics.SetAngularVelocity(ent, 0f);
        ent.Comp.AttachedEntity = attachedEnt;
        RemComp<ItemComponent>(ent);
        RemComp<PullableComponent>(ent);
        DirtyField(ent.Owner, ent.Comp, nameof(FishingLureComponent.AttachedEntity));
    }

    protected override void ThrowFishReward(EntProtoId fishId, EntityUid fishSpot, EntityUid target)
    {
        var position = Transform(fishSpot).Coordinates;
        var fish = Spawn(fishId, position);
        // Throw da fish back to the player because it looks funny
        var direction = Xform.GetWorldPosition(target) - Xform.GetWorldPosition(fish);
        var length = direction.Length();
        var distance = Math.Clamp(length, 0.5f, 15f);
        direction *= distance / length;

        Throwing.TryThrow(fish, direction, 7f);
    }

    protected override void CalculateFightingTimings(Entity<ActiveFisherComponent> fisher, ActiveFishingSpotComponent activeSpotComp)
    {
        if (Timing.CurTime < fisher.Comp.NextStruggle)
            return;

        fisher.Comp.NextStruggle = Timing.CurTime + TimeSpan.FromSeconds(_random.NextFloat(0.06f, 0.18f));
        fisher.Comp.TotalProgress -= activeSpotComp.FishDifficulty;
        DirtyFields(fisher.Owner, fisher.Comp, null, nameof(ActiveFisherComponent.NextStruggle), nameof(ActiveFisherComponent.TotalProgress));
    }
}
