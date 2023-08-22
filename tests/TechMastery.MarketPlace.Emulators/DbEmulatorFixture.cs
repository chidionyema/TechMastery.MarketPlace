
using Microsoft.EntityFrameworkCore;
using TechMastery.MarketPlace.Persistence;
using TechMastery.MarketPlace.Persistence.Repositories;

namespace TechMastery.MarketPlace.Tests.Emulators
{
    public class DbEmulatorFixture : IAsyncLifetime
    {
        private string? _connectionString;
        private TestContainerManager? _containerManager;
        private string? _containerId;
        public ApplicationDbContext? DbContext { get; private set; }
        private bool setupContext;
        const int port = 5432;
        public DbEmulatorFixture(bool setupDb = true)
        {
            setupContext = setupDb;
        }
        public async Task InitializeAsync()
        {
            _containerManager = new TestContainerManager();

            // Stop and remove the existing container with the same name if it exists
            await _containerManager.StopAndRemoveContainerAsync("postgres:latest");

            var environmentVariables = new List<string>
            {
                "POSTGRES_PASSWORD=#testingDockerPassword#",
                "POSTGRES_USER=postgres",
                "POSTGRES_DB=Billing"
            };
            var portBindings = new List<string> { port.ToString() };

            _containerId = await _containerManager.StartContainerAsync("postgres:latest", environmentVariables, port, port);

            // Wait for PostgreSQL to be ready to accept connections
            await DockerSqlDatabaseUtilities.WaitUntilDatabaseAvailableAsync(port.ToString());

            _connectionString = DockerSqlDatabaseUtilities.GetSqlConnectionString(port.ToString());

            if (setupContext)
            {
                var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseNpgsql(_connectionString)
                    .Options;

                DbContext = new ApplicationDbContext(options);
                DbContext.Database.Migrate();
            }
        }

        public async Task DisposeAsync()
        {
            if (_containerManager != null && !string.IsNullOrEmpty(_containerId))
                await _containerManager.StopAndRemoveContainerAsync(_containerId);

            _containerManager = null;
            DbContext?.Dispose();
        }

        public ProductRepository CreateProductRepository()
        {
            return new ProductRepository(DbContext);
        }

        public CategoryRepository CreateCategoryRepository()
        {
            return new CategoryRepository(DbContext);
        }
    }

    [CollectionDefinition("DbEmulatorCollection")]
    public class DbEmulatorCollection : ICollectionFixture<DbEmulatorFixture>
    {
    }
}
