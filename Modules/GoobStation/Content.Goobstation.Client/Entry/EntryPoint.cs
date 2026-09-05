using Content.Goobstation.Client.IoC;
using Robust.Shared.ContentPack;

namespace Content.Goobstation.Client.Entry;

public sealed class EntryPoint : GameClient
{
    public override void Init()
    {
        ClientGoobContentIoc.Register();

        IoCManager.BuildGraph();
        IoCManager.InjectDependencies(this);
    }

    public override void PostInit()
    {
        base.PostInit();
    }
}
