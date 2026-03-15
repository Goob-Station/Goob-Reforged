// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

namespace Content.Goobstation.Shared.Supermatter.Systems;

public abstract class SharedSupermatterSystem : EntitySystem
{
    public enum DelamType
    {
        Explosion = 0,
        Singulo = 1,
        Tesla = 2,
        Cascade = 3
    }

}
