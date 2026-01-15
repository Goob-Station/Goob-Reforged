// SPDX-FileCopyrightText: 2025 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using Content.Goobstation.Server.Database;

namespace Content.Goobstation.Server.IoC;

internal static class ServerGoobContentIoC
{
    internal static void Register()
    {
        var instance = IoCManager.Instance!;
        instance.Register<IGoobstationDbManager, GoobstationDbManager>();
    }
}
