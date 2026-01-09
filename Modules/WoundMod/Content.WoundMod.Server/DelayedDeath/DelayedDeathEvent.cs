namespace Content.WoundMod.Server.DelayedDeath;

/// <summary>
/// 	Raised on a user when delayed death is triggered on them.
///     (E.G, they die to it.)
/// </summary>
[ByRefEvent]
public record struct DelayedDeathEvent(EntityUid User, bool Cancelled = false);
