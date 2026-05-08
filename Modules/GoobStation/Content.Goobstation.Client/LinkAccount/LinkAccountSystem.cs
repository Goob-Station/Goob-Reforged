using Content.Goobstation.Common.LinkAccount;

namespace Content.Goobstation.Client.LinkAccount;

public sealed class LinkAccountSystem : EntitySystem
{
    public event Action<SharedRMCDisplayLobbyMessageEvent>? LobbyMessageReceived;

    public override void Initialize()
    {
        SubscribeNetworkEvent<SharedRMCDisplayLobbyMessageEvent>(OnDisplayLobbyMessage);
    }

    private void OnDisplayLobbyMessage(SharedRMCDisplayLobbyMessageEvent ev)
    {
        LobbyMessageReceived?.Invoke(ev);
    }
}
