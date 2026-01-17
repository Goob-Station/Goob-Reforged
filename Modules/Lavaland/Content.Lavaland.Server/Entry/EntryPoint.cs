using Content.Lavaland.Server.IoC;
using Robust.Shared.ContentPack;

namespace Content.Lavaland.Server.Entry;

public sealed class EntryPoint : GameServer
{
    public override void Init()
    {
        base.Init();

        ServerLavalandContentIoC.Register();
        IoCManager.BuildGraph();
    }
}
