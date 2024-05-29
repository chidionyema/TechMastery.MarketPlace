using Nest;

namespace TechMastery.MarketPlace.Tests.Emulators
{
    public class ElasticSearchFixture : IAsyncLifetime
    {
        private TestContainerManager? _containerManager;
        public IElasticClient? ElasticClient { get; private set; }
        private string ?_containerId;
        const string esVersion = "docker.elastic.co/elasticsearch/elasticsearch:7.16.3";
        const string discoveryType = "discovery.type=single-node";
        const string esOptions = "ES_JAVA_OPTS=-Xms512m -Xmx512m";
        const int port = 9200;
        
        public async Task InitializeAsync()
        {
            _containerManager = new TestContainerManager();
            await _containerManager.StopAndRemoveContainerAsync(esVersion);

            _containerId = await _containerManager.StartContainerAsync(esVersion, new List<string>
            {
               discoveryType,
               esOptions
            }, port, port);

            await Task.Delay(TimeSpan.FromSeconds(10));

            var elasticSettings = new ConnectionSettings(new Uri($"http://{_containerManager.GetContainerIPAddress(_containerId)}:9200")).DefaultIndex("products");
            ElasticClient = new ElasticClient(elasticSettings);

        }

        public async Task DisposeAsync()
        {
            if (_containerManager != null && !string.IsNullOrEmpty(_containerId))
                await _containerManager.StopAndRemoveContainerAsync(_containerId);

            _containerManager = null; 
        }
    }

    [CollectionDefinition("ElasticSearchTestCollection")]
    public class ElasticSearchTestCollection : ICollectionFixture<ElasticSearchFixture>
    {
    }
}
