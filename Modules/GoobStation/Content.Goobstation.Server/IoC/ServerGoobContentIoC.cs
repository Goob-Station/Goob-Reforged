// SPDX-FileCopyrightText: 2025 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using Content.GoobStation.Server.Database;

namespace Content.GoobStation.Server.IoC;

internal static class ServerGoobContentIoC
{
    internal static void Register()
    {
        var instance = IoCManager.Instance!;
        instance.Register<IGoobStationDbManager, GoobStationDbManager>();
    }
}
