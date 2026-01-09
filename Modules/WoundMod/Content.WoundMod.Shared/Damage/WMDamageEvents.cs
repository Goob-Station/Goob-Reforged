using Content.Shared.Damage;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Systems;
using Content.Shared.Inventory;
using Content.WoundMod.Shared.Targeting;

namespace Content.WoundMod.Shared.Damage;

/// <summary>
/// WoundMod extension of DamageModifyEvent with additional context
/// </summary>
public sealed class WMDamageModifyEvent(
    DamageSpecifier damage,
    EntityUid? origin = null,
    TargetBodyPart? targetPart = null,
    float armorPenetration = 0f)
    : EntityEventArgs, IInventoryRelayEvent
{
    public SlotFlags TargetSlots => ~SlotFlags.POCKET;

    public readonly DamageSpecifier OriginalDamage = damage;
    public DamageSpecifier Damage = damage;
    public readonly EntityUid? Origin = origin;
    public TargetBodyPart? TargetPart = targetPart;
    public float ArmorPenetration = armorPenetration;
}

/// <summary>
/// WoundMod extension of DamageChangedEvent with additional context
/// </summary>
public sealed class WMDamageChangedEvent : EntityEventArgs
{
    public readonly DamageableComponent Damageable;
    public readonly DamageSpecifier? DamageDelta;
    public readonly bool DamageIncreased;
    public readonly bool InterruptsDoAfters;
    public readonly EntityUid? Origin;

    // WoundMod additions
    public readonly bool CanSever;
    public TargetBodyPart? TargetPart;

    public WMDamageChangedEvent(
        DamageableComponent damageable,
        DamageSpecifier? damageDelta,
        bool interruptsDoAfters,
        EntityUid? origin,
        bool canSever = true,
        TargetBodyPart? targetPart = null)
    {
        Damageable = damageable;
        DamageDelta = damageDelta;
        Origin = origin;
        InterruptsDoAfters = interruptsDoAfters;
        CanSever = canSever;
        TargetPart = targetPart;

        if (DamageDelta is null)
            return;

        foreach (var damageChange in DamageDelta.DamageDict.Values)
        {
            if (damageChange <= 0)
                continue;

            DamageIncreased = true;

            break;
        }

        InterruptsDoAfters = interruptsDoAfters && DamageIncreased;
    }
}
