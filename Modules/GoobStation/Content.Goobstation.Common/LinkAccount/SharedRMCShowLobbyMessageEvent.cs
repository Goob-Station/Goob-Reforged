using Robust.Shared.Serialization;

namespace Content.Goobstation.Common.LinkAccount;

[Serializable, NetSerializable]
public sealed class SharedRMCShowLobbyMessageEvent(string text) : EntityEventArgs
{
    public readonly string Text = text;
}
