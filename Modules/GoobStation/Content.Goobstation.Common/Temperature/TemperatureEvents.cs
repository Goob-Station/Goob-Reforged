// SPDX-FileCopyrightText: 2026 Goob Station Contributors
//
// SPDX-License-Identifier: MPL-2.0

namespace Content.Goobstation.Common.Temperature;

[ByRefEvent]
public record struct TemperatureDamageThresholdOverrideEvent(float HeatDamageThreshold, float ColdDamageThreshold);
