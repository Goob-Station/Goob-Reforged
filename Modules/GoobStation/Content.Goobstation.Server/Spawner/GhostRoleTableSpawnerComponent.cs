using Content.Shared.EntityTable.EntitySelectors;

namespace Content.Goobstation.Server.Spawner;

/// <summary>
/// A version of <see cref="GhostRoleTableSpawnerComponent"/> that supports <see cref="EntityTableSelector"/>.
/// </summary>
[RegisterComponent]
public sealed partial class GhostRoleTableSpawnerComponent : Component
{
    [DataField]
    public EntityTableSelector? Table;

    [DataField]
    public bool DeleteOnSpawn = true;

    [DataField]
    public int AvailableTakeovers = 1;

    [ViewVariables]
    public int CurrentTakeovers = 0;
}
