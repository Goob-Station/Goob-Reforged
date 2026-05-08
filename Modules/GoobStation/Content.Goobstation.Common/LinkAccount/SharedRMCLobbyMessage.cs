using Robust.Shared.Serialization;

namespace Content.Goobstation.Common.LinkAccount;

[Serializable, NetSerializable]
public sealed record SharedRMCLobbyMessage(string Message)
{
    public const int CharacterLimit = 40;
}
