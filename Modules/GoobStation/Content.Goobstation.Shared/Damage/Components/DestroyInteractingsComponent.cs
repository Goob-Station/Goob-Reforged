using Content.Goobstation.Shared.Particles;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Goobstation.Common.Damage;

/// <summary>
/// Make entity destroy other entities on interaction
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class DestroyInteractingsComponent : Component
{
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

    /// <summary>
    /// The destroy on collide fixture 
    /// </summary>
    [DataField]
    public string FixtureId = "destroyer";

    /// <summary>
    /// Sound that should be played on destruction coordinates
    /// </summary>
    [DataField]
    public SoundSpecifier? DestructionSound;

    /// <summary>
    /// Entity that should instead of destroyed entity
    /// </summary>
    [DataField]
    public EntProtoId? SpawnOnDestruction;

    /// <summary>
    /// Particle that should appear on entity destruction
    /// </summary>
    [DataField]
    public ProtoId<ParticleEffectPrototype>? Particle;

    //TODO:Particles
}