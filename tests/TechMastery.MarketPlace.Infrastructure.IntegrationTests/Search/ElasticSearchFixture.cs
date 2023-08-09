using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Nest;
using TechMastery.MarketPlace.Infrastructure.IntegrationTests.Base;
using Xunit;

namespace TechMastery.MarketPlace.Infrastructure.IntegrationTests
{
    public class ElasticSearchFixture : IAsyncLifetime
    {
        private TestContainerManager? _containerManager;
        private string ?_containerId;

        public IElasticClient? ElasticClient { get; private set; }

        public async Task InitializeAsync()
        {
            _containerManager = new TestContainerManager();
            await _containerManager.StopAndRemoveContainerAsync("docker.elastic.co/elasticsearch/elasticsearch:7.16.3");
            var environmentVariables = new List<string>
            {
                 "discovery.type=single-node",
                "ES_JAVA_OPTS=-Xms512m -Xmx512m" // Adjust these values as needed
            };

            _containerId = await _containerManager.StartContainerAsync("docker.elastic.co/elasticsearch/elasticsearch:7.16.3", environmentVariables, 9200, 9200);

            // Wait for Elasticsearch container to start (you can improve this by checking for readiness)
            await Task.Delay(TimeSpan.FromSeconds(10));

            // Setup ElasticClient
            var elasticSettings = new ConnectionSettings(new Uri($"http://{_containerManager.GetContainerIPAddress(_containerId)}:9200")).DefaultIndex("products");
            ElasticClient = new ElasticClient(elasticSettings);

            // You can include additional setup for other services like Azurite here
        }

        public async Task DisposeAsync()
        {
            // Dispose Elasticsearch container
            await _containerManager.StopAndRemoveContainerAsync(_containerId);

            // Dispose other services here
        }
    }

    [CollectionDefinition("ElasticSearchTestCollection")]
    public class ElasticSearchTestCollection : ICollectionFixture<ElasticSearchFixture>
    {
    }
}
