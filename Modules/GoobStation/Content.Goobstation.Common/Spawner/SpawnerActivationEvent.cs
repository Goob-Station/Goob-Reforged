namespace Content.Goobstation.Common.Spawner;

/// <summary>
/// Raised after an entity was spawned from any sort of spawner.
/// </summary>
/// <param name="Spawned"></param>
[ByRefEvent]
public record struct SpawnerActivationEvent(EntityUid Spawned);
