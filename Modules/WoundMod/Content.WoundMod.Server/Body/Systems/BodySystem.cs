using Content.Shared.Humanoid;
using Content.WoundMod.Shared.Body.Part;
using Content.WoundMod.Shared.Body.Systems;

namespace Content.WoundMod.Server.Body.Systems;

public sealed class WMBodySystem : SharedWMBodySystem
{
    protected override void ApplyPartMarkings(EntityUid target, BodyPartAppearanceComponent component)
    {
    }

    protected override void RemoveBodyMarkings(EntityUid target,
        BodyPartAppearanceComponent partAppearance,
        HumanoidAppearanceComponent bodyAppearance)
    {
    }
};
