using Content.WoundMod.Shared.Surgery.Tools;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.WoundMod.Shared.Body.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class WMOrganComponent : Component, ISurgeryToolComponent
{
    #region Original Body Tracking

    /// <summary>
    /// Relevant body this organ originally belonged to.
    /// Used for tracking organ history and transplants.
    /// </summary>
    [DataField]
    public EntityUid? OriginalBody;

    #endregion

    #region Slot Information

    /// <summary>
    /// Shitcodey solution to not being able to know what name corresponds
    /// to each organ's slot ID without referencing the prototype or hardcoding.
    /// </summary>
    [DataField]
    public string SlotId = string.Empty;

    #endregion

    #region Surgery Tool Interface

    [DataField]
    public string ToolName { get; set; } = "An organ";

    [DataField]
    public float Speed { get; set; } = 1f;

    /// <summary>
    /// If true, the organ will not heal an entity when transplanted into them.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool? Used { get; set; }

    #endregion

    #region Component Management

    /// <summary>
    /// When attached, the organ will ensure these components on the entity,
    /// and delete them on removal.
    /// </summary>
    [DataField, AlwaysPushInheritance]
    public ComponentRegistry? OnAdd;

    /// <summary>
    /// When removed, the organ will ensure these components on the entity,
    /// and delete them on insertion.
    /// </summary>
    [DataField, AlwaysPushInheritance]
    public ComponentRegistry? OnRemove;

    #endregion

    #region Enable/Disable State

    /// <summary>
    /// Is this organ working or not?
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool Enabled = true;

    /// <summary>
    /// Can this organ be enabled or disabled?
    /// Used mostly for prop, damaged or useless organs.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool CanEnable = true;

    #endregion
}
