using Content.Shared.Trigger.Components.Effects;
using Robust.Shared.Audio;

namespace Content.Goobstation.Server.Trigger;

[RegisterComponent]
public sealed partial class AnnounceOnTriggerComponent : BaseXOnTriggerComponent
{
    /// <summary>
    /// Locale id of the announcement message.
    /// </summary>
    [DataField(required: true)]
    public LocId Message = string.Empty;

    /// <summary>
    /// Locale id of the announcement's sender, defaults to Central Command.
    /// </summary>
    [DataField]
    public LocId? Sender;

    /// <summary>
    /// Sound override for the announcement.
    /// </summary>
    [DataField]
    public SoundSpecifier? Sound;

    /// <summary>
    /// Color override for the announcement.
    /// </summary>
    [DataField]
    public Color? Color;
}
