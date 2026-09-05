// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: MIT-WIZARDS

using Content.Shared.Access.Systems;
using JetBrains.Annotations;

namespace Content.Client.Access
{
    [UsedImplicitly]
    public sealed partial class IdCardConsoleSystem : SharedIdCardConsoleSystem
    {
        // one day, maybe bound user interfaces can be shared too.
        // then this doesn't have to be like this.
        // I hate this.
    }
}
