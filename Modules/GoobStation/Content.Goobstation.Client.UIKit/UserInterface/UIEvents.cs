using Robust.Shared.Map;

namespace Content.GoobStation.Client.UIKit.UserInterface;

[ByRefEvent]
public readonly record struct ButtonTagPressedEvent(string Id, NetEntity User, NetCoordinates Coords);
