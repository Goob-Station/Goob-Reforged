using Robust.Shared.Serialization;

namespace Content.Goobstation.Common.LinkAccount;

[Serializable, NetSerializable]
public sealed record SharedRMCRoundEndShoutouts(string? NT)
{
    public const int CharacterLimit = 50;
}
