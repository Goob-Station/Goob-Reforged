// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: LicenseRef-GAG-1.0.txt

using Content.Shared.DoAfter;
using Content.Shared.Hands;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Popups;
using Content.Shared.Storage;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Tag;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.Prototypes;

namespace Content.Goobstation.Shared.PseudoItem;

public abstract partial class SharedPseudoItemSystem : EntitySystem
{
    [Dependency] private SharedStorageSystem _storage = default!;
    [Dependency] private SharedItemSystem _item = default!;
    [Dependency] private SharedDoAfterSystem _doAfter = default!;
    [Dependency] private TagSystem _tag = default!;
    [Dependency] private SharedPopupSystem _popupSystem = default!;

    private static readonly ProtoId<TagPrototype> PreventTag = "PreventLabel";

    [SubscribeLocalEvent]
    private void AddInsertVerb(EntityUid uid, PseudoItemComponent component, GetVerbsEvent<InnateVerb> args)
    {
        if (!args.CanInteract || !args.CanAccess)
            return;

        if (component.Active)
            return;

        if (!TryComp<StorageComponent>(args.Target, out var targetStorage))
            return;

        if (!CheckItemFits((uid, component), (args.Target, targetStorage)))
            return;

        if (Transform(args.Target).ParentUid == uid)
            return;

        InnateVerb verb = new()
        {
            Act = () =>
            {
                TryInsert(args.Target, uid, component, targetStorage);
            },
            Text = Loc.GetString("action-name-insert-self"),
            IconEntity = GetNetEntity(uid),
            Priority = 2
        };
        args.Verbs.Add(verb);
    }

    [SubscribeLocalEvent]
    private void OnEntRemoved(EntityUid uid, PseudoItemComponent component, EntGotRemovedFromContainerMessage args)
    {
        if (!component.Active)
            return;

        RemComp<ItemComponent>(uid);
        component.Active = false;
    }

    [SubscribeLocalEvent]
    private void OnDropAttempt(EntityUid uid, PseudoItemComponent component, DropAttemptEvent args)
    {
        if (component.Active)
            args.Cancel();
    }

    [SubscribeLocalEvent]
    private void OnInsertAttempt(EntityUid uid, PseudoItemComponent component, ContainerGettingInsertedAttemptEvent args)
    {
        if (!component.Active)
            return;
        // This hopefully shouldn't trigger, but this is a failsafe just in case so we dont bluespace them cats
        args.Cancel();
    }

    [SubscribeLocalEvent]
    // Prevents moving within the bag :)
    private void OnInteractAttempt(EntityUid uid, PseudoItemComponent component, InteractionAttemptEvent args)
    {
        if (args.Uid == args.Target && component.Active)
            args.Cancelled = true;
    }

    [SubscribeLocalEvent]
    private void OnDoAfter(EntityUid uid, PseudoItemComponent component, DoAfterEvent args)
    {
        if (args.Handled || args.Cancelled || args.Args.Used == null)
            return;

        args.Handled = TryInsert(args.Args.Used.Value, uid, component);
    }

    [SubscribeLocalEvent]
    private void OnAttackAttempt(EntityUid uid, PseudoItemComponent component, AttackAttemptEvent args)
    {
        if (component.Active)
            args.Cancel();
    }

    public bool CheckItemFits(Entity<PseudoItemComponent?> itemEnt, Entity<StorageComponent?> storageEnt)
    {
        if (!Resolve(itemEnt, ref itemEnt.Comp) || !Resolve(storageEnt, ref storageEnt.Comp)
                                                || TerminatingOrDeleted(itemEnt))
            return false;

        // If the entity doesn't have an item comp, create a temporary one
        // No, we cannot do the bullshit that was done earlier due to the fact i cannot give access to this system from ItemComponent easily
        // and even if i could, i cannot feed the wrong entity and comp combo to CanInsert.
        // We add the real itemcomp later.
        if (!TryComp<ItemComponent>(itemEnt, out var item))
        {
            item = EnsureComp<ItemComponent>(itemEnt);
            _item.SetSize(itemEnt, itemEnt.Comp.Size, item);
            _item.SetShape(itemEnt, itemEnt.Comp.Shape, item);
            _item.SetStoredOffset(itemEnt, itemEnt.Comp.StoredOffset, item);
            var canInsert = _storage.CanInsert(storageEnt, itemEnt, out _, storageEnt.Comp, item, ignoreStacks: true);
            RemComp<ItemComponent>(itemEnt);
            return canInsert;
        }

        return _storage.CanInsert(storageEnt, itemEnt, out _, storageEnt.Comp, item, ignoreStacks: true);
    }

    public bool TryInsert(EntityUid storageUid,
        EntityUid toInsert,
        PseudoItemComponent component,
        StorageComponent? storage = null)
    {
        if (!Resolve(storageUid, ref storage))
            return false;

        if (!CheckItemFits((toInsert, component), (storageUid, storage)))
            return false;

        var itemComp = EnsureComp<ItemComponent>(toInsert);
        _item.SetSize(toInsert, component.Size, itemComp);
        _item.SetShape(toInsert, component.Shape, itemComp);
        _item.SetStoredOffset(toInsert, component.StoredOffset, itemComp);
        _item.VisualsChanged(toInsert);

        _tag.TryAddTag(toInsert, PreventTag);

        if (!_storage.Insert(storageUid, toInsert, out _, null, storage))
        {
            component.Active = false;
            RemComp<ItemComponent>(toInsert);
            return false;
        }

        component.Active = true;
        return true;
    }


    protected void StartInsertDoAfter(EntityUid inserter, EntityUid toInsert, EntityUid storageEntity, PseudoItemComponent? pseudoItem = null)
    {
        if (!Resolve(toInsert, ref pseudoItem))
            return;

        var ev = new PseudoItemInsertDoAfterEvent();
        var args = new DoAfterArgs(EntityManager, inserter, 5f, ev, toInsert, toInsert, storageEntity)
        {
            BreakOnMove = true,
            NeedHand = true
        };

        if (_doAfter.TryStartDoAfter(args))
        {
            // Show a popup to the person getting picked up
            _popupSystem.PopupEntity(Loc.GetString("carry-started", ("carrier", inserter)), toInsert, toInsert);
        }
    }
}

