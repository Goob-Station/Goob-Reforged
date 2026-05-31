using Content.Shared.Maps;
using Content.Shared.Trigger.Components.Effects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Goobstation.Shared.Trigger.Components.Effects;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class TileReplaceOnTriggerComponent : BaseXOnTriggerComponent
{
    [DataField, AutoNetworkedField]
    public ProtoId<ContentTileDefinition> Tile;

    /// <summary>
    /// The default range in which the tiles are spawned.
    /// </summary>
    [DataField, AutoNetworkedField]
    public float Radius = 1f;

    /// <summary>
    /// The chance to spawn a tile for each tile inside the radius.
    /// </summary>
    [DataField, AutoNetworkedField]
    public float Prob = 0.1f;

    /// <summary>
    /// If true and the entity has <see cref="TriggerCounterComponent"/>, the <see cref="Radius"/>
    /// will be scaled to the total amount of triggers in the activated key.
    /// After that it's multiplied by <see cref="RadiusCounterModifier"/> in order to modify the speed of growth.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool RadiusCounterScaling;

    /// <summary>
    /// How much per each trigger do we scale the <see cref="Radius"/> if <see cref="RadiusCounterScaling"/> is enabled.
    /// </summary>
    [DataField, AutoNetworkedField]
    public float RadiusCounterModifier = 0.1f;
}
