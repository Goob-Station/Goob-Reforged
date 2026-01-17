// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: MIT-WIZARDS

using Content.Lavaland.Client.IoC;
using Robust.Shared.ContentPack;

namespace Content.Lavaland.Client.Entry;

public sealed class EntryPoint : GameClient
{
    public override void Init()
    {
        ContentLavalandClientIoC.Register();

        IoCManager.BuildGraph();
        IoCManager.InjectDependencies(this);
    }
}
