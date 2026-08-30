using Content.Goobstation.Common.Damage;
using Content.Goobstation.Shared.Particles.Systems;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Systems;
using Content.Shared.Destructible;
using Content.Shared.Ghost;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Whitelist;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Physics.Events;
using Robust.Shared.Timing;

namespace Content.Goobstation.Shared.Damage;

public sealed partial class DestroyInteractingSystem : EntitySystem
{
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private SharedAudioSystem _audioSystem = default!;
    [Dependency] private SharedDestructibleSystem _destructibleSystem = default!;
    [Dependency] private SharedHandsSystem _handsSystem = default!;

    [Dependency] private SharedParticleSystem _particleSystem = default!;

    [Dependency] private SharedTransformSystem _transformSystem = default!;
    [Dependency] private EntityWhitelistSystem _whitelistSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DestroyInteractingsComponent, InteractUsingEvent>(OnInteractUsing);
        SubscribeLocalEvent<DestroyInteractingsComponent, InteractHandEvent>(OnInteractHand);
        SubscribeLocalEvent<DestroyInteractingsComponent, StartCollideEvent>(OnCollide);
        SubscribeLocalEvent<DestroyInteractingsComponent, AttackedEvent>(OnAttacked);
    }

    private bool TryDestroyEntity(EntityUid entity,
        Entity<DestroyInteractingsComponent> destructor,
        out EntityUid? spawned,
        EntityUid? user = null)
    {
        spawned = null;
        if (!_whitelistSystem.CheckBoth(entity, destructor.Comp.DestroyBlacklist, destructor.Comp.DestroyWhitelist))
            return false;

        if (HasComp<GodmodeComponent>(entity) || HasComp<GhostComponent>(entity))
            return false;

        var position = _transformSystem.GetMapCoordinates(entity);
        if (!_destructibleSystem.DestroyEntity(entity))
            return false;

        _audioSystem.PlayPredicted(destructor.Comp.DestructionSound, destructor, user);

        if (destructor.Comp.Particle is { } particleId)
            _particleSystem.CreateParticleOnCoordinates(particleId, position);

        if (destructor.Comp.SpawnOnDestruction is { } protoId)
            spawned = PredictedSpawnAtPosition(protoId, _transformSystem.ToCoordinates(position));

        return true;
    }

    private void OnInteractUsing(Entity<DestroyInteractingsComponent> destructor, ref InteractUsingEvent args)
    {
        var target = destructor.Comp.RespectHandInteraction ? args.Used : args.User;
        args.Handled = TryDestroyEntity(target, destructor, out var spawned, args.User);

        if (HasComp<GodmodeComponent>(args.User) || HasComp<GhostComponent>(args.User))
            return;

        if (spawned is not { } spawnedEnt)
            return;

        _handsSystem.TryPickup(args.User, spawnedEnt, animate: false);
    }

    private void OnInteractHand(Entity<DestroyInteractingsComponent> destructor, ref InteractHandEvent args)
    {
        args.Handled = TryDestroyEntity(args.User, destructor, out _, args.User);
    }

    private void OnCollide(Entity<DestroyInteractingsComponent> destructor, ref StartCollideEvent args)
    {
        if (destructor.Comp.RespectContacts
            && destructor.Comp.FixtureId == args.OurFixtureId
            && args.OtherFixture.Hard
            && args.OurFixture.Hard)
            TryDestroyEntity(args.OtherEntity, destructor, out _);
    }

    private void OnAttacked(Entity<DestroyInteractingsComponent> destructor, ref AttackedEvent args)
    {
        TryDestroyEntity(args.Used, destructor, out _, user: args.User);
    }
}