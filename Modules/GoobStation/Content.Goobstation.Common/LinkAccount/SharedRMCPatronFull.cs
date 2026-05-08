using Robust.Shared.Serialization;

namespace Content.Goobstation.Common.LinkAccount;

[Serializable, NetSerializable]
public sealed record SharedRMCPatronFull(
    SharedRMCPatronTier? Tier,
    bool Linked,
    Color? GhostColor,
    SharedRMCLobbyMessage? LobbyMessage,
    SharedRMCRoundEndShoutouts? RoundEndShoutout
);
