// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using Content.Goobstation.Shared.Supermatter.Components;
using Robust.Shared.Serialization;

namespace Content.Goobstation.Shared.Supermatter.Systems;

public abstract class SharedSupermatterSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SupermatterComponent, ComponentStartup>(OnSupermatterStartup);
    }

    public enum SuperMatterSound : sbyte
    {
        Aggressive = 0,
        Delam = 1
    }

    public enum DelamType : sbyte
    {
        Explosion = 0,
        Singulo = 1,
        Tesla = 2,
        Cascade = 3
    }
    #region Getters/Setters

    public static void OnSupermatterStartup(EntityUid uid, SupermatterComponent comp, ComponentStartup args)
    {
    }

    #endregion Getters/Setters

}
