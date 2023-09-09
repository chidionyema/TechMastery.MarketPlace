using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TechMastery.MarketPlace.Api;

namespace TechMastery.MarketPlace.API.IntegrationTests.Base
{
    public class TestStartup
    {
        private readonly WebApplicationBuilder _builder;

        public TestStartup(WebApplicationBuilder builder)
        {
            _builder = builder;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            _builder.ConfigureServices().
                     ConfigurePipeline();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                // Here, you can mock some services or data specifically for testing if needed
            }

        }
    }
}
