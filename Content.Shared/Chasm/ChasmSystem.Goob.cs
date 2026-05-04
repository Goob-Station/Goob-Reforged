using Content.Goobstation.Common.Chasm;

namespace Content.Shared.Chasm;

public sealed partial class ChasmSystem
{
    private void StopFalling(Entity<ChasmFallingComponent> tripper)
    {
        RemCompDeferred(tripper.Owner, tripper.Comp);

        if (!TryComp(tripper.Comp.FallChasm, out ChasmComponent? chasmComp))
            return;

        chasmComp.Falling.Remove(tripper.Owner);
        var beforeEv = new BeforeChasmFallEvent(tripper.Comp.FallChasm);
        RaiseLocalEvent(tripper.Owner, ref beforeEv);
        if (beforeEv.Cancelled)
            return;

        var ev = new ChasmFallEffectsEvent(tripper.Owner);
        RaiseLocalEvent(tripper.Comp.FallChasm.Value, ref ev);
    }

    public void RemoveFallingEnt(Entity<ChasmComponent> chasm, EntityUid falling)
    {
        chasm.Comp.Falling.Remove(falling); // I just don't want to ruin component access for now
    }
}
