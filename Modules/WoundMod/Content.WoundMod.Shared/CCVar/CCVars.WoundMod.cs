using Robust.Shared.Configuration;

namespace Content.WoundMod.Shared.CCVar;

[CVarDefs]
public sealed class WoundModCVars
{
    public static readonly CVarDef<bool> CanOperateOnSelf =
        CVarDef.Create("surgery.can_operate_on_self", true, CVar.SERVERONLY);
}
