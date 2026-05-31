using Content.Goobstation.Shared.Redial;
using Robust.Client;

namespace Content.Goobstation.Client.Redial;

public sealed partial class RedialManager : SharedRedialManager
{
    [Dependency] private IGameController _gameController = default!;

    public override void Initialize()
    {
        NetManager.RegisterNetMessage<MsgRedial>(RedialOnMessage);
    }

    private void RedialOnMessage(MsgRedial message)
        => _gameController.Redial(message.Address);
}
