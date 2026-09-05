using Content.Goobstation.Server.Database;
using Content.Goobstation.Server.Redial;

namespace Content.Goobstation.Server.IoC;

internal static class ServerGoobContentIoC
{
    internal static void Register()
    {
        var instance = IoCManager.Instance!;
        instance.Register<IGoobstationDbManager, GoobstationDbManager>();
        instance.Register<RedialManager>();
    }
}
