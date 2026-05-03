using Content.Template.Client.IoC;
using Robust.Shared.ContentPack;

namespace Content.Template.Client.Entry;

public sealed class EntryPoint : GameClient
{
    public override void Init()
    {
        ContentTemplateClientIoC.Register();

        IoCManager.BuildGraph();
        IoCManager.InjectDependencies(this);
    }
}
