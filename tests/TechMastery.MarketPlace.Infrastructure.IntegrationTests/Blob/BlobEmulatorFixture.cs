using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using TechMastery.MarketPlace.Infrastructure.IntegrationTests.Base;
using Microsoft.Extensions.Configuration;

namespace TechMastery.MarketPlace.Infrastructure.IntegrationTests
{
    public class BlobEmulatorFixture : IAsyncLifetime
    {
        private TestContainerManager _containerManager;
        private string _containerId;

        public async Task InitializeAsync()
        {
            _containerManager = new TestContainerManager();
            await _containerManager.StopAndRemoveContainerAsync("mcr.microsoft.com/azure-storage/azurite");
            var environmentVariables = new List<string>
            {
                "BLOB_HOST=0.0.0.0"
            };

            var portBindings = new List<string>
            {
                "10000"
            };

            _containerId = await _containerManager.StartContainerAsync("mcr.microsoft.com/azure-storage/azurite", environmentVariables, 10000, 10000);

            // Wait for the emulator to start (you can improve this by checking for readiness)
            await Task.Delay(TimeSpan.FromSeconds(5));
        }

        public async Task DisposeAsync()
        {
            await _containerManager.StopAndRemoveContainerAsync(_containerId);
        }

        internal string GetBlobEmulatorConnectionString()
        {
            return "UseDevelopmentStorage=true"; // Use the appropriate connection string
        }
    }

    [CollectionDefinition("BlobEmulatorCollection")]
    public class BlobEmulatorCollection : ICollectionFixture<BlobEmulatorFixture>
    {
    }
}
