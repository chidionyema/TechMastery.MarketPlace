using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TechMastery.MarketPlace.Domain.Common;
using TechMastery.MarketPlace.Domain.Entities;
using TechMastery.MarketPlace.Persistence.Seed;

namespace TechMastery.MarketPlace.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet properties for your entities
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryDependency> CategoryDependencies { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderLineItem> OrderLineItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductOwner> ProductOwners { get; set; }
        public DbSet<Contributor> Contributors { get; set; }
        public DbSet<Contribution> Contributions { get; set; }
        public DbSet<SaleTransaction> SaleTransactions { get; set; }
        public DbSet<ProductArtifact> ProductArtifacts { get; set; }
        public DbSet<ProductDependency> ProductDependencies { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }
        public DbSet<ProductArtifactDownloadHistory> ProductDownloadHistory { get; set; }
        public DbSet<ProductLicense> ProductLicenses { get; set; }
        public DbSet<ProductDependencyType> ProductDependencyTypes { get; set; }
        public DbSet<CategoryDependencyType> CategoryDependencyTypes { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.UtcNow;
                        entry.Entity.CreatedBy = "";
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedDate = DateTime.UtcNow;
                        entry.Entity.LastModifiedBy = "";
                        break;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureEntities(modelBuilder);
            ConfigureEnums(modelBuilder);
            SeedEnums(modelBuilder);
            SeedCategoryDependencies(modelBuilder);
        }

        private void ConfigureEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.CategoryId);
                entity.HasMany(c => c.SubCategories).WithOne(c => c.ParentCategory).HasForeignKey(c => c.ParentCategoryId);
            });

            modelBuilder.Entity<CategoryDependency>(entity =>
            {
                entity.HasKey(cd => cd.CategoryDependencyId);
                entity.HasOne(cd => cd.Category).WithMany(c => c.Dependencies).HasForeignKey(cd => cd.CategoryId);
            });

            modelBuilder.Entity<Order>(entity => { entity.HasKey(o => o.OrderId); });

            modelBuilder.Entity<OrderLineItem>(entity => { entity.HasKey(oli => oli.OrderLineItemId); });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(s => s.ProductId);
                entity.HasOne(s => s.Category).WithMany(c => c.ProductListings).HasForeignKey(s => s.CategoryId);
            });

            modelBuilder.Entity<ProductOwner>(entity =>
            {
                entity.HasKey(po => po.Email);
                entity.HasMany(po => po.OwnedProducts).WithOne(p => p.ProductOwner).HasForeignKey(p => p.Owner);
            });

            modelBuilder.Entity<Contributor>(entity => { /* Configure Contributor */ });

            modelBuilder.Entity<Contribution>(entity =>
            {
                entity.HasKey(c => c.ContributionId);
                entity.HasOne(c => c.Product).WithMany(p => p.Contributions).HasForeignKey(c => c.ProductId);
                entity.HasOne(c => c.Contributor).WithMany(c => c.Contributions).HasForeignKey(c => c.ContributorId);
            });

            modelBuilder.Entity<SaleTransaction>(entity => { /* Configure SaleTransaction */ });

            modelBuilder.Entity<ProductArtifact>(entity =>
            {
                entity.HasKey(sa => sa.ProductArtifactId);
            });

            modelBuilder.Entity<ProductDependency>(entity =>
            {
                entity.HasKey(sd => sd.ProductDependencyId);
                entity.HasOne(sd => sd.Product).WithMany(s => s.Dependencies).HasForeignKey(sd => sd.ProductId);
            });

            modelBuilder.Entity<ProductReview>(entity => { entity.HasKey(sr => sr.ProductReviewId); });

            modelBuilder.Entity<ProductTag>(entity => { entity.HasKey(st => st.TagId); });

            modelBuilder.Entity<ProductArtifactDownloadHistory>(entity =>
            {
                entity.HasKey(sd => sd.ProductDownloadId);
                entity.HasOne(sd => sd.ProductListing).WithMany(s => s.Downloads)
                    .HasForeignKey(sd => sd.ProductListingId);
                entity.HasOne(sd => sd.ProductArtifact).WithMany().HasForeignKey(sd => sd.ProductArtifactId);
            });

            modelBuilder.Entity<ProductLicense>(entity => { entity.HasKey(sl => sl.ProductLicenseId); });
        }

        private void ConfigureEnums(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductDependencyType>().HasData(
                Enum.GetValues(typeof(ProductDependencyTypeEnum))
                    .Cast<ProductDependencyTypeEnum>()
                    .Select((type, index) =>
                        new ProductDependencyType { Id = index + 1, Name = type.ToString() })
            );

            modelBuilder.Entity<CategoryDependencyType>().HasData(
                Enum.GetValues(typeof(CategoryDependencyTypeEnum))
                    .Cast<CategoryDependencyTypeEnum>()
                    .Select((type, index) =>
                        new CategoryDependencyType { Id = index + 1, Name = type.ToString() })
            );

            modelBuilder.Entity<ProductStatus>().HasData(
                Enum.GetValues(typeof(ProductStatusEnum))
                    .Cast<ProductStatusEnum>()
                    .Select((type, index) =>
                        new ProductStatus { Id = index + 1, Name = type.ToString() })
            );
        }

        private void SeedEnums(ModelBuilder modelBuilder)
        {
            // Seed your enum data here if needed
        }

        private void SeedCategoryDependencies(ModelBuilder modelBuilder)
        {
            CategoryDataSeeder.SeedCategoryDependencies(modelBuilder);
        }
    }
}
