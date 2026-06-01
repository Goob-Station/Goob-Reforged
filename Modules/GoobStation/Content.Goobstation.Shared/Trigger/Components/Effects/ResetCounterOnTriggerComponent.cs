using Content.Goobstation.Shared.Trigger.Components.Counter;
using Content.Shared.Trigger.Components.Effects;
using Robust.Shared.GameStates;

namespace Content.Goobstation.Shared.Trigger.Components.Effects;

/// <summary>
/// When triggered, resets a list of keys from their activation counts in <see cref="TriggerCounterComponent"/>.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ResetCounterOnTriggerComponent : BaseXOnTriggerComponent
{
    /// <summary>
    /// Dictionary of trigger keys -> list of keys to reset.
    /// If a key is not specified in <see cref="BaseXOnTriggerComponent.KeysIn"/>, does nothing.
    /// </summary>
    [DataField]
    public Dictionary<string, string[]> Reset = new();
}
