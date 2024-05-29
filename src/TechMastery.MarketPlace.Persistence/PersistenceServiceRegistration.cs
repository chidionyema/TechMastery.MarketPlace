using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechMastery.MarketPlace.Application.Contracts;
using TechMastery.MarketPlace.Application.Persistence;
using TechMastery.MarketPlace.Application.Persistence.Contracts;
using TechMastery.MarketPlace.Application.Services;
using TechMastery.MarketPlace.Persistence.Repositories;

namespace TechMastery.MarketPlace.Persistence
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            // Configure logging
            services.AddLogging();

            // Validate configuration and retrieve connection string
            var connectionString = configuration.GetConnectionString("TechMasteryMarketPlaceConnectionString");
            

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Connection string is missing or empty.", nameof(connectionString));
            }

            // Add database context with configuration options
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
                options.EnableSensitiveDataLogging(); // Optional, for debugging purposes
            });

            // Add health checks
            services.AddHealthChecks()
                 .AddCheck<DatabaseContextHealthCheck>("DatabaseContextHealthCheck");


            // Scoped services for better separation of concerns
            services.AddScoped(typeof(IAsyncRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICartItemRepository, CartItemRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
            services.AddScoped<IOutboxRepository, OutboxRepository>();
            services.AddScoped<ILoggedInUserService, LoggedInUserService>();

            return services;
        }
    }
}
