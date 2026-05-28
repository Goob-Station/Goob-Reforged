using Robust.Shared.ContentPack;

namespace Content.Modules.Client;

public sealed class EntryPoint : GameClient
{
    public override void Init()
    {
        base.Init();
        IoCManager.BuildGraph();
    }
}
