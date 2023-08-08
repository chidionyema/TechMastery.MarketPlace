using System.Diagnostics;
using System.Threading.Tasks;
using Nest;
using Xunit;

namespace TechMastery.MarketPlace.Infrastructure.IntegrationTests
{
    public class ElasticSearchFixture : IAsyncLifetime
    {
        private Process? _elasticsearchProcess;

        public IElasticClient? ElasticClient { get; private set; }

        public async Task InitializeAsync()
        {
            // Start Elasticsearch container
            var elasticsearchStartInfo = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = "run -d -p 9200:9200 docker.elastic.co/elasticsearch/elasticsearch:7.16.3",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            _elasticsearchProcess = Process.Start(elasticsearchStartInfo);

            // Wait for Elasticsearch container to start (you can improve this by checking for readiness)
            await Task.Delay(TimeSpan.FromSeconds(10));

            // Setup ElasticClient
            var elasticSettings = new ConnectionSettings(new Uri("http://localhost:9200")).DefaultIndex("products");
            ElasticClient = new ElasticClient(elasticSettings);

            // You can include additional setup for other services like Azurite here
        }

        public async Task DisposeAsync()
        {
            // Dispose Elasticsearch
            if (_elasticsearchProcess != null && !_elasticsearchProcess.HasExited)
            {
                _elasticsearchProcess.Kill();
                await _elasticsearchProcess.WaitForExitAsync();
            }

            // Dispose other services here
        }
    }

    [CollectionDefinition("ElasticSearchTestCollection")]
    public class ElasticSearchTestCollection : ICollectionFixture<ElasticSearchFixture>
    {
    }
}
