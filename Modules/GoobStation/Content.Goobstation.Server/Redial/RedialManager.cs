using Content.Goobstation.Shared.Redial;
using Robust.Shared.Network;

namespace Content.Goobstation.Server.Redial;

public sealed class RedialManager : SharedRedialManager
{
    public override void Initialize()
    {
        NetManager.RegisterNetMessage<MsgRedial>();
    }

    public void Redial(INetChannel channel, string address)
    {
        if (!channel.IsConnected)
            return;

        var msg = new MsgRedial
        {
            Address = address,
        };

        channel.SendMessage(msg);
    }
}
