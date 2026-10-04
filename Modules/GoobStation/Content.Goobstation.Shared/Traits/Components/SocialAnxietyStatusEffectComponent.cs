namespace Content.Goobstation.Shared.Traits.Components;

/// <summary>
/// Marks a status effect that stuns and displays a popup on intimate interaction such as a hug.
/// </summary>
[RegisterComponent]
public sealed partial class SocialAnxietyStatusEffectComponent : Component
{
    [DataField]
    public int DownedTime = 5;

    [DataField]
    public LocId? DownedPopup = "social-anxiety-hugged";
}
