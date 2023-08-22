namespace TechMastery.MarketPlace.Tests.Emulators;
public class MockSQSFixture : IAsyncLifetime
{
    private TestContainerManager? _containerManager;
    private string? _containerId;

    public async Task InitializeAsync()
    {
        _containerManager = new TestContainerManager();
        await _containerManager.StopAndRemoveContainerAsync("localstack/localstack");

        var environmentVariables = new List<string>
        {
            "SERVICES=sqs"
        };

        _containerId = await _containerManager.StartContainerAsync("localstack/localstack", environmentVariables, 4566, 4566);

        // Wait for LocalStack to start (improve this by checking for readiness)
        await Task.Delay(TimeSpan.FromSeconds(10));
    }

    public async Task DisposeAsync()
    {
        if (_containerManager != null && !string.IsNullOrEmpty(_containerId))
            await _containerManager.StopAndRemoveContainerAsync(_containerId);

        _containerManager = null;
    }
}

[CollectionDefinition("MockSQSCollection")]
public class MockSQSCollection : ICollectionFixture<MockSQSFixture>
{
}
