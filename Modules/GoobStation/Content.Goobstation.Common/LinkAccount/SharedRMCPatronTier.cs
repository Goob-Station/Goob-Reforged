using Robust.Shared.Serialization;

namespace Content.Goobstation.Common.LinkAccount;

[Serializable, NetSerializable]
public sealed record SharedRMCPatronTier(
    bool ShowOnCredits,
    bool GhostColor,
    bool LobbyMessage,
    bool RoundEndShoutout,
    string Tier,
    string? Icon
);
