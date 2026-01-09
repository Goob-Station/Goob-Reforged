using Robust.Shared.Serialization;

namespace Content.WoundMod.Shared.Eye;

[Flags]
[FlagsFor(typeof(VisibilityMaskLayer))]
public enum WMVisibilityFlags : int
{
    Abductor  = 1 << 3, // Shitmed Change - Starlight Abductor
}
