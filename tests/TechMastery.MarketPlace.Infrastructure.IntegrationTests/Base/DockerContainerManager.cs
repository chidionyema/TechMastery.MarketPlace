using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechMastery.MarketPlace.Infrastructure.IntegrationTests
{
    public class DockerContainerManager
    {
        private readonly DockerClient _dockerClient;

        public DockerContainerManager()
        {
            _dockerClient = new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock")).CreateClient();
        }

        public async Task<string> StartContainerAsync(string imageName, IDictionary<string, string> environmentVariables, IDictionary<string, string> ports)
        {
            var exposedPorts = new Dictionary<string, EmptyStruct>();
            foreach (var port in ports)
            {
                exposedPorts[port.Key] = default;
            }

            var createContainerResponse = await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Image = imageName,
                Env = environmentVariables.Select(kv => $"{kv.Key}={kv.Value}").ToList(),
                ExposedPorts = exposedPorts,
                HostConfig = new HostConfig
                {
                    PortBindings = (IDictionary<string, IList<PortBinding>>)ports.ToDictionary(kv => kv.Key, kv => new List<PortBinding>
                    {
                        new PortBinding { HostPort = kv.Value }
                    })
                }
            });

            var containerId = createContainerResponse.ID;

            await _dockerClient.Containers.StartContainerAsync(containerId, null);

            return containerId;
        }

        public async Task StopAndRemoveContainerAsync(string containerId)
        {
            try
            {
                await _dockerClient.Containers.StopContainerAsync(containerId, new ContainerStopParameters());
                await _dockerClient.Containers.RemoveContainerAsync(containerId, new ContainerRemoveParameters());
            }
            catch
            {
                // Ignore errors if the container doesn't exist or cannot be stopped/removed
            }
        }

        public async Task WaitUntilDatabaseAvailableAsync(string databasePort)
        {
            var start = DateTime.UtcNow;
            const int maxWaitTimeSeconds = 60;
            var connectionEstablished = false;
            while (!connectionEstablished && start.AddSeconds(maxWaitTimeSeconds) > DateTime.UtcNow)
            {
                try
                {
                    var sqlConnectionString = DockerSqlDatabaseUtilities.GetSqlConnectionString(databasePort);
                    using var sqlConnection = new Npgsql.NpgsqlConnection(sqlConnectionString);
                    await sqlConnection.OpenAsync();
                    connectionEstablished = true;
                }
                catch
                {
                    // If opening the SQL connection fails, SQL Server is not ready yet
                    await Task.Delay(500);
                }
            }

            if (!connectionEstablished)
            {
                throw new Exception($"Connection to the SQL docker database could not be established within {maxWaitTimeSeconds} seconds.");
            }
        }
    }
}
