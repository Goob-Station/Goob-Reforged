using Content.IntegrationTests.Fixtures;
using Content.IntegrationTests.Fixtures.Attributes;
using Content.Lavaland.Shared.EntityShapes;
using Content.Lavaland.Shared.EntityShapes.Shapes;

namespace Content.Lavaland.IntegrationTests.Tests.EntityShapes;

[TestFixture]
public sealed partial class EntityShapesTest : GameTest
{
    [SidedDependency(Side.Server)] private EntityShapeSystem _shape = default!;

    [Test]
    public async Task ShapeSpawningTest()
    {
        await Pair.CreateTestMap();
        var map = TestMap!.CGridCoords;

        var shape = new BoxEntityShape
        {
            DefaultSize = 5,
            Hollow = false,
        };

        await Pair.Server.WaitPost(() =>
        {
            _shape.SpawnEntityShape(shape, map, null, out var spawned);
            Assert.That(spawned, Has.Count.EqualTo(25));
        });
    }
}
