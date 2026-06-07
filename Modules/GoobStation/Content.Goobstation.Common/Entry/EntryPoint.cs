using Robust.Shared.ContentPack;

namespace Content.GoobStation.Common.Entry;

public sealed class EntryPoint : GameShared
{
    public override void PreInit()
    {
        IoCManager.InjectDependencies(this);
    }
}
