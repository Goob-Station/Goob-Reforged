using Robust.Shared.Serialization;

namespace Content.Goobstation.Common.LinkAccount;

[Serializable, NetSerializable]
public sealed class SharedRMCDisplayLobbyMessageEvent(string message, string user) : EntityEventArgs
{
    public readonly string Message = message;
    public readonly string User = user;
}
