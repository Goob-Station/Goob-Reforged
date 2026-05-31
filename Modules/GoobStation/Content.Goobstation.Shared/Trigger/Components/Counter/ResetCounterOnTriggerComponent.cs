using Content.Shared.Trigger.Components.Effects;
using Robust.Shared.GameStates;

namespace Content.Goobstation.Shared.Trigger.Components.Counter;

/// <summary>
/// Resets the amount of triggers for some key when a trigger is activated.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ResetCounterOnTriggerComponent : BaseXOnTriggerComponent
{
    /// <summary>
    /// The keys to reset. If not specified, will reset all of them to 0.
    /// </summary>
    [DataField, AutoNetworkedField]
    public HashSet<string>? Keys;
}
