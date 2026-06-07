using Content.Benchmarks;
using Content.IntegrationTests;

namespace Content.GoobStation.IntegrationTests;

[SetUpFixture]
public sealed class GoobStationPoolManagerTestEventHandler
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
