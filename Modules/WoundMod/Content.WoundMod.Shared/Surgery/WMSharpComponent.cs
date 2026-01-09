namespace Content.WoundMod.Shared.Surgery;

[RegisterComponent]
public sealed class WMSharpComponent : Component
{
    [DataField]
    public bool HadSurgeryTool;
    [DataField]
    public bool HadScalpel;
    [DataField]
    public bool HadBoneSaw;
}
