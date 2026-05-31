using Content.Shared.Trigger.Systems;
using Robust.Shared.GameStates;

namespace Content.Goobstation.Shared.Trigger.Components.Counter;

/// <summary>
/// Counts the total amount of triggers that this entity had in its entire lifetime.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class TriggerCounterComponent : Component
{
    /// <summary>
    /// Keys to count when they are triggered.
    /// </summary>
    [DataField(required: true)]
    public HashSet<string> Keys;

    /// <summary>
    /// Amount of triggers per each trigger key.
    /// </summary>
    [ViewVariables, AutoNetworkedField]
    public Dictionary<string, int> Counts = new();
}
