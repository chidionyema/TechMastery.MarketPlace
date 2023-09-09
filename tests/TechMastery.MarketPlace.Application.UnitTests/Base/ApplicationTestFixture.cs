using Microsoft.EntityFrameworkCore;
using TechMastery.MarketPlace.Persistence;
using TechMastery.MarketPlace.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using TechMastery.MarketPlace.Application.Contracts.Persistence;

public class ApplicationTestFixture : IDisposable
{
    private readonly ApplicationDbContext _dbContext;
    private readonly string _databasePath = "test-database.db";

    public ApplicationTestFixture()
    {
        var connectionString = $"Data Source={_databasePath}";

        var serviceProvider = new ServiceCollection()
            .AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString))
            .BuildServiceProvider();

        _dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
        _dbContext.Database.Migrate();
    }

    internal IShoppingCartRepository CreateCartRepository()
    {
        return new ShoppingCartRepository(_dbContext);
    }

    internal ICartItemRepository CreateCartItemRepository()
    {
        return new CartItemRepository(_dbContext);
    }

    internal IProductRepository CreateProductListingRepository()
    {
        return new ProductRepository(_dbContext);
    }

    internal ICategoryRepository CreateCategoryRepository()
    {
        return new CategoryRepository(_dbContext);
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    internal IOrderRepository CreateOrderRepository()
    {
        return new OrderRepository(_dbContext);
    }
}
