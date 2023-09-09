
namespace TechMastery.MarketPlace.Tests.Emulators
{
    public class MockS3Fixture : IAsyncLifetime
    {
        private TestContainerManager? _containerManager;
        private string? _containerId;

        public async Task InitializeAsync()
        {
            _containerManager = new TestContainerManager();
            await _containerManager.StopAndRemoveContainerAsync("localstack/localstack");

            var environmentVariables = new List<string>
            {
                "SERVICES=s3"
            };

            _containerId = await _containerManager.StartContainerAsync("localstack/localstack", environmentVariables, 4572, 4572);

            // Wait for LocalStack to start (improve this by checking for readiness, maybe through AWS CLI commands)
            await Task.Delay(TimeSpan.FromSeconds(10));
        }

        public async Task DisposeAsync()
        {
            // Dispose Elasticsearch container
            if (_containerManager != null && !string.IsNullOrEmpty(_containerId))
                await _containerManager.StopAndRemoveContainerAsync(_containerId);

            _containerManager = null;
        }

    }

    [CollectionDefinition("MockS3Collection")]
    public class MockS3Collection : ICollectionFixture<MockS3Fixture>
    {
    }
}
