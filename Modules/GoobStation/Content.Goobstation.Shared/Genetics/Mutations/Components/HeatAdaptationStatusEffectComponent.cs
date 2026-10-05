// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

using Robust.Shared.GameStates;

namespace Content.Goobstation.Shared.Genetics.Mutations.Components;

[RegisterComponent]
public sealed partial class HeatAdaptationStatusEffectComponent : Component
{
    [DataField]
    public float? ColdDamageThreshold;

    [DataField]
    public float? HeatDamageThreshold;
}
