using System.Linq;
using System.Numerics;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Lavaland.Shared.EntityShapes.Shapes;

public sealed partial class BoxEntityShape : EntityShape
{
    [DataField]
    public bool Hollow;

    protected override List<Vector2> GetShapeImplementation(IRobustRandom rand, IPrototypeManager proto)
    {
        return ShapeHelpers.MakeBox(Offset, Size, Hollow, StepSize).ToList();
    }
}
