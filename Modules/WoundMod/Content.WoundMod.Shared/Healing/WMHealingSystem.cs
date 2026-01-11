using Content.Shared.Damage.Components;
using Content.Shared.Damage.Systems;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Medical;
using Content.Shared.Medical.Healing;
using Content.Shared.Popups;
using Content.Shared.Stacks;
using Content.WoundMod.Shared.Body.Systems;
using Content.WoundMod.Shared.Targeting;
using Robust.Shared.Audio.Systems;

namespace Content.WoundMod.Shared.Healing;

public sealed class WMHealingSystem : EntitySystem
{
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly SharedStackSystem _stacks = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedWMBodySystem _wmBody = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DamageableComponent, HealingDoAfterEvent>(OnHealingDoAfter);
        SubscribeLocalEvent<HealingComponent, MapInitEvent>(OnHealingMapInit);
        SubscribeLocalEvent<HealingComponent, UseInHandEvent>(OnHealingUse, before: [typeof(HealingSystem)]);
        SubscribeLocalEvent<HealingComponent, AfterInteractEvent>(OnHealingInteract, before: [typeof(HealingSystem)]);
    }

    private void OnHealingMapInit(Entity<HealingComponent> ent, ref MapInitEvent args)
    {
        // if i had reflection bro this wouldnt be a problem but fuck me ig.
        EnsureComp<WMHealingComponent>(ent);
    }

    private void OnHealingInteract(Entity<HealingComponent> healing, ref AfterInteractEvent args)
    {
        if (args.Handled || args.Target == null || TryStartLimbHeal(healing, args.User, args.Target.Value))
            args.Handled = true;
    }

    private void OnHealingUse(Entity<HealingComponent> healing, ref UseInHandEvent args)
    {
        if (!args.Handled || TryStartLimbHeal(healing, args.User, args.User))
            args.Handled = true;
    }

    private bool TryStartLimbHeal(Entity<HealingComponent> healing, EntityUid user, EntityUid target)
    {
        if (!TryComp<TargetingComponent>(user, out var targeting) || targeting.Target == TargetBodyPart.Torso)
            return false;

        var (type, symmetry) = _wmBody.ConvertTargetBodyPart(targeting.Target);
        var parts = _wmBody.GetBodyChildrenOfType(target, type, symmetry: symmetry);
        var limbDamaged = false;

        foreach (var part in parts)
        {
            if (!TryComp<DamageableComponent>(part.Id, out var dmg) || dmg.TotalDamage <= 0)
                continue;
            limbDamaged = true;
            break;
        }

        if (!limbDamaged)
            return false;

        _audio.PlayPredicted(healing.Comp.HealingBeginSound, healing, user);

        var doAfterArgs = new DoAfterArgs(EntityManager, user, healing.Comp.Delay, new HealingDoAfterEvent(), target, target: target, used: healing)
        {
            NeedHand = true,
            BreakOnMove = true,
        };

        _doAfter.TryStartDoAfter(doAfterArgs);
        return true;
    }

    private void OnHealingDoAfter(Entity<DamageableComponent> target, ref HealingDoAfterEvent args)
    {
        if (args.Handled
            || args.Cancelled
            || !TryComp(args.Used, out HealingComponent? healing)
            || !TryComp<TargetingComponent>(args.User, out var targeting)
            || targeting.Target == TargetBodyPart.Torso)
            return;

        var (type, symmetry) = _wmBody.ConvertTargetBodyPart(targeting.Target);
        var parts = _wmBody.GetBodyChildrenOfType(target, type, symmetry: symmetry);
        var healedAny = false;
        var totalHealed = FixedPoint2.Zero;

        foreach (var part in parts)
        {
            if(! _damageable.TryChangeDamage(
                   part.Id,
                   healing.Damage * _damageable.UniversalTopicalsHealModifier,
                   ignoreResistances: true,
                   origin: args.User))
                continue;

            healedAny = true;
            totalHealed += (healing.Damage * _damageable.UniversalTopicalsHealModifier).GetTotal();
        }

        if (!healedAny)
            return;

        if (TryComp<StackComponent>(args.Used, out _))
            _stacks.TryUse(args.Used.Value, 1);
        else
            QueueDel(args.Used.Value);

        _audio.PlayPredicted(healing.HealingEndSound, target, args.User);
        _popup.PopupPredicted(Loc.GetString("medical-item-finished-using", ("item", args.Used)), target, args.User);
        args.Handled = true;
        args.Repeat = false;
    }
}
