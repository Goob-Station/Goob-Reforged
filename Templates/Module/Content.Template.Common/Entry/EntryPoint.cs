using Robust.Shared.ContentPack;

namespace Content.Template.Common.Entry;

public sealed class EntryPoint : GameShared
{
    public override void PreInit()
    {
        IoCManager.InjectDependencies(this);
    }
}
