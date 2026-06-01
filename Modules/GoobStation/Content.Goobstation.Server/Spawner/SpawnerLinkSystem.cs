using Content.Goobstation.Common.Spawner;
using Content.Goobstation.Shared.Spawner;
using Content.Server.Ghost.Roles.Events;
using Content.Shared.Teleportation.Components;
using Content.Shared.Teleportation.Systems;

namespace Content.Goobstation.Server.Spawner;

public sealed partial class SpawnerLinkSystem : EntitySystem
{
    [Dependency] private LinkedEntitySystem _link = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SpawnerLinkComponent, SpawnerActivationEvent>(OnSpawnerActivation);
        SubscribeLocalEvent<LinkedEntityTransferComponent, SpawnerActivationEvent>(OnLinkTransferActivation);
        SubscribeLocalEvent<LinkedEntityTransferComponent, GhostRoleSpawnerUsedEvent>(OnGhostRoleSpawnerUsed);
    }

    private void OnSpawnerActivation(Entity<SpawnerLinkComponent> ent, ref SpawnerActivationEvent args)
    {
        _link.TryLink(ent.Owner, args.Spawned);
    }

    private void OnLinkTransferActivation(Entity<LinkedEntityTransferComponent> ent, ref SpawnerActivationEvent args)
        => TransferLinks(ent.Owner, args.Spawned);

    private void OnGhostRoleSpawnerUsed(Entity<LinkedEntityTransferComponent> ent, ref GhostRoleSpawnerUsedEvent args)
        => TransferLinks(ent.Owner, args.Spawned);

    private void TransferLinks(Entity<LinkedEntityComponent?> from, EntityUid to)
    {
        if (!Resolve(from.Owner, ref from.Comp))
            return;

        foreach (var linked in from.Comp.LinkedEntities)
        {
            _link.TryLink(to, linked);
        }
    }
}
