// WMBodyPartComponent.cs
using Content.Shared.Body.Part;
using Content.Shared.Containers.ItemSlots;
using Content.WoundMod.Shared.Surgery.Tools;
using Content.WoundMod.Shared.Targeting;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.WoundMod.Shared.Body.Part;

/// <summary>
/// Wound mod extensions for body parts - integrity, healing, severing, etc.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class WMBodyPartComponent : Component, ISurgeryToolComponent
{
    #region Slot & Parent Info

    [DataField, AutoNetworkedField]
    public BodyPartSlot? ParentSlot;

    [DataField]
    public string SlotId = string.Empty;

    #endregion

    #region Surgery Tool Interface

    [DataField]
    public string ToolName { get; set; } = "A body part";

    [DataField, AutoNetworkedField]
    public bool? Used { get; set; } = null;

    [DataField]
    public float Speed { get; set; } = 1f;

    [DataField]
    public string ContainerName { get; set; } = "part_slot";

    [DataField, AutoNetworkedField]
    public ItemSlot ItemInsertionSlot = new();

    #endregion

    #region Integrity & Damage

    /// <summary>
    /// Minimum integrity threshold for this body part.
    /// </summary>
    [DataField]
    public float MinIntegrity;

    /// <summary>
    /// The total damage required to sever this body part.
    /// </summary>
    [DataField, AutoNetworkedField]
    public float SeverIntegrity = 90;

    /// <summary>
    /// Whether this body part can be severed.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool CanSever = true;

    /// <summary>
    /// Bleeding stacks to give when this body part is severed.
    /// Doubled for vital parts.
    /// </summary>
    [DataField]
    public float SeverBleeding = 4f;

    /// <summary>
    /// Integrity thresholds for different wound states.
    /// </summary>
    [DataField, AutoNetworkedField]
    public Dictionary<TargetIntegrity, float> IntegrityThresholds = new()
    {
        { TargetIntegrity.CriticallyWounded, 90 },
        { TargetIntegrity.HeavilyWounded, 75 },
        { TargetIntegrity.ModeratelyWounded, 60 },
        { TargetIntegrity.SomewhatWounded, 40},
        { TargetIntegrity.LightlyWounded, 20 },
        { TargetIntegrity.Healthy, 10 },
    };

    /// <summary>
    /// On what TargetIntegrity threshold should we re-enable the part.
    /// </summary>
    [DataField, AutoNetworkedField]
    public TargetIntegrity EnableIntegrity = TargetIntegrity.ModeratelyWounded;

    #endregion

    #region Enable/Disable State

    /// <summary>
    /// Whether this body part is currently enabled/functional.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool Enabled = true;

    /// <summary>
    /// Whether this body part can be enabled. Used for non-functional prosthetics.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool CanEnable = true;

    /// <summary>
    /// Whether this body part can attach children.
    /// </summary>
    [DataField]
    public bool CanAttachChildren = true;

    #endregion

    #region Self-Healing

    /// <summary>
    /// How long between self-heal ticks.
    /// </summary>
    [DataField]
    public float HealingTime = 30;

    /// <summary>
    /// Current timer for self-healing.
    /// </summary>
    public float HealingTimer;

    /// <summary>
    /// How much to heal per tick.
    /// </summary>
    [DataField]
    public float SelfHealingAmount = 5;

    #endregion

    #region Appearance

    /// <summary>
    /// Current species. Dictates body part sprites.
    /// </summary>
    [DataField, AutoNetworkedField]
    public string Species { get; set; } = "";

    /// <summary>
    /// The ID of the base layer for this body part.
    /// </summary>
    [DataField, AutoNetworkedField]
    public string? BaseLayerId;

    #endregion

    #region Component Management

    /// <summary>
    /// When attached, ensure these components on the entity.
    /// </summary>
    [DataField, AlwaysPushInheritance]
    public ComponentRegistry? OnAdd;

    /// <summary>
    /// When removed, ensure these components on the entity.
    /// </summary>
    [DataField, AlwaysPushInheritance]
    public ComponentRegistry? OnRemove;

    #endregion
}
