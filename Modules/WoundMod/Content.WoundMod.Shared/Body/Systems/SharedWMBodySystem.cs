using System.Diagnostics.CodeAnalysis;
using Content.Shared.Body.Components;
using Content.Shared.Body.Events;
using Content.Shared.Body.Organ;
using Content.Shared.Body.Part;
using Content.Shared.Body.Systems;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Damage.Systems;
using Content.Shared.Inventory;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Random;
using Content.Shared.Standing;
using Content.WoundMod.Shared.Body.Components;
using Content.WoundMod.Shared.Body.Events;
using Content.WoundMod.Shared.Body.Organ;
using Content.WoundMod.Shared.Body.Part;
using Content.WoundMod.Shared.BodyEffects;
using Robust.Shared.Containers;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.WoundMod.Shared.Body.Systems;

/// <summary>
/// This handles...
/// </summary>
public abstract partial class SharedWMBodySystem : EntitySystem
{
    [Dependency] private readonly SharedBodySystem _body = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly InventorySystem _inventorySystem = default!;
    [Dependency] private readonly RandomHelperSystem _randomHelper = default!;
    [Dependency] private readonly SharedContainerSystem _containers = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly StandingStateSystem _standing = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly ItemSlotsSystem _slots = default!; // Shitmed Change

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<BodyPartComponent, ComponentStartup>(OnBodyPartStartup);

        InitializeIntegrityQueue();
        InitializePartAppearances();
        InitializeOrgans();
        InitializeRelay();
        InitializeParts();
    }

    private void InitializeParts()
    {
        SubscribeLocalEvent<BodyPartComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<BodyPartComponent, ComponentRemove>(OnBodyPartRemove);
        SubscribeLocalEvent<BodyPartComponent, AmputateAttemptEvent>(OnAmputateAttempt);
        SubscribeLocalEvent<BodyPartComponent, BodyPartEnableChangedEvent>(OnPartEnableChanged);
        SubscribeLocalEvent<WMBodyPartComponent, EntInsertedIntoContainerMessage>(OnWMPartInserted);
        SubscribeLocalEvent<WMBodyPartComponent, EntRemovedFromContainerMessage>(OnWMPartRemoved);
    }

    private void InitializeOrgans()
    {
        SubscribeLocalEvent<OrganComponent, ComponentInit>(OnOrganInit);
        SubscribeLocalEvent<OrganComponent, OrganEnableChangedEvent>(OnOrganEnableChanged);
    }

    private void OnOrganInit(Entity<OrganComponent> ent, ref ComponentInit args)
    {
        EnsureComp<WMOrganComponent>(ent);
    }

    private void OnWMPartInserted(Entity<WMBodyPartComponent> ent, ref EntInsertedIntoContainerMessage args)
    {
        if (!TryComp<BodyPartComponent>(ent, out var part))
            return;

        if (TryComp<BodyPartComponent>(args.Container.Owner, out var parentPart)
            && parentPart.Body is { Valid: true } bodyUid)
        {
            foreach (var (organId, _, _) in GetPartOrgans(ent, part))
            {
                var ev = new OrganAddedToBodyEvent(bodyUid, ent);
                RaiseLocalEvent(organId, ref ev);
            }

            CheckBodyPart((ent, part), GetTargetBodyPart(part), false, wmPart: ent.Comp);
        }

        Dirty(ent, ent.Comp);
    }

    private void OnWMPartRemoved(Entity<WMBodyPartComponent> ent, ref EntRemovedFromContainerMessage args)
    {
        if (!TryComp<BodyPartComponent>(ent, out var part))
            return;

        var parentSlot = _body.GetParentPartAndSlotOrNull(ent);
        if (parentSlot == null)
            return;

        if (TryComp<BodyPartComponent>(args.Container.Owner, out var parentPart)
            && parentPart.Body is { Valid: true } oldBodyUid)
        {
            foreach (var (organId, _, _) in GetPartOrgans(ent, part))
            {
                var ev = new OrganRemovedFromBodyEvent(oldBodyUid, ent);
                RaiseLocalEvent(organId, ref ev);
            }

            CheckBodyPart((ent, part), GetTargetBodyPart(part), true, wmPart: ent.Comp);
        }

        Dirty(ent, ent.Comp);
    }

    private void OnBodyPartStartup(Entity<BodyPartComponent> ent, ref ComponentStartup args)
    {
        EnsureComp<WMBodyPartComponent>(ent);
    }

    // move to organs partial
    public bool TrySetOrganUsed(EntityUid organId, bool used, WMBodyPartComponent? organ = null)
    {
        if (!Resolve(organId, ref organ) || organ.Used == used)
            return false;

        organ.Used = used;
        Dirty(organId, organ);
        return true;
    }

    private void OnOrganEnableChanged(Entity<OrganComponent> organEnt, ref OrganEnableChangedEvent args)
    {
        if (!TryComp<WMBodyPartComponent>(organEnt, out var wmPart))
            return;

        if (!wmPart.CanEnable && args.Enabled)
            return;

        wmPart.Enabled = args.Enabled;

        if (args.Enabled)
            EnableOrgan(organEnt);
        else
            DisableOrgan(organEnt);

        if (organEnt.Comp.Body is { Valid: true } bodyEnt)
            RaiseLocalEvent(organEnt, new OrganComponentsModifyEvent(bodyEnt, args.Enabled));

        Dirty(organEnt, organEnt.Comp);
    }

    private void EnableOrgan(Entity<OrganComponent> organEnt)
    {
        if (!TryComp(organEnt.Comp.Body, out BodyComponent? body))
            return;

        // I hate having to hardcode these checks so much.
        if (!HasComp<EyesComponent>(organEnt))
            return;
        var ev = new OrganEnabledEvent(organEnt);
        RaiseLocalEvent(organEnt, ref ev);
    }

    private void DisableOrgan(Entity<OrganComponent> organEnt)
    {
        if (!TryComp(organEnt.Comp.Body, out BodyComponent? body))
            return;

        // I hate having to hardcode these checks so much.
        if (!HasComp<EyesComponent>(organEnt))
            return;
        var ev = new OrganDisabledEvent(organEnt);
        RaiseLocalEvent(organEnt, ref ev);
    }
    // end of organ partial

    // move to parts partial
    /// <summary>
    ///     Tries to get a list of ValueTuples of EntityUid and OrganComponent on each organ
    ///     in the given part.
    /// </summary>
    /// <param name="uid">The part entity id to check on.</param>
    /// <param name="type">The type of component to check for.</param>
    /// <param name="part">The part to check for organs on.</param>
    /// <param name="organs">The organs found on the body part.</param>
    /// <returns>Whether any were found.</returns>
    /// <remarks>
    ///     This method is somewhat of a copout to the fact that we can't use reflection to generically
    ///     get the type of component on runtime due to sandboxing. So we simply do a HasComp check for each organ.
    /// </remarks>
    public bool TryGetBodyPartOrgans(
        EntityUid uid,
        Type type,
        [NotNullWhen(true)] out List<(EntityUid Id, OrganComponent Organ)>? organs,
        BodyPartComponent? part = null)
    {
        if (!Resolve(uid, ref part))
        {
            organs = null;
            return false;
        }

        var list = new List<(EntityUid Id, OrganComponent Organ)>();

        foreach (var organ in _body.GetPartOrgans(uid, part))
        {
            if (HasComp(organ.Id, type))
                list.Add((organ.Id, organ.Component));
        }

        if (list.Count != 0)
        {
            organs = list;
            return true;
        }

        organs = null;
        return false;
    }

    public void DropSlotContents(Entity<BodyPartComponent> partEnt)
    {
        if (partEnt.Comp.Body is null
            || !TryComp<InventoryComponent>(partEnt.Comp.Body, out var inventory) || // Prevent error for non-humanoids
            GetBodyPartCount(partEnt.Comp.Body.Value, partEnt.Comp.PartType) != 1
            || !TryGetPartSlotContainerName(partEnt.Comp.PartType, out var containerNames))
            return;

        foreach (var containerName in containerNames)
            DropSlotContent(partEnt.Comp.Body.Value, containerName, inventory);
    }

    private void DropSlotContent(EntityUid uid, string slotName, InventoryComponent? inventory = null)
    {
        if (!Resolve(uid, ref inventory))
            return;

        foreach (var slot in inventory.Slots)
        {
            if (slot.Name != slotName)
                continue;

            if (!_inventorySystem.TryGetSlotContainer(uid, slotName, out var container, out _, inventory))
                break;

            if (container.ContainedEntity is { } entityUid && TryComp(entityUid, out TransformComponent? transform) && _timing.IsFirstTimePredicted)
            {
                _transform.AttachToGridOrMap(entityUid, transform);
                _randomHelper.RandomOffset(entityUid, 0.5f);
            }

            break;
        }

        Dirty(uid, inventory);
    }

    private void RemoveLeg(Entity<BodyPartComponent> legEnt, Entity<BodyComponent?> bodyEnt)
    {
        if (!Resolve(bodyEnt, ref bodyEnt.Comp) || legEnt.Comp.PartType != BodyPartType.Leg)
            return;
        bodyEnt.Comp.LegEntities.Remove(legEnt);
        _body.UpdateMovementSpeed(bodyEnt);
        Dirty(bodyEnt, bodyEnt.Comp);
        _standing.Down(bodyEnt);
    }

    private void DisablePart(Entity<BodyPartComponent> partEnt)
    {
        if (!TryComp(partEnt.Comp.Body, out BodyComponent? body))
            return;

        switch (partEnt.Comp.PartType)
        {
            case BodyPartType.Leg:
                RemoveLeg(partEnt, (partEnt.Comp.Body.Value, body));
                break;
            case BodyPartType.Arm:
            {
                var hand = GetBodyChildrenOfType(partEnt.Comp.Body.Value, BodyPartType.Hand, symmetry: partEnt.Comp.Symmetry).FirstOrDefault();
                if (hand != default)
                {
                    var ev = new BodyPartDisabledEvent(hand);
                    RaiseLocalEvent(partEnt.Comp.Body.Value, ref ev);
                }

                break;
            }
            case BodyPartType.Hand:
            {
                var ev = new BodyPartDisabledEvent(partEnt);
                RaiseLocalEvent(partEnt.Comp.Body.Value, ref ev);
                break;
            }
        }
    }

    // TODO: Refactor this crap. I hate it so much.
    private void RemovePartEffect(Entity<BodyPartComponent> partEnt, Entity<BodyComponent?> bodyEnt)
    {
        if (TerminatingOrDeleted(bodyEnt)
            || !Resolve(bodyEnt, ref bodyEnt.Comp, logMissing: false))
            return;

        RemovePartChildren(partEnt, bodyEnt, bodyEnt.Comp);
    }

    public IEnumerable<(EntityUid Id, OrganComponent Component, WMOrganComponent WMComponent)> GetPartOrgans(EntityUid partId, BodyPartComponent? part = null)
    {
        if (!Resolve(partId, ref part, logMissing: false))
            yield break;

        foreach (var slotId in part.Organs.Keys)
        {
            var containerSlotId = SharedBodySystem.GetOrganContainerId(slotId);
            if (!_containers.TryGetContainer(partId, containerSlotId, out var container))
                continue;

            foreach (var containedEnt in container.ContainedEntities)
            {
                if (!TryComp(containedEnt, out OrganComponent? organ) || !TryComp<WMOrganComponent>(containedEnt, out var wmOrgan))
                    continue;
                yield return (containedEnt, organ,wmOrgan);
            }
        }
    }

    private void OnPartGibbed(Entity<BodyPartComponent> ent, ref BeingGibbedEvent args)
    {
        // Upstream drops Hands/Pockets. We drop Shoes/Gloves/Etc.
        DropSlotContents(ent);
        // Disable the part in WM so integrity checks stop immediately
        if (!TryComp<WMBodyPartComponent>(ent, out var wmPart))
            return;
        wmPart.Enabled = false;
        Dirty(ent, wmPart);
    }

    protected void RemovePartChildren(Entity<BodyPartComponent> partEnt, EntityUid bodyEnt, BodyComponent? body = null)
    {
        if (!Resolve(bodyEnt, ref body, logMissing: false) || partEnt.Comp.Children.Count == 0)
            return;

        foreach (var slotId in partEnt.Comp.Children.Keys)
        {
            var realId = SharedBodySystem.GetPartSlotContainerId(slotId);
            if (!_containers.TryGetContainer(partEnt, realId, out var container)
                || container is not ContainerSlot { ContainedEntity: { } childEntity }
                || !TryComp(childEntity, out BodyPartComponent? childPart))
                continue;
            var ev = new BodyPartEnableChangedEvent(false);
            RaiseLocalEvent(childEntity, ref ev);
            DropPart((childEntity, childPart));
        }

        Dirty(bodyEnt, body);
    }

    protected void DropPart(Entity<BodyPartComponent> partEnt)
    {
        DropSlotContents(partEnt);
        // I don't know if this can cause issues, since any part that's being detached HAS to have a Body.
        // though I really just want the compiler to shut the fuck up.
        var body = partEnt.Comp.Body.GetValueOrDefault();
        if (!TryComp(partEnt, out TransformComponent? transform) || !_timing.IsFirstTimePredicted)
            return;
        var enableEvent = new BodyPartEnableChangedEvent(false);
        RaiseLocalEvent(partEnt, ref enableEvent);
        var droppedEvent = new BodyPartDroppedEvent(partEnt);
        RaiseLocalEvent(body, ref droppedEvent);
        _transform.AttachToGridOrMap(partEnt, transform);
        _randomHelper.RandomOffset(partEnt, 0.5f);

    }

    private void OnAmputateAttempt(Entity<BodyPartComponent> partEnt, ref AmputateAttemptEvent args) =>
        DropPart(partEnt);

    private bool TryGetPartSlotContainerName(BodyPartType partType, out HashSet<string> containerNames)
    {
        containerNames = partType switch
        {
            BodyPartType.Hand => ["gloves"],
            BodyPartType.Foot => ["shoes"],
            BodyPartType.Head => ["eyes", "ears", "head", "mask"],
            _ => [],
        };
        return containerNames.Count > 0;
    }

    public bool MissingVitalOrgans(EntityUid uid)
    {
        if (!TryComp<BodyComponent>(uid, out var body))
            return false; // no organs to be missing

        var ent = (uid, body);
        return _body.TryGetBodyOrganEntityComps<BrainComponent>(ent, out _) && _body.TryGetBodyOrganEntityComps<HeartComponent>(ent, out _);
    }

    public bool CanAttachToSlot(
        EntityUid parentId,
        string slotId,
        BodyPartComponent? parentPart = null)
    {
        return Resolve(parentId, ref parentPart, logMissing: false)
               && parentPart.Children.ContainsKey(slotId);
    }

    private bool TryGetPartFromSlotContainer(string slot, out BodyPartType? partType)
    {
        partType = slot switch
        {
            "gloves" => BodyPartType.Hand,
            "shoes" => BodyPartType.Foot,
            "eyes" or "ears" or "head" or "mask" => BodyPartType.Head,
            _ => null
        };
        return partType is not null;
    }

    public int GetBodyPartCount(EntityUid bodyId, BodyPartType partType, BodyComponent? body = null)
    {
        if (!Resolve(bodyId, ref body, logMissing: false))
            return 0;

        var count = 0;
        foreach (var part in _body.GetBodyChildren(bodyId, body))
        {
            if (part.Component.PartType == partType)
                count++;
        }
        return count;
    }

    public IEnumerable<(EntityUid Id, BodyPartComponent Component)> GetBodyChildrenOfType(
        EntityUid bodyId,
        BodyPartType type,
        BodyComponent? body = null,
        BodyPartSymmetry? symmetry = null)
    {
        foreach (var part in _body.GetBodyChildren(bodyId, body))
            if (part.Component.PartType == type && (symmetry == null || part.Component.Symmetry == symmetry)) // Shitmed Change
                yield return part;

    }

    public string GetSlotFromBodyPart(EntityUid uid, BodyPartComponent? part = null, WMBodyPartComponent? wmPart = null)
    {
        var slotName = "";
        if (!Resolve(uid, ref part, ref wmPart))
            return slotName;

        slotName = wmPart.SlotId != "" ? wmPart.SlotId : part.PartType.ToString().ToLower();
        return part.Symmetry != BodyPartSymmetry.None ? $"{part.Symmetry.ToString().ToLower()} {slotName}" : slotName;
    }

    private void OnMapInit(Entity<BodyPartComponent> ent, ref MapInitEvent args)
    {
        if (!TryComp<WMBodyPartComponent>(ent, out var wmPart))
            return;
        if (ent.Comp.PartType == BodyPartType.Torso)
        {
            // For whatever reason this slot is initialized properly on the server, but not on the client.
            // This seems to be an issue due to wiz-merge, on my old branch it was properly instantiating
            // ItemInsertionSlot's container on both ends. It does show up properly on ItemSlotsComponent though.
            _slots.AddItemSlot(ent, wmPart.ContainerName, wmPart.ItemInsertionSlot);
            Dirty(ent, ent.Comp);
        }

        if (wmPart.OnAdd is not null || wmPart.OnRemove is not null)
            EnsureComp<BodyPartEffectComponent>(ent);

        foreach (var connection in ent.Comp.Children.Keys)
            _containers.EnsureContainer<ContainerSlot>(ent, SharedBodySystem.GetPartSlotContainerId(connection));


        foreach (var organ in ent.Comp.Organs.Keys)
            _containers.EnsureContainer<ContainerSlot>(ent, SharedBodySystem.GetOrganContainerId(organ));

    }

    private void OnBodyPartRemove(Entity<BodyPartComponent> ent, ref ComponentRemove args)
    {
        if (!TryComp<WMBodyPartComponent>(ent, out var comp)
            || ent.Comp.PartType != BodyPartType.Torso)
            return;
        _slots.RemoveItemSlot(ent, comp.ItemInsertionSlot);
    }

    private void OnPartEnableChanged(Entity<BodyPartComponent> partEnt, ref BodyPartEnableChangedEvent args)
    {
        if (!TryComp<WMBodyPartComponent>(partEnt, out var wmPart) || wmPart.CanEnable && args.Enabled)
            return;

        wmPart.Enabled = args.Enabled;

        if (args.Enabled)
        {
            EnablePart(partEnt);
            if (partEnt.Comp.Body is { Valid: true } bodyEnt)
                RaiseLocalEvent(partEnt, new BodyPartComponentsModifyEvent(bodyEnt, true));
        }
        else
        {
            DisablePart(partEnt);
            if (partEnt.Comp.Body is { Valid: true } bodyEnt)
                RaiseLocalEvent(partEnt, new BodyPartComponentsModifyEvent(bodyEnt, false));
        }

        Dirty(partEnt, partEnt.Comp);
    }

    private void AddLeg(Entity<BodyPartComponent> legEnt, Entity<BodyComponent?> bodyEnt)
    {
        if (!Resolve(bodyEnt, ref bodyEnt.Comp, logMissing: false))
            return;

        if (legEnt.Comp.PartType != BodyPartType.Leg)
            return;
        bodyEnt.Comp.LegEntities.Add(legEnt);
        _body.UpdateMovementSpeed(bodyEnt);
        Dirty(bodyEnt, bodyEnt.Comp);
    }

    private void EnablePart(Entity<BodyPartComponent> partEnt)
    {
        if (!TryComp(partEnt.Comp.Body, out BodyComponent? body))
            return;

        // I hate having to hardcode these checks so much.
        if (partEnt.Comp.PartType == BodyPartType.Leg)
            AddLeg(partEnt, (partEnt.Comp.Body.Value, body));
        if (partEnt.Comp.PartType == BodyPartType.Arm)
        {
            var hand = GetBodyChildrenOfType(partEnt.Comp.Body.Value, BodyPartType.Hand, symmetry: partEnt.Comp.Symmetry).FirstOrDefault();
            if (hand != default)
            {
                var ev = new BodyPartEnabledEvent(hand);
                RaiseLocalEvent(partEnt.Comp.Body.Value, ref ev);
            }
        }
        if (partEnt.Comp.PartType == BodyPartType.Hand)
        {
            var ev = new BodyPartEnabledEvent(partEnt);
            RaiseLocalEvent(partEnt.Comp.Body.Value, ref ev);
        }
    }

}
