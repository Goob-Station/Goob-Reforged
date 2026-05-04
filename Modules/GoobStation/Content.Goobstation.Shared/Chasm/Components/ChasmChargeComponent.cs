using Content.Shared.Charges.Components;
using Content.Shared.Whitelist;
using Robust.Shared.GameStates;

namespace Content.Goobstation.Shared.Chasm.Components;

/// <summary>
/// Adds more charges to <see cref="LimitedChargesComponent"/> when an entity falls into the chasm.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ChasmChargeComponent : Component
{
    /// <summary>
    /// The default amount of charges to give when an entity falls into the chasm.
    /// </summary>
    [DataField]
    public int DefaultCharges = 1;

    /// <summary>
    /// If true, the tripped entity will be able to pass all <see cref="SpecialCharges"/> entires
    /// and possibly add even more charges if it passes multiple whitelists.
    /// </summary>
    [DataField]
    public bool CanMultipleSpecials;

    /// <summary>
    /// Dictionary of Charge amounts and Whitelists for tripped entities.
    /// If a whitelist gets passed, the key charge amount is used instead of <see cref="DefaultCharges"/>.
    /// </summary>
    [DataField]
    public Dictionary<int, EntityWhitelist>? SpecialCharges;
}
