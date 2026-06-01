using Robust.Shared.GameStates;

namespace Content.Goobstation.Shared.Spawner;

/// <summary>
/// Links entities together after they were spawned by this entity.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class SpawnerLinkComponent : Component;
