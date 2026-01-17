using Content.Lavaland.Shared.IoC;
using Robust.Shared.ContentPack;

namespace Content.Lavaland.Shared.Entry;

public sealed class EntryPoint : GameShared
{
    public override void PreInit()
    {
        IoCManager.InjectDependencies(this);
        SharedLavalandContentIoC.Register();
    }
}
