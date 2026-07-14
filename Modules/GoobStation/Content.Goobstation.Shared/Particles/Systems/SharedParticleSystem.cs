using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Goobstation.Shared.Particles;

public abstract class SharedParticleSystem : EntitySystem
{
    public virtual void CreateParticleOnCoords(ProtoId<ParticleEffectPrototype> effectId,
        MapCoordinates coordinates,
        Color? colorOverride = null)
    { }
}