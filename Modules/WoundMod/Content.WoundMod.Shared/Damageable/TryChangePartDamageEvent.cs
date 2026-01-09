using Content.Shared.Damage;
using Content.WoundMod.Shared.Targeting;

namespace Content.WoundMod.Shared.Damageable;

[ByRefEvent]
public record struct TryChangePartDamageEvent(
    DamageSpecifier Damage,
    EntityUid? Origin = null,
    TargetBodyPart? TargetPart = null,
    bool IgnoreResistances = false,
    float ArmorPenetration = 0f,
    bool CanSever = true,
    bool CanEvade = false,
    float PartMultiplier = 1.00f,
    bool Evaded = false,
    bool Cancelled = false);
