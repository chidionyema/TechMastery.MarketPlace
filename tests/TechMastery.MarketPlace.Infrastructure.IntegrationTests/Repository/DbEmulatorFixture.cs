using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using TechMastery.MarketPlace.Persistence;
using TechMastery.MarketPlace.Persistence.Repositories;

namespace TechMastery.MarketPlace.Infrastructure.IntegrationTests
{
    public class DbEmulatorFixture : IAsyncLifetime
    {
        private readonly string _connectionString;
        private readonly DockerClient _dockerClient;
        private readonly string _dockerContainerId;
        public ApplicationDbContext DbContext { get; private set; }

        public DbEmulatorFixture()
        {
            _dockerClient = new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock"))
                .CreateClient();

            // Stop and remove the existing container with the same name if it exists
            StopAndRemoveExistingContainer();

            var createContainerResponse = _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Image = "postgres:latest",
                Name = "integration-test-db",
                Env = new[] { "POSTGRES_PASSWORD=#testingDockerPassword#", "POSTGRES_USER=postgres", "POSTGRES_DB=Billing" },
                ExposedPorts = new Dictionary<string, EmptyStruct>
                {
                    ["5432/tcp"] = default
                },
                HostConfig = new HostConfig
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        ["5432/tcp"] = new List<PortBinding>
                        {
                            new PortBinding
                            {
                                HostPort = "5432"
                            }
                        }
                    }
                }
            }).Result; // Wait for the container creation to complete

            _dockerContainerId = createContainerResponse.ID;

            _dockerClient.Containers.StartContainerAsync(_dockerContainerId, null).Wait();

            // Wait for PostgreSQL to be ready to accept connections
            WaitForPostgresToBeReady();

            _connectionString = DockerSqlDatabaseUtilities.GetSqlConnectionString("5432");

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(_connectionString)
                .Options;

            DbContext = new ApplicationDbContext(options);
            DbContext.Database.Migrate();
        }

        // Rest of the code...

        private void StopAndRemoveExistingContainer()
        {
            try
            {
                _dockerClient.Containers.StopContainerAsync("integration-test-db", new ContainerStopParameters()).Wait();
                _dockerClient.Containers.RemoveContainerAsync("integration-test-db", new ContainerRemoveParameters()).Wait();
            }
            catch
            {
                // Ignore errors if the container doesn't exist or cannot be stopped/removed
            }
        }

        private void WaitForPostgresToBeReady()
        {
            using var connection = new NpgsqlConnection(DockerSqlDatabaseUtilities.GetSqlConnectionString("5432"));
            const int maxRetries = 30;
            int retryCount = 0;

            while (retryCount < maxRetries)
            {
                try
                {
                    connection.Open();
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error connecting to PostgreSQL: {ex.Message}");
                    retryCount++;
                    Task.Delay(1000).Wait();
                }
            }
        }

        public async Task InitializeAsync()
        {
            // Initialization logic here, if needed
        }

        public async Task DisposeAsync()
        {
            _dockerClient.Containers.StopContainerAsync(_dockerContainerId, new ContainerStopParameters()).Wait();
            _dockerClient.Containers.RemoveContainerAsync(_dockerContainerId, new ContainerRemoveParameters()).Wait();
            DbContext?.Dispose();
        }

        public ProductRepository CreateProductRepository()
        {
            return new ProductRepository(DbContext);
        }
    }

    [CollectionDefinition("DbEmulatorCollection")]
    public class DbEmulatorCollection : ICollectionFixture<DbEmulatorFixture>
    {
    }
}
