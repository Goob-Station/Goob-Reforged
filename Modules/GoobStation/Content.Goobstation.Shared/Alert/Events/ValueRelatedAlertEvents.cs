using Content.Shared.Alert;

namespace Content.Goobstation.Shared.Alert.Events;

[ByRefEvent]
public record struct GetValueRelatedAlertValuesEvent(AlertPrototype Alert, float? MaxValue = null, float? CurrentValue = null, float MinValue = 0)
{
    public readonly bool Handled => MaxValue.HasValue && CurrentValue.HasValue;
}
