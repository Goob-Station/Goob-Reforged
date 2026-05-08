using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Goobstation.Common.LinkAccount;

public sealed class LinkAccountRequestMsg : NetMessage
{
    public override MsgGroups MsgGroup => MsgGroups.Core;

    public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
    {
    }

    public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
    {
    }
}
