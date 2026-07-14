using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Goobstation.Shared.Particles;

/// <summary>
/// Does not have server-sided logic. Exist only to allow shared system to create particles without creating separate client systems.
/// </summary>
public abstract class SharedParticleSystem : EntitySystem
{
    public virtual void CreateParticleOnEntity(ProtoId<ParticleEffectPrototype> effectId,
        EntityUid entity,
        Color? colorOverride = null,
        bool attach = true)
    { }

    public virtual void CreateParticleOnCoordinates(ProtoId<ParticleEffectPrototype> effectId,
        MapCoordinates coordinates,
        Color? colorOverride = null)
    { }
}