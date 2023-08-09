using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TechMastery.MarketPlace.Infrastructure.IntegrationTests.Base;
using TechMastery.MarketPlace.Persistence;
using TechMastery.MarketPlace.Persistence.Repositories;

namespace TechMastery.MarketPlace.Infrastructure.IntegrationTests
{
    public class DbEmulatorFixture : IAsyncLifetime
    {
        private string _connectionString;
        private TestContainerManager _containerManager;
        private string _dockerContainerId;

        public ApplicationDbContext DbContext { get; private set; }

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
            var portBindings = new List<string> { "5432" };

            _dockerContainerId = await _containerManager.StartContainerAsync("postgres:latest", environmentVariables, 5432, 5432);

            // Wait for PostgreSQL to be ready to accept connections
            await DockerSqlDatabaseUtilities.WaitUntilDatabaseAvailableAsync("5432");

            _connectionString = DockerSqlDatabaseUtilities.GetSqlConnectionString("5432");

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(_connectionString)
                .Options;

            DbContext = new ApplicationDbContext(options);
            DbContext.Database.Migrate();
        }

        public async Task DisposeAsync()
        {
            await _containerManager.StopAndRemoveContainerAsync(_dockerContainerId);
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
