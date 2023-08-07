using Microsoft.EntityFrameworkCore;
using TechMastery.MarketPlace.Persistence;
using TechMastery.MarketPlace.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Respawn.Graph;
using TechMastery.MarketPlace.API.IntegrationTests.Base;
using Xunit;

public class ApiIntegrationTestFixture 
{
    private readonly ApplicationDbContext _dbContext;

    public ApiIntegrationTestFixture()
    {
        var serviceProvider = new ServiceCollection()
            .AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(databaseName: "TestDatabase"))
            .BuildServiceProvider();

        _dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
        _dbContext.Database.EnsureCreated();
        _dbContext.Database.Migrate();
    }

    private async Task ApplyMigrationsAsync()
    {
        var database = _dbContext.Database;
        await database.MigrateAsync();
        
    }

    /*
    private async Task InitRespawn()
    {
        _respawner = await Respawner.CreateAsync(_dbContext.Database.GetDbConnection(), new RespawnerOptions
        {
            TablesToIgnore = new[]
            {
                new Respawn.Graph.Table("__EFMigrationsHistory")
            }
        });
    
    public async Task ClearDown()
    {
        await _respawner.ResetAsync(_dbContext.Database.GetConnectionString());
    }
    }*/

    public ProductRepository CreateProductListingRepository()
    {
        return new ProductRepository(_dbContext); // Instantiate your real repository
    }

    protected void  DisposeContext()
    {
         _dbContext.Database.EnsureDeleted(); // Clean up the in-memory database
        _dbContext.Dispose();
    }
}
