using Content.Benchmarks;
using Content.IntegrationTests;

namespace Content.Lavaland.IntegrationTests;

[SetUpFixture]
public sealed class LavalandPoolManagerTestEventHandler
{
    [OneTimeSetUp]
    public void Setup()
    {
        IntegrationTestHelpers.ChangeRootDir("../../../");
        PoolManagerHelpers.Setup();
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        PoolManager.Shutdown();
    }
}
