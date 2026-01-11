using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.WoundMod.Shared.Healing;

[Serializable, NetSerializable]
public sealed partial class WMHealingDoAfterEvent : SimpleDoAfterEvent;
