namespace Content.Goobstation.Common.Temperature;

[ByRefEvent]
public record struct TemperatureDamageThresholdOverrideEvent(float HeatDamageThreshold, float ColdDamageThreshold);
