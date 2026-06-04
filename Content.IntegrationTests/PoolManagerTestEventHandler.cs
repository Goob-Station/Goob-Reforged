namespace Content.IntegrationTests;

[SetUpFixture]
public sealed class PoolManagerTestEventHandler
{
    // Goobstation edit - migrated the code to PoolManagerHelpers

    [OneTimeSetUp]
    public void Setup()
    {
        PoolManagerHelpers.Setup();
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        PoolManager.Shutdown();
    }
}
