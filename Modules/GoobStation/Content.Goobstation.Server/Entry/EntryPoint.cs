using Content.Goobstation.Server.Database;
using Content.Goobstation.Server.IoC;
using Robust.Shared.ContentPack;
using Robust.Shared.Timing;

namespace Content.Goobstation.Server.Entry;

public sealed class EntryPoint : GameServer
{
    private IGoobstationDbManager _db = default!;

    public override void Init()
    {
        base.Init();
        ServerGoobContentIoC.Register();
        IoCManager.BuildGraph();

        _db = IoCManager.Resolve<IGoobstationDbManager>();
        _db.Init();
    }

    public override void PostInit()
    {
        base.PostInit();
    }

    public override void Update(ModUpdateLevel level, FrameEventArgs frameEventArgs)
    {
        base.Update(level, frameEventArgs);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _db.Shutdown();
    }
}
