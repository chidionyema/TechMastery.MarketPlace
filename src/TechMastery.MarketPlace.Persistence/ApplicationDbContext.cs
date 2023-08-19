using Microsoft.EntityFrameworkCore;
using TechMastery.MarketPlace.Domain.Common;
using TechMastery.MarketPlace.Domain.Entities;

namespace TechMastery.MarketPlace.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryDependency> CategoryDependencies { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderLineItem> OrderLineItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Contributor> Contributors { get; set; }
        public DbSet<Contribution> Contributions { get; set; }
        public DbSet<SaleTransaction> SaleTransactions { get; set; }
        public DbSet<ProductArtifact> ProductArtifacts { get; set; }
        public DbSet<ProductDependency> ProductDependencies { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }
        public DbSet<ProductArtifactDownloadHistory> ProductDownloadHistory { get; set; }
        public DbSet<ProductLicense> ProductLicenses { get; set; }
        public DbSet<DependencyType> DependencyTypes { get; set; }
        public DbSet<Dependency> Dependencies { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedDate = DateTime.UtcNow;
                    entry.Entity.CreatedBy = ""; // Provide a suitable default value
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.LastModifiedDate = DateTime.UtcNow;
                    entry.Entity.LastModifiedBy = ""; // Provide a suitable default value
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureEntities(modelBuilder);
            SeedEnums(modelBuilder);
            SeedCategoryDependencies(modelBuilder);
            modelBuilder.Entity<Category>().HasData(
               new Category("Web", Guid.NewGuid()),
               new Category("Devops", Guid.NewGuid()),
               new Category("AI", Guid.NewGuid())
           );
        }

        private void ConfigureEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.CategoryId);
                entity.HasMany(c => c.SubCategories).WithOne(c => c.ParentCategory).HasForeignKey(c => c.ParentCategoryId);
                // Configure other relationships and properties for Category entity
            });

            modelBuilder.Entity<CategoryDependency>(entity =>
            {
                entity.HasKey(cd => cd.CategoryDependencyId);
                entity.HasOne(cd => cd.Category).WithMany(c => c.Dependencies).HasForeignKey(cd => cd.CategoryId);
                // Configure other relationships and properties for CategoryDependency entity
            });

            modelBuilder.Entity<Order>(entity => { entity.HasKey(o => o.OrderId); /* Configure other properties and relationships */ });

            modelBuilder.Entity<OrderLineItem>(entity => { entity.HasKey(oli => oli.OrderLineItemId); /* Configure other properties and relationships */ });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(s => s.ProductId);
                // Configure other properties and relationships for Product entity
            });

            modelBuilder.Entity<Contributor>(entity => { /* Configure Contributor entity */ });

            modelBuilder.Entity<Contribution>(entity =>
            {
                entity.HasKey(c => c.ContributionId);
                entity.HasOne(c => c.Product).WithMany(p => p.Contributions).HasForeignKey(c => c.ProductId);
                entity.HasOne(c => c.Contributor).WithMany(c => c.Contributions).HasForeignKey(c => c.ContributorId);
                // Configure other properties and relationships for Contribution entity
            });

            modelBuilder.Entity<SaleTransaction>(entity => { /* Configure SaleTransaction entity */ });

            modelBuilder.Entity<ProductArtifact>(entity =>
            {
                entity.HasKey(sa => sa.ProductArtifactId);
                // Configure properties and relationships for ProductArtifact entity
            });

            modelBuilder.Entity<ProductDependency>(entity =>
            {
                entity.HasKey(sd => sd.ProductDependencyId);
                entity.HasOne(sd => sd.Product).WithMany(s => s.Dependencies).HasForeignKey(sd => sd.ProductId);
                // Configure properties and relationships for ProductDependency entity
            });

            modelBuilder.Entity<ProductReview>(entity => { entity.HasKey(sr => sr.ProductReviewId); /* Configure other properties and relationships */ });

            modelBuilder.Entity<ProductTag>(entity => { entity.HasKey(st => st.TagId); /* Configure other properties and relationships */ });

            modelBuilder.Entity<ProductArtifactDownloadHistory>(entity =>
            {
                entity.HasKey(sd => sd.ProductDownloadId);
                entity.HasOne(sd => sd.ProductListing).WithMany(s => s.Downloads).HasForeignKey(sd => sd.ProductListingId);
                entity.HasOne(sd => sd.ProductArtifact).WithMany().HasForeignKey(sd => sd.ProductArtifactId);
                // Configure other properties and relationships for ProductArtifactDownloadHistory entity
            });

            modelBuilder.Entity<ProductLicense>(entity => { entity.HasKey(sl => sl.ProductLicenseId); /* Configure other properties and relationships */ });
        }

        private void SeedEnums(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DependencyType>().HasData(
                Enum.GetValues(typeof(DependencyTypeEnum))
                    .Cast<DependencyTypeEnum>()
                    .Select((type, index) =>
                        new DependencyType { Id = index + 1, Name = type.ToString() })
            );
            modelBuilder.Entity<ProductStatus>().HasData(
                Enum.GetValues(typeof(ProductStatusEnum))
                    .Cast<ProductStatusEnum>()
                    .Select((type, index) =>
                        new ProductStatus { Id = index + 1, Name = type.ToString() })
            );
        }

        private void SeedCategoryDependencies(ModelBuilder modelBuilder)
        {
          //  CategoryDataSeeder.SeedCategoryDependencies(modelBuilder);
        }
    }
}
