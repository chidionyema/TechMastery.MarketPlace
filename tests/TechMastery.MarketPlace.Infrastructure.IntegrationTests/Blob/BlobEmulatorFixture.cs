using System.Diagnostics;

namespace TechMastery.MarketPlace.Infrastructure.IntegrationTests
{
    public class BlobEmulatorFixture : IAsyncLifetime
    {
        private Process _emulatorProcess;

        public async Task InitializeAsync()
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = "run -d -p 10000:10000 mcr.microsoft.com/azure-storage/azurite azurite-blob --blobHost 0.0.0.0",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            _emulatorProcess = Process.Start(startInfo);

            // Wait for the emulator to start (you can improve this by checking for readiness)
            await Task.Delay(TimeSpan.FromSeconds(5));
        }

        public async Task DisposeAsync()
        {
            if (_emulatorProcess != null && !_emulatorProcess.HasExited)
            {
                _emulatorProcess.Kill();
                await _emulatorProcess.WaitForExitAsync();
            }
        }
    }

    [CollectionDefinition("BlobEmulatorCollection")]
    public class BlobEmulatorCollection : ICollectionFixture<BlobEmulatorFixture>
    {
    }
}
