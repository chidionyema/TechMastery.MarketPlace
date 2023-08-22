using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TechMastery.MarketPlace.Persistence;
using TechMastery.MarketPlace.Tests.Emulators;

namespace TechMastery.MarketPlace.API.IntegrationTests.Base
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        private readonly DbEmulatorFixture _dbEmulatorFixture;
        private readonly RabbitMQFixture _rabbitMQFixture;
        private readonly ElasticSearchFixture _elasticSearchFixture;
        private readonly BlobEmulatorFixture _blobFixture;

        public CustomWebApplicationFactory(
            DbEmulatorFixture dbEmulatorFixture,
            RabbitMQFixture rabbitMQFixture,
            ElasticSearchFixture elasticSearchFixture,
            BlobEmulatorFixture blobFixture)
        {
            _dbEmulatorFixture = dbEmulatorFixture ?? throw new ArgumentNullException(nameof(dbEmulatorFixture));
            _rabbitMQFixture = rabbitMQFixture ?? throw new ArgumentNullException(nameof(rabbitMQFixture));
            _elasticSearchFixture = elasticSearchFixture ?? throw new ArgumentNullException(nameof(elasticSearchFixture));
            _blobFixture = blobFixture ?? throw new ArgumentNullException(nameof(blobFixture));
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseStartup<TestStartup>();

            builder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                // Here, we're adding in-memory configurations for the test environment.
                // Depending on your test scenario, adjust these values.
                var testConfig = new Dictionary<string, string>
            {
                { "MessagingSystemOptions:EnableRabbitMq", "true" },
                { "MessagingSystemOptions:EnableSQS", "false" },
                { "MessagingSystemOptions:EnableAzureServiceBus", "false" }
            };

                config.AddInMemoryCollection(testConfig);
            });

            builder.ConfigureServices(async services =>
            {
                // Mock the authentication. Assuming you have a scheme and handler for this purpose.
                services.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });

                // Initialize the fixtures in parallel.
                await InitializeEmulatorsInParallel();

                // Seed Database or any other setup procedures.
                SeedDatabase(services);
            });
        }

        private async Task InitializeEmulatorsInParallel()
        {
            var tasks = new List<Task>
            {
                _dbEmulatorFixture.InitializeAsync(),
                _rabbitMQFixture.InitializeAsync(),
                _elasticSearchFixture.InitializeAsync(),
                _blobFixture.InitializeAsync()
            };

            await Task.WhenAll(tasks);
        }

        private void SeedDatabase(IServiceCollection services)
        {
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();

            var scopedServices = scope.ServiceProvider;
            var context = scopedServices.GetRequiredService<ApplicationDbContext>();
            var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

            try
            {
                context.Database.EnsureCreated();
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error occurred seeding the database with test messages. Error: {ex.Message}");
                throw; // Re-throw to make sure test setup fails if seeding fails.
            }
        }

        public HttpClient GetAnonymousClient()
        {
            return CreateClient();
        }

        public HttpClient GetAuthenticatedClient()
        {
            var client = CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", "TestAuth"); // Replace with actual auth header
            return client;
        }
    }

    // You'll need an authentication handler for the above mocked authentication.
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Mock a user authentication for testing.
            // You might want to customize this with claims, roles, etc. relevant to your application.
            var claims = new[] { new Claim(ClaimTypes.Name, "TestUser") };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");

            var result = AuthenticateResult.Success(ticket);

            return Task.FromResult(result);
        }
    }
}
