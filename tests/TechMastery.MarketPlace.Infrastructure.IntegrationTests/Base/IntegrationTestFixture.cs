using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using TechMastery.MarketPlace.Persistence;
using TechMastery.MarketPlace.Persistence.Repositories;

public class IntegrationTestFixture
{
    private readonly ApplicationDbContext _dbContext;
    private readonly string _connectionString;

    public IntegrationTestFixture()
    {
        // Replace "YourConnectionString" with your actual PostgreSQL connection string
        _connectionString = "YourConnectionString";

        var serviceProvider = new ServiceCollection()
            .AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(_connectionString)) // Use PostgreSQL provider
            .BuildServiceProvider();

        _dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
        _dbContext.Database.EnsureCreated();
        _dbContext.Database.Migrate();
    }

    public ProductRepository CreateProductListingRepository()
    {
        return new ProductRepository(_dbContext); // Instantiate your real repository
    }

    protected void DisposeContext()
    {
        _dbContext.Database.EnsureDeleted(); // Clean up the database
        _dbContext.Dispose();
    }
}
