using Robust.Shared.ContentPack;

namespace Content.Modules.Server;

public sealed class EntryPoint : GameServer
{
    public override void Init()
    {
        base.Init();
        IoCManager.BuildGraph();
    }
}
