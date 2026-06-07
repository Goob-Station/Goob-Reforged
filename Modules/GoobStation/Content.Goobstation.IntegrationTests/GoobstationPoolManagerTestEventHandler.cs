using Content.Benchmarks;
using Content.IntegrationTests;

namespace Content.Goobstation.IntegrationTests;

[SetUpFixture]
public sealed class GoobstationPoolManagerTestEventHandler
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
