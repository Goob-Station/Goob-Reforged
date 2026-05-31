using Content.Shared.Trigger.Systems;
using Robust.Shared.GameStates;

namespace Content.Goobstation.Shared.Trigger.Components.Effects;

/// <summary>
/// Activates a list of trigger keys when it's triggered by some other specific trigger key.
/// Use this in order to easily multiply a trigger.
/// </summary>
/// <remarks>
/// Use this only if you know what you're doing, because this is basically a dangerous workaround.
/// </remarks>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class TriggerOnTriggerComponent : Component
{
    [DataField, AutoNetworkedField]
    public string KeyIn = TriggerSystem.DefaultTriggerKey;

    [DataField(required: true), AutoNetworkedField]
    public HashSet<string> KeysOut;
}
