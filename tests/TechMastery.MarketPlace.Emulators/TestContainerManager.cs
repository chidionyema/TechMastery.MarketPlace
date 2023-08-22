using Docker.DotNet;
using Docker.DotNet.Models;

namespace TechMastery.MarketPlace.Tests.Emulators
{
    public class TestContainerManager : IDisposable
    {
        private DockerClient _dockerClient;

        public TestContainerManager()
        {
            _dockerClient = new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock"))
                .CreateClient();
        }

        public async Task<string> StartContainerAsync(string imageName, List<string> environmentVariables, int containerPort, int hostPort)
        {
            var envVariables = environmentVariables.Select(envVar => $"{envVar.Split('=')[0]}={envVar.Split('=')[1]}");

            var exposedPort = $"{containerPort}/tcp";

            var createContainerResponse = await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Image = imageName,
                Env = environmentVariables,
                ExposedPorts = new Dictionary<string, EmptyStruct>
                {
                    [exposedPort] = default
                },
                HostConfig = new HostConfig
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        [exposedPort] = new List<PortBinding>
                {
                    new PortBinding
                    {
                        HostPort = hostPort.ToString()
                    }
                }
                    }
                }
            });

            var containerId = createContainerResponse.ID;

            await _dockerClient.Containers.StartContainerAsync(containerId, null);

            return containerId;
        }



        public async Task StopAndRemoveContainerAsync(string containerId)
        {
            var containerExists = await ContainerExists(containerId);

            if (containerExists)
            {
                await _dockerClient.Containers.StopContainerAsync(containerId, new ContainerStopParameters());
                await _dockerClient.Containers.RemoveContainerAsync(containerId, new ContainerRemoveParameters());
            }
        }

        private async Task<bool> ContainerExists(string containerId)
        {
            try
            {
                var containers = await _dockerClient.Containers.ListContainersAsync(new ContainersListParameters
                {
                    All = true // Include stopped containers in the list
                });

                return containers.Any(container => container.ID == containerId);
            }
            catch (Exception)
            {
                return false; // Return false if any error occurs during listing containers
            }
        }


        public string GetContainerIPAddress(string containerId)
        {
            var inspectResponse = _dockerClient.Containers.InspectContainerAsync(containerId).Result;
            return inspectResponse.NetworkSettings.IPAddress;
        }

        public void Dispose()
        {
            _dockerClient?.Dispose();
        }
    }
}
