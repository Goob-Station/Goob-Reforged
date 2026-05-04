using Content.Goobstation.Common.Chasm;
using Content.Goobstation.Shared.Chasm.Components;
using Content.Shared.Charges.Systems;
using Content.Shared.Chasm;
using Content.Shared.Destructible;
using Content.Shared.Stunnable;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;

namespace Content.Goobstation.Shared.Chasm;

/// <summary>
/// Controls effects of falling into different types of chasms.
/// </summary>
public sealed class GoobChasmSystem : EntitySystem
{
    [Dependency] private readonly SharedContainerSystem _container = default!;
    [Dependency] private readonly SharedChargesSystem _charges = default!;
    [Dependency] private readonly ChasmSystem _chasm = default!;
    [Dependency] private readonly EntityWhitelistSystem _whitelist = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ChasmFallingComponent, EntityTerminatingEvent>(OnFallingDelete);
        SubscribeLocalEvent<ChasmComponent, EntityTerminatingEvent>(OnChasmDelete);
        SubscribeLocalEvent<ChasmContainerComponent, MapInitEvent>(OnContainerInit);
        SubscribeLocalEvent<ChasmContainerComponent, DestructionEventArgs>(OnContainerDestruction);

        SubscribeLocalEvent<ChasmContainerComponent, ChasmFallEffectsEvent>(OnContainerFall);
        SubscribeLocalEvent<ChasmDeleteComponent, ChasmFallEffectsEvent>(OnDeleteFall);
        SubscribeLocalEvent<ChasmChargeComponent, ChasmFallEffectsEvent>(OnChargeFall);
    }

    private void OnFallingDelete(Entity<ChasmFallingComponent> ent, ref EntityTerminatingEvent args)
    {
        if (TryComp(ent.Comp.FallChasm, out ChasmComponent? chasm))
            _chasm.RemoveFallingEnt((ent.Comp.FallChasm.Value, chasm), ent.Owner);
    }

    private void OnChasmDelete(Entity<ChasmComponent> ent, ref EntityTerminatingEvent args)
    {
        foreach (var uid in ent.Comp.Falling)
        {
            if (!TerminatingOrDeleted(uid) && Exists(uid))
                RemComp<ChasmFallingComponent>(uid);
        }
    }

    private void OnContainerDestruction(Entity<ChasmContainerComponent> ent, ref DestructionEventArgs args)
    {
        var entities = _container.EmptyContainer(ent.Comp.Container);
        if (ent.Comp.DoStun)
        {
            foreach (var uid in entities)
            {
                RemComp<StunnedComponent>(uid);
            }
        }
    }

    private void OnContainerInit(Entity<ChasmContainerComponent> ent, ref MapInitEvent args)
    {
        ent.Comp.Container = _container.EnsureContainer<Container>(ent.Owner, "chasm");
    }

    private void OnDeleteFall(Entity<ChasmDeleteComponent> ent, ref ChasmFallEffectsEvent args)
    {
        PredictedQueueDel(args.Entity);
    }

    private void OnContainerFall(Entity<ChasmContainerComponent> ent, ref ChasmFallEffectsEvent args)
    {
        _container.Insert(args.Entity, ent.Comp.Container);

        if (ent.Comp.DoStun)
            EnsureComp<StunnedComponent>(args.Entity);
    }

    private void OnChargeFall(Entity<ChasmChargeComponent> ent, ref ChasmFallEffectsEvent args)
    {
        var toAdd = 0;
        if (ent.Comp.SpecialCharges != null)
        {
            foreach (var (charges, whitelist) in ent.Comp.SpecialCharges)
            {
                if (_whitelist.IsWhitelistFail(whitelist, args.Entity))
                    continue;

                toAdd += charges;
                if (ent.Comp.CanMultipleSpecials)
                    continue;

                break;
            }
        }
        else
        {
            toAdd = ent.Comp.DefaultCharges;
        }

        _charges.AddCharges(ent.Owner, toAdd);
    }
}
