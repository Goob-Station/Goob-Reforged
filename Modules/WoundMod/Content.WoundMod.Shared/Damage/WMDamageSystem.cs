using Content.Shared.Body.Part;
using Content.Shared.Body.Systems;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Damage.Systems;
using Content.Shared.Popups;
using Content.Shared.Rejuvenate;
using Content.WoundMod.Shared.Body.Systems;
using Content.WoundMod.Shared.Targeting;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.WoundMod.Shared.Damage;

public sealed class WMDamageSystem : EntitySystem
{
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly SharedWMBodySystem _wmBody = default!;
    [Dependency] private readonly SharedBodySystem _body = default!;
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<TargetingComponent, BeforeDamageChangedEvent>(OnBeforeDamage);
        SubscribeLocalEvent<BodyPartComponent, DamageModifyEvent>(OnPartDamageModify);
        SubscribeLocalEvent<TargetingComponent, RejuvenateEvent>(OnRejuvenate);
    }

    private void OnBeforeDamage(EntityUid uid, TargetingComponent targeting, ref BeforeDamageChangedEvent args)
    {
        if (args.Cancelled)
            return;

        TargetBodyPart? targetPart;

        if (args.Origin.HasValue && TryComp<TargetingComponent>(args.Origin.Value, out var attackerTargeting))
        {
            targetPart = attackerTargeting.Target;
            if (targetPart == TargetBodyPart.Torso)
                targetPart = targetPart.Value | _wmBody.GetRandomPartSpread(10);
        }
        else if (args.Origin.HasValue)
            targetPart = _wmBody.CanEvadeDamage(uid) ? TargetBodyPart.Torso : _wmBody.GetRandomBodyPart(uid, targeting);
        else
            return;

        if (targetPart == null)
            return;

        args.Cancelled = true;

        var (partType, symmetry) = _wmBody.ConvertTargetBodyPart(targetPart.Value);
        var evadeChance = SharedWMBodySystem.GetEvadeChance(partType);

        if (_wmBody.CanEvadeDamage(uid) && _wmBody.TryEvadeDamage(uid, evadeChance))
        {
            if (_net.IsServer) // make this predicted
                _popup.PopupEntity(Loc.GetString("surgery-part-damage-evaded", ("user", uid)), uid, PopupType.Small);
            return;
        }

        var parts = _wmBody.GetBodyChildrenOfType(uid, partType, symmetry: symmetry);
        foreach (var part in parts)
        {
            _damageable.TryChangeDamage(part.Id, args.Damage, origin: args.Origin);
        }
    }

    private void OnPartDamageModify(Entity<BodyPartComponent> part, ref DamageModifyEvent args)
    {
        args.Damage *= SharedWMBodySystem.GetPartDamageModifier(part.Comp.PartType);

        if (_proto.TryIndex<DamageModifierSetPrototype>("PartDamage", out var modifierSet))
            args.Damage = DamageSpecifier.ApplyModifierSet(args.Damage,modifierSet);
    }

    private void OnRejuvenate(EntityUid uid, TargetingComponent component, RejuvenateEvent args)
    {
        foreach (var (partId, _) in _body.GetBodyChildren(uid))
            _damageable.SetAllDamage(partId, 0);
    }
}
