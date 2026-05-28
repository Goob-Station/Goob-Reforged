using Robust.Shared.GameStates;

namespace Content.Goobstation.Shared.Fishing.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(fieldDeltas:true)]
public sealed partial class FishingLureComponent : Component
{
    [DataField, AutoNetworkedField]
    public EntityUid FishingRod;

    [DataField, AutoNetworkedField]
    public EntityUid? AttachedEntity;

    [ViewVariables]
    public TimeSpan NextUpdate;

    [DataField]
    public float UpdateInterval = 1f;
}
