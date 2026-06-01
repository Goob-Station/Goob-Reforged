using System.Linq;
using Content.Server.Ghost.Roles.Components;
using Content.Server.Ghost.Roles.Events;
using Content.Shared.EntityTable;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Content.Shared.Players;
using Content.Shared.Roles;
using Robust.Shared.Player;
using Robust.Shared.Utility;

namespace Content.Goobstation.Server.Spawner;

public sealed partial class GhostRoleTableSpawnerSystem : EntitySystem
{
    [Dependency] private EntityTableSystem _table = default!;
    [Dependency] private SharedTransformSystem _transform = default!;
    [Dependency] private SharedMindSystem _mindSystem = default!;
    [Dependency] private SharedRoleSystem _roleSystem = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<GhostRoleTableSpawnerComponent, TakeGhostRoleEvent>(OnSpawnerTakeRole);
    }

    private void OnSpawnerTakeRole(Entity<GhostRoleTableSpawnerComponent> ent, ref TakeGhostRoleEvent args)
    {
        var (uid, component) = ent;

        if (!TryComp(uid, out GhostRoleComponent? ghostRole) ||
            !CanTakeGhost(uid, ghostRole))
        {
            args.TookRole = false;
            return;
        }

        var spawns = _table.GetSpawns(ent.Comp.Table).ToList();
        if (spawns.Count == 0)
            return;

        var proto = spawns[0];

        var mob = Spawn(proto, Transform(uid).Coordinates);
        _transform.AttachToGridOrMap(mob);

        var spawnedEvent = new GhostRoleSpawnerUsedEvent(uid, mob);
        RaiseLocalEvent(mob, spawnedEvent);
        RaiseLocalEvent(uid, spawnedEvent); // Goobstation edit

        if (ghostRole.MakeSentient)
            _mindSystem.MakeSentient(mob, ghostRole.AllowMovement, ghostRole.AllowSpeech);

        EnsureComp<MindContainerComponent>(mob);

        GhostRoleInternalCreateMindAndTransfer(args.Player, uid, mob, ghostRole);

        if (++component.CurrentTakeovers < component.AvailableTakeovers)
        {
            args.TookRole = true;
            return;
        }

        ghostRole.Taken = true;

        if (component.DeleteOnSpawn)
            QueueDel(uid);

        args.TookRole = true;
    }

    private bool CanTakeGhost(EntityUid uid, GhostRoleComponent? component = null)
    {
        return Resolve(uid, ref component, false) &&
               !component.Taken &&
               !MetaData(uid).EntityPaused;
    }

    public void GhostRoleInternalCreateMindAndTransfer(ICommonSession player, EntityUid roleUid, EntityUid mob, GhostRoleComponent? role = null)
    {
        if (!Resolve(roleUid, ref role))
            return;

        DebugTools.AssertNotNull(player.ContentData());

        // After taking a ghost role, the player cannot return to the original body, so wipe the player's current mind
        // unless it is a visiting mind
        if(_mindSystem.TryGetMind(player.UserId, out _, out var mind) && !mind.IsVisitingEntity)
            _mindSystem.WipeMind(player);

        var newMind = _mindSystem.CreateMind(player.UserId,
            Comp<MetaDataComponent>(mob).EntityName);

        _mindSystem.SetUserId(newMind, player.UserId);
        _mindSystem.TransferTo(newMind, mob);

        _roleSystem.MindAddRoles(newMind.Owner, role.MindRoles, newMind.Comp);
    }
}
