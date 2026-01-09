// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2024 Skubman <ba.fallaria@gmail.com>
// SPDX-FileCopyrightText: 2024 gluesniffler <159397573+gluesniffler@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 whateverusername0 <whateveremail>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aviu00 <93730715+Aviu00@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aviu00 <aviu00@protonmail.com>
// SPDX-FileCopyrightText: 2025 GoobBot <uristmchands@proton.me>
// SPDX-FileCopyrightText: 2025 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 deltanedas <@deltanedas:kde.org>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Diagnostics.CodeAnalysis;
using Content.Shared.Body.Components;
using Content.Shared.Body.Part;
using Content.Shared.Damage;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Damage.Systems;
using Content.Shared.FixedPoint;
using Content.Shared.IdentityManagement;
using Content.Shared.Inventory;
using Content.Shared.Mobs.Components;
using Content.WoundMod.Shared.Body.Events;
using Content.WoundMod.Shared.Body.Part;
using Content.WoundMod.Shared.Damage;
using Content.WoundMod.Shared.Damageable;
using Content.WoundMod.Shared.Surgery.Steps.Parts;
using Content.WoundMod.Shared.Targeting;
using Robust.Shared.CPUJob.JobQueues;
using Robust.Shared.CPUJob.JobQueues.Queues;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

// Namespace has set accessors, leaving it on the default.
namespace Content.WoundMod.Shared.Body.Systems;

public partial class SharedWMBodySystem
{
    private readonly ProtoId<DamageTypePrototype>[] _severingDamageTypes = { "Slash", "Piercing", "Blunt" };
    private const double IntegrityJobTime = 0.005;
    private readonly JobQueue _integrityJobQueue = new(IntegrityJobTime);
    public sealed class IntegrityJob(
        SharedWMBodySystem self,
        Entity<BodyPartComponent> ent,
        double maxTime,
        CancellationToken cancellation = default)
        : Job<object>(maxTime, cancellation)
    {
        protected override Task<object?> Process()
        {
            self.ProcessIntegrityTick(ent);

            return Task.FromResult<object?>(null);
        }
    }

    private EntityQuery<TargetingComponent> _queryTargeting;
    private void InitializeIntegrityQueue()
    {
        _queryTargeting = GetEntityQuery<TargetingComponent>();
        SubscribeLocalEvent<BodyComponent, TryChangePartDamageEvent>(OnTryChangePartDamage);
        SubscribeLocalEvent<BodyComponent, DamageModifyEvent>(OnBodyDamageModify);
        SubscribeLocalEvent<BodyComponent, WMDamageModifyEvent>(OnWMBodyDamageModify);
        SubscribeLocalEvent<BodyPartComponent, DamageModifyEvent>(OnPartDamageModify);
        SubscribeLocalEvent<BodyPartComponent, WMDamageModifyEvent>(OnWMPartDamageModify);
        SubscribeLocalEvent<BodyPartComponent, DamageChangedEvent>(OnDamageChanged);
    }
    private void ProcessIntegrityTick(Entity<BodyPartComponent> entity)
    {
        if (!TryComp<DamageableComponent>(entity, out var damageable) || !TryComp<WMBodyPartComponent>(entity, out var wmPart))
            return;

        var damage = damageable.TotalDamage;

        if (entity.Comp is { Body: { } body }
            && damage > wmPart.MinIntegrity
            && damage <= wmPart.IntegrityThresholds[TargetIntegrity.HeavilyWounded]
            && _queryTargeting.HasComp(body)
            && !_mobState.IsDead(body))
            ;
        // _damageable.TryChangeDamage(entity, GetHealingSpecifier(entity), canSever: false, targetPart: GetTargetBodyPart(entity));
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        _integrityJobQueue.Process();

        if (!_timing.IsFirstTimePredicted)
            return;

        using var query = EntityQueryEnumerator<BodyPartComponent, WMBodyPartComponent>();
        while (query.MoveNext(out var ent, out var part, out var wmPart))
        {
            wmPart.HealingTimer += frameTime;

            if (wmPart.HealingTimer < wmPart.HealingTime)
                continue;
            wmPart.HealingTimer = 0;
            _integrityJobQueue.EnqueueJob(new IntegrityJob(this, (ent, part), IntegrityJobTime));
        }
    }

    private void OnTryChangePartDamage(Entity<BodyComponent> ent, ref TryChangePartDamageEvent args)
    {
        // If our target has a TargetingComponent, that means they will take limb damage
        // And if their attacker also has one, then we use that part.
        if (!_queryTargeting.TryComp(ent, out var targetEnt))
            return;
        var damage = args.Damage;
        TargetBodyPart? targetPart = null;

        if (args.TargetPart != null)
        {
            targetPart = args.TargetPart;
        }
        else if (args.Origin.HasValue && _queryTargeting.TryComp(args.Origin.Value, out var targeter))
        {
            targetPart = targeter.Target;
            // If the target is Torso then have a 33% chance to hit another part
            if (targetPart.Value == TargetBodyPart.Torso)
            {
                var additionalPart = GetRandomPartSpread(10);
                targetPart = targetPart.Value | additionalPart;
            }
        }
        else
        {
            // If there's an origin in this case, that means it comes from an entity without TargetingComponent,
            // such as an animal, so we attack a random part.
            if (args.Origin.HasValue)
            {
                // Evasion would trigger constantly if we don't target torso
                targetPart = args.CanEvade ? TargetBodyPart.Torso : GetRandomBodyPart(ent, targetEnt);
            }
            // Otherwise we damage all parts equally (barotrauma, explosions, etc).
            else
            {
                // Division by 2 cuz damaging all parts by the same damage by default is too much.
                damage /= 2;
                targetPart = TargetBodyPart.All;
            }
        }

        if (targetPart == null)
            return;

        if (TryChangePartDamage(ent,
                args.Damage,
                args.IgnoreResistances,
                args.ArmorPenetration,
                args.CanSever,
                args.CanEvade,
                args.PartMultiplier,
                targetPart.Value,
                out var evaded)
            || !args.CanEvade || !evaded)
            return;
        if (_net.IsServer)
            _popup.PopupEntity(Loc.GetString("surgery-part-damage-evaded", ("user", Identity.Entity(ent, EntityManager))), ent);

        args.Evaded = true;
    }

    private void OnBodyDamageModify(Entity<BodyComponent> bodyEnt, ref DamageModifyEvent args)
    {
        var ev = new WMDamageModifyEvent(args.Damage);
        RaiseLocalEvent(bodyEnt, ref ev);
    }

    private void OnWMBodyDamageModify(Entity<BodyComponent> bodyEnt, ref WMDamageModifyEvent args)
    {
        if (args.TargetPart == null)
            return;
        var (targetType, _) = ConvertTargetBodyPart(args.TargetPart.Value);
        args.Damage *= GetPartDamageModifier(targetType);
    }
    private void OnWMPartDamageModify(Entity<BodyPartComponent> ent, ref WMDamageModifyEvent args)
    {
        if (ent.Comp.Body != null
            && TryComp(ent.Comp.Body.Value, out InventoryComponent? inventory))
            _inventorySystem.RelayEvent((ent.Comp.Body.Value, inventory), ref args);

        if (_prototypeManager.TryIndex<DamageModifierSetPrototype>("PartDamage", out var partModifierSet))
        {
            //args.Damage = DamageSpecifier.ApplyModifierSet(args.Damage, DamageSpecifier.PenetrateArmor(partModifierSet, args.ArmorPenetration));
        }

        args.Damage *= GetPartDamageModifier(ent.Comp.PartType);

    }

    private void OnPartDamageModify(Entity<BodyPartComponent> partEnt, ref DamageModifyEvent args)
    {
        var ev = new WMDamageModifyEvent(args.Damage);
        RaiseLocalEvent(partEnt, ref ev);
    }

    private bool TryChangePartDamage(EntityUid entity,
        DamageSpecifier damage,
        bool ignoreResistances,
        float armorPenetration,
        bool canSever,
        bool canEvade,
        float partMultiplier,
        TargetBodyPart targetParts,
        out bool evaded)
    {
        evaded = false;

        if (damage.GetTotal() == 0)
            return false;

        var landed = false;
        var targets = SharedTargetingSystem.GetValidParts();
        foreach (var target in targets)
        {
            if (!targetParts.HasFlag(target))
                continue;

            var (targetType, targetSymmetry) = ConvertTargetBodyPart(target);
            if (GetBodyChildrenOfType(entity, targetType, symmetry: targetSymmetry) is not { } part)
                continue;
            if (canEvade && TryEvadeDamage(entity, GetEvadeChance(targetType)))
            {
                evaded = true;
                continue;
            }

            var damageResult =
                _damageable.TryChangeDamage(part.FirstOrDefault().Id, damage * partMultiplier, ignoreResistances); // canSever: canSever, armorPenetration: armorPenetration);
            if (damageResult && (damage * partMultiplier).GetTotal() != 0)
                landed = true;
        }

        return landed;
    }

    private void OnDamageChanged(Entity<BodyPartComponent> partEnt, ref DamageChangedEvent args)
    {
        var wmEvent = new WMDamageChangedEvent(args.Damageable, args.DamageDelta, args.InterruptsDoAfters, args.Origin);
        RaiseLocalEvent(partEnt, ref wmEvent);
    }

    private void OnWMDamageChanged(Entity<BodyPartComponent> partEnt, ref WMDamageChangedEvent args)
    {
        if (!TryComp<DamageableComponent>(partEnt, out var damageable) || !TryComp<WMBodyPartComponent>(partEnt, out var wmPart))
            return;

        var severed = false;
        var partIdSlot = _body.GetParentPartAndSlotOrNull(partEnt)?.Slot;
        var delta = args.DamageDelta;

        if (args.CanSever
            && wmPart.CanSever
            && partIdSlot is not null
            && delta != null
            && !HasComp<BodyPartReattachedComponent>(partEnt)
            && !wmPart.Enabled
            && damageable.TotalDamage >= wmPart.SeverIntegrity
            && _severingDamageTypes.Any(damageType => delta.DamageDict.TryGetValue(damageType, out var value) && value > 0))
            severed = true;

        CheckBodyPart(partEnt, GetTargetBodyPart(partEnt), severed, damageable);

        if (severed)
            DropPart(partEnt);

        Dirty(partEnt, partEnt.Comp);
    }

    /// <summary>
    /// Gets the random body part rolling a number between 1 and 9, and returns
    /// Torso if the result is 9 or more. The higher torsoWeight is, the higher chance to return it.
    /// By default, the chance to return Torso is 50%.
    /// </summary>
    private TargetBodyPart GetRandomPartSpread(ushort torsoWeight = 9)
    {
        var rand = new System.Random((int) _timing.CurTick.Value);

        const int targetPartsAmount = 9;
        // 5 = amount of target parts except Torso
        return rand.Next(1, targetPartsAmount + torsoWeight) switch
        {
            1 => TargetBodyPart.Head,
            2 => TargetBodyPart.RightArm,
            3 => TargetBodyPart.RightHand,
            4 => TargetBodyPart.LeftArm,
            5 => TargetBodyPart.LeftHand,
            6 => TargetBodyPart.RightLeg,
            7 => TargetBodyPart.RightFoot,
            8 => TargetBodyPart.LeftLeg,
            9 => TargetBodyPart.LeftFoot,
            _ => TargetBodyPart.Torso,
        };
    }

    public TargetBodyPart? GetRandomBodyPart(EntityUid uid, TargetingComponent? target = null)
    {
        if (!Resolve(uid, ref target, false))
            return null;

        var rand = new System.Random((int) _timing.CurTick.Value);

        var totalWeight = target.TargetOdds.Values.Sum();
        var randomValue = rand.NextFloat() * totalWeight;

        foreach (var (part, weight) in target.TargetOdds)
        {
            if (randomValue <= weight)
                return part;
            randomValue -= weight;
        }

        return TargetBodyPart.Torso; // Default to torso if something goes wrong
    }

    /// <summary>
    /// This should be called after body part damage was changed.
    /// </summary>
    public void CheckBodyPart(
        Entity<BodyPartComponent> partEnt,
        TargetBodyPart? targetPart,
        bool severed,
        DamageableComponent? damageable = null,
        WMBodyPartComponent? wmPart = null)
    {
        if (!Resolve(partEnt, ref damageable, ref wmPart))
            return;

        var integrity = damageable.TotalDamage;

        // KILL the body part
        if (wmPart.Enabled && integrity >= wmPart.IntegrityThresholds[TargetIntegrity.CriticallyWounded])
        {
            var ev = new BodyPartEnableChangedEvent(false);
            RaiseLocalEvent(partEnt, ref ev);
        }

        // LIVE the body part
        if (!wmPart.Enabled && integrity <= wmPart.IntegrityThresholds[wmPart.EnableIntegrity] && !severed)
        {
            var ev = new BodyPartEnableChangedEvent(true);
            RaiseLocalEvent(partEnt, ref ev);
        }

        if (!_queryTargeting.TryComp(partEnt.Comp.Body, out var targeting)
            || !HasComp<MobStateComponent>(partEnt.Comp.Body))
            return;
        var newIntegrity = GetIntegrityThreshold(wmPart, integrity.Float(), severed);
        // We need to check if the part is dead to prevent the UI from showing dead parts as alive.
        if (targetPart is not null &&
            targeting.BodyStatus.TryGetValue(targetPart.Value, out var value) && value != TargetIntegrity.Dead)
        {
            targeting.BodyStatus[targetPart.Value] = newIntegrity;
            if (targetPart.Value == TargetBodyPart.Torso)
                targeting.BodyStatus[TargetBodyPart.Groin] = newIntegrity;
            Dirty(partEnt.Comp.Body.Value, targeting);
        }
        // Revival events are handled by the server, so we end up being locked to a network event.
        // I hope you like the _net.IsServer, Remuchi :)
        if (_net.IsServer)
            RaiseNetworkEvent(new TargetIntegrityChangeEvent(GetNetEntity(partEnt.Comp.Body.Value)), partEnt.Comp.Body.Value);
    }

    /// <summary>
    /// Gets the integrity of all body parts in the entity.
    /// </summary>
    public Dictionary<TargetBodyPart, TargetIntegrity> GetBodyPartStatus(EntityUid entityUid)
    {
        var result = new Dictionary<TargetBodyPart, TargetIntegrity>();

        if (!TryComp<BodyComponent>(entityUid, out var body))
            return result;

        foreach (var part in SharedTargetingSystem.GetValidParts())
        {
            result[part] = TargetIntegrity.Severed;
        }

        foreach (var partComponent in _body.GetBodyChildren(entityUid, body))
        {
            var targetBodyPart = GetTargetBodyPart(partComponent.Component.PartType, partComponent.Component.Symmetry);

            if (targetBodyPart != null
                && TryComp<DamageableComponent>(partComponent.Id, out var damageable)
                && TryComp<WMBodyPartComponent>(partComponent.Component.Body, out var wmPart))
                result[targetBodyPart.Value] = GetIntegrityThreshold(wmPart, damageable.TotalDamage.Float(), false);
        }

        // Hardcoded shitcode for Groin :)
        result[TargetBodyPart.Groin] = result[TargetBodyPart.Torso];

        return result;
    }

    public TargetBodyPart? GetTargetBodyPart(Entity<BodyPartComponent> part) => GetTargetBodyPart(part.Comp.PartType, part.Comp.Symmetry);
    public TargetBodyPart? GetTargetBodyPart(BodyPartComponent part) => GetTargetBodyPart(part.PartType, part.Symmetry);

    /// <summary>
    /// Converts Enums from BodyPartType to their Targeting system equivalent.
    /// </summary>
    public static TargetBodyPart? GetTargetBodyPart(BodyPartType type, BodyPartSymmetry symmetry)
    {
        return (type, symmetry) switch
        {
            (BodyPartType.Head, _) => TargetBodyPart.Head,
            (BodyPartType.Torso, _) => TargetBodyPart.Torso,
            (BodyPartType.Arm, BodyPartSymmetry.Left) => TargetBodyPart.LeftArm,
            (BodyPartType.Arm, BodyPartSymmetry.Right) => TargetBodyPart.RightArm,
            (BodyPartType.Hand, BodyPartSymmetry.Left) => TargetBodyPart.LeftHand,
            (BodyPartType.Hand, BodyPartSymmetry.Right) => TargetBodyPart.RightHand,
            (BodyPartType.Leg, BodyPartSymmetry.Left) => TargetBodyPart.LeftLeg,
            (BodyPartType.Leg, BodyPartSymmetry.Right) => TargetBodyPart.RightLeg,
            (BodyPartType.Foot, BodyPartSymmetry.Left) => TargetBodyPart.LeftFoot,
            (BodyPartType.Foot, BodyPartSymmetry.Right) => TargetBodyPart.RightFoot,
            _ => null,
        };
    }

    /// <summary>
    /// Converts Enums from Targeting system to their BodyPartType equivalent.
    /// </summary>
    public (BodyPartType Type, BodyPartSymmetry Symmetry) ConvertTargetBodyPart(TargetBodyPart targetPart)
    {
        return targetPart switch
        {
            TargetBodyPart.Head => (BodyPartType.Head, BodyPartSymmetry.None),
            TargetBodyPart.Torso => (BodyPartType.Torso, BodyPartSymmetry.None),
            TargetBodyPart.Groin => (BodyPartType.Torso, BodyPartSymmetry.None), // TODO: Groin is not a part type yet
            TargetBodyPart.LeftArm => (BodyPartType.Arm, BodyPartSymmetry.Left),
            TargetBodyPart.LeftHand => (BodyPartType.Hand, BodyPartSymmetry.Left),
            TargetBodyPart.RightArm => (BodyPartType.Arm, BodyPartSymmetry.Right),
            TargetBodyPart.RightHand => (BodyPartType.Hand, BodyPartSymmetry.Right),
            TargetBodyPart.LeftLeg => (BodyPartType.Leg, BodyPartSymmetry.Left),
            TargetBodyPart.LeftFoot => (BodyPartType.Foot, BodyPartSymmetry.Left),
            TargetBodyPart.RightLeg => (BodyPartType.Leg, BodyPartSymmetry.Right),
            TargetBodyPart.RightFoot => (BodyPartType.Foot, BodyPartSymmetry.Right),
            _ => (BodyPartType.Torso, BodyPartSymmetry.None)
        };

    }

    public bool TryGetHealingSpecifier(Entity<WMBodyPartComponent?> part, [NotNullWhen(true)] out DamageSpecifier? damageSpecifier)
    {
        damageSpecifier = null;
        if (!Resolve(part, ref part.Comp))
            return false;

        damageSpecifier = new DamageSpecifier()
        {
            DamageDict = new Dictionary<string, FixedPoint2>()
            {
                { "Blunt", -part.Comp.SelfHealingAmount },
                { "Slash", -part.Comp.SelfHealingAmount },
                { "Piercing", -part.Comp.SelfHealingAmount },
                { "Heat", -part.Comp.SelfHealingAmount },
                { "Cold", -part.Comp.SelfHealingAmount },
                { "Shock", -part.Comp.SelfHealingAmount },
                { "Caustic", -part.Comp.SelfHealingAmount * 0.1}, // not much caustic healing
            },
        };

        return true;
    }

    /// <summary>
    /// Fetches the damage multiplier for part integrity based on part types.
    /// </summary>
    /// TODO: Serialize this per body part.
    public static float GetPartDamageModifier(BodyPartType partType)
    {
        return partType switch
        {
            BodyPartType.Head => 0.5f, // 50% damage, necks are hard to cut
            BodyPartType.Torso => 1.0f, // 100% damage
            BodyPartType.Arm => 0.7f, // 70% damage
            BodyPartType.Hand => 0.7f, // 70% damage
            BodyPartType.Leg => 0.7f, // 70% damage
            BodyPartType.Foot => 0.7f, // 70% damage
            _ => 0.5f,
        };
    }

    /// <summary>
    /// Fetches the TargetIntegrity equivalent of the current integrity value for the body part.
    /// </summary>
    public static TargetIntegrity GetIntegrityThreshold(WMBodyPartComponent component, float integrity, bool severed)
    {
        if (severed)
            return TargetIntegrity.Severed;
        else if (!component.Enabled)
            return TargetIntegrity.Disabled;

        var targetIntegrity = TargetIntegrity.Healthy;
        foreach (var threshold in component.IntegrityThresholds)
        {
            if (integrity <= threshold.Value)
                targetIntegrity = threshold.Key;
        }

        return targetIntegrity;
    }

    /// <summary>
    /// Fetches the chance to evade integrity damage for a body part.
    /// Used when the entity is not dead, laying down, or incapacitated.
    /// </summary>
    public static float GetEvadeChance(BodyPartType partType)
    {
        return partType switch
        {
            BodyPartType.Head => 0.70f,  // 70% chance to evade
            BodyPartType.Arm => 0f,   // 0% chance to evade
            BodyPartType.Hand => 0f, // 0% chance to evade
            BodyPartType.Leg => 0f,   // 0% chance to evade
            BodyPartType.Foot => 0f, // 0% chance to evade
            BodyPartType.Torso => 0f, // 0% chance to evade
            _ => 0f
        };
    }

    public bool CanEvadeDamage(EntityUid uid)
    {
        return !_mobState.IsIncapacitated(uid) && !_standing.IsDown(uid);
    }

    public bool TryEvadeDamage(EntityUid uid, float evadeChance)
    {
        if (!CanEvadeDamage(uid))
            return false;

        if (evadeChance == 0f)
            return false;

        var rand = new Random((int) _timing.CurTick.Value);

        return rand.Prob(evadeChance);
    }

}
