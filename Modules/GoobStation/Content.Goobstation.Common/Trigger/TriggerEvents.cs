namespace Content.Goobstation.Common.Trigger;

[ByRefEvent]
public record struct AfterTriggerEvent(EntityUid? User = null, string? Key = null, bool Predicted = true, bool Handled = false);
