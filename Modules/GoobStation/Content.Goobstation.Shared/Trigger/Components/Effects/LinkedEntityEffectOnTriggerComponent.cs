using Content.Shared.EntityEffects;
using Content.Shared.Teleportation.Components;
using Content.Shared.Trigger.Components.Effects;
using Robust.Shared.GameStates;

namespace Content.Goobstation.Shared.Trigger.Components.Effects;

/// <summary>
/// Applies an entity effect to all entities in the <see cref="LinkedEntityComponent"/>
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class LinkedEntityEffectOnTriggerComponent : BaseXOnTriggerComponent
{
    /// <summary>
    /// The effects to apply.
    /// </summary>
    [DataField]
    public EntityEffect[] Effects;

    /// <summary>
    /// Optional scale multiplier for the effects.
    /// </summary>
    [DataField, AutoNetworkedField]
    public float Scale = 1f;
}
