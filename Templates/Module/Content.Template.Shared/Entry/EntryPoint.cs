using Content.Template.Shared.IoC;
using Robust.Shared.ContentPack;

namespace Content.Template.Shared.Entry;

public sealed class EntryPoint : GameShared
{
    public override void PreInit()
    {
        IoCManager.InjectDependencies(this);
        SharedTemplateContentIoC.Register();
    }
}
