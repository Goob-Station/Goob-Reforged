using Content.Template.Server.IoC;
using Robust.Shared.ContentPack;

namespace Content.Template.Server.Entry;

public sealed class EntryPoint : GameServer
{
    public override void Init()
    {
        base.Init();

        ServerTemplateContentIoC.Register();
        IoCManager.BuildGraph();
    }
}
