using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Goobstation.Common.Damage;

/// <summary>
/// Goobstation
/// Make entity destroy other entities on interaction
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class DestroyInteractingsComponent : Component
{
    #region Logic Handlers
    /// <summary>
    /// Whitelist entities that this entity can destroy
    /// </summary>
    [DataField]
    public EntityWhitelist? DestroyWhitelist;

    /// <summary>
    /// Marks entities that can not be destroed by this entity
    /// </summary>
    [DataField]
    public EntityWhitelist? DestroyBlacklist;

    /// <summary>
    /// Should entity count contacts as interaction
    /// </summary>
    [DataField]
    public bool RespectContacts = false;

    /// <summary>
    /// Should entity try interaction with item in hand first
    /// </summary>
    [DataField]
    public bool RespectHandInteraction = true;

    #endregion

    #region Visuals and Sounds
    /// <summary>
    /// Sound that should be played on destruction coordinates
    /// </summary>
    [DataField]
    public SoundSpecifier? DestructionSound;

    /// <summary>
    /// Entity that should instead of destroyed entity
    /// </summary>
    [DataField(customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string SpawnOnDestruction = string.Empty;

    //TODO:Particles
    #endregion
}