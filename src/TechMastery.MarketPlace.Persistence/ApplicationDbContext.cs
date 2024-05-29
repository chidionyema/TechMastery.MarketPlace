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

        public DbSet<OutboxMessage> OutboxMessages { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderLineItem> OrderLineItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Framework> Frameworks { get; set; }
        public DbSet<Platform> Platforms { get; set; }
        public DbSet<ProductLanguage> ProductLanguages { get; set; }
        public DbSet<ProductFramework> ProductFrameworks { get; set; }
        public DbSet<ProductPlatform> ProductPlatforms { get; set; }
        public DbSet<Contributor> Contributors { get; set; }
        public DbSet<Contribution> Contributions { get; set; }
        public DbSet<SaleTransaction> SaleTransactions { get; set; }
        public DbSet<ProductArtifact> ProductArtifacts { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }
        public DbSet<ProductArtifactDownloadHistory> ProductDownloadHistory { get; set; }
        public DbSet<ProductLicense> ProductLicenses { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<ArchivedOutboxMessage> ArchivedOutboxMessages { get; set; }

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
               new Category("AI/ML", Guid.NewGuid())
           );
        }

        private void ConfigureEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.HasMany(c => c.SubCategories).WithOne(c => c.ParentCategory).HasForeignKey(c => c.ParentCategoryId);
                // Configure other relationships and properties for Category entity
            });

            modelBuilder.Entity<Order>(entity => { entity.HasKey(o => o.Id); /* Configure other properties and relationships */ });

            modelBuilder.Entity<OrderLineItem>(entity => { entity.HasKey(oli => oli.Id); /* Configure other properties and relationships */ });

            // ProductLanguage Configuration
            modelBuilder.Entity<ProductLanguage>()
                .HasKey(pl => new { pl.ProductId, pl.LanguageId });
            modelBuilder.Entity<ProductLanguage>()
                .HasOne(pl => pl.Product)
                .WithMany(p => p.ProductLanguages)
                .HasForeignKey(pl => pl.ProductId);
            modelBuilder.Entity<ProductLanguage>()
                .HasOne(pl => pl.Language)
                .WithMany() // If Language does not have a navigation property back
                .HasForeignKey(pl => pl.LanguageId);

            // ProductFramework Configuration
            modelBuilder.Entity<ProductFramework>()
                .HasKey(pf => new { pf.ProductId, pf.FrameworkId });
            modelBuilder.Entity<ProductFramework>()
                .HasOne(pf => pf.Product)
                .WithMany(p => p.ProductFrameworks)
                .HasForeignKey(pf => pf.ProductId);
            modelBuilder.Entity<ProductFramework>()
                .HasOne(pf => pf.Framework)
                .WithMany() // If Framework does not have a navigation property back
                .HasForeignKey(pf => pf.FrameworkId);

            // ProductPlatform Configuration
            modelBuilder.Entity<ProductPlatform>()
                .HasKey(pp => new { pp.ProductId, pp.PlatformId });
            modelBuilder.Entity<ProductPlatform>()
                .HasOne(pp => pp.Product)
                .WithMany(p => p.ProductPlatforms)
                .HasForeignKey(pp => pp.ProductId);
            modelBuilder.Entity<ProductPlatform>()
                .HasOne(pp => pp.Platform)
                .WithMany() // If Platform does not have a navigation property back
                .HasForeignKey(pp => pp.PlatformId);


            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(s => s.Id);
                // Configure other properties and relationships for Product entity
            });

            modelBuilder.Entity<Contributor>(entity => { /* Configure Contributor entity */ });

            modelBuilder.Entity<Contribution>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.HasOne(c => c.Product).WithMany(p => p.Contributions).HasForeignKey(c => c.ProductId);
                entity.HasOne(c => c.Contributor).WithMany(c => c.Contributions).HasForeignKey(c => c.ContributorId);
                // Configure other properties and relationships for Contribution entity
            });

            modelBuilder.Entity<SaleTransaction>(entity => { /* Configure SaleTransaction entity */ });

            modelBuilder.Entity<ProductArtifact>(entity =>
            {
                entity.HasKey(sa => sa.Id);
                // Configure properties and relationships for ProductArtifact entity
            });


            modelBuilder.Entity<ProductReview>(entity => { entity.HasKey(sr => sr.ProductReviewId); /* Configure other properties and relationships */ });

            modelBuilder.Entity<ProductTag>(entity => { entity.HasKey(st => st.TagId); /* Configure other properties and relationships */ });

            modelBuilder.Entity<ProductArtifactDownloadHistory>(entity =>
            {
                entity.HasKey(sd => sd.Id);
                entity.HasOne(sd => sd.ProductListing).WithMany(s => s.Downloads).HasForeignKey(sd => sd.ProductListingId);
                entity.HasOne(sd => sd.ProductArtifact).WithMany().HasForeignKey(sd => sd.ProductArtifactId);
                // Configure other properties and relationships for ProductArtifactDownloadHistory entity
            });

            modelBuilder.Entity<ProductLicense>(entity => { entity.HasKey(sl => sl.Id); /* Configure other properties and relationships */ });
        }

        private void SeedEnums(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<ProductStatus>().HasData(
                Enum.GetValues(typeof(ProductStatusEnum))
                    .Cast<ProductStatusEnum>()
                    .Select((type, index) =>
                        new ProductStatus { Name = type.ToString() })
            );
            modelBuilder.Entity<PaymentStatus>().HasData(
               Enum.GetValues(typeof(PaymentStatusEnum))
                   .Cast<PaymentStatusEnum>()
                   .Select((type, index) =>
                       new PaymentStatus { Name = type.ToString() })
           );

            modelBuilder.Entity<Language>().HasData(
                new Language {  Name = "C#" },
                new Language { Name = "JavaScript" }
            // Add more languages as needed
            );

            modelBuilder.Entity<Framework>().HasData(
                new Framework {  Name = ".NET Core", Version = "3.1" },
                new Framework {  Name = "React", Version = "16.13.1" }
            // Add more frameworks as needed
            );

            modelBuilder.Entity<Platform>().HasData(
                new Platform {  Name = "Windows" },
                new Platform {  Name = "Linux" }
            // Add more platforms as needed
            );

        }

        private void SeedCategoryDependencies(ModelBuilder modelBuilder)
        {
          //  CategoryDataSeeder.SeedCategoryDependencies(modelBuilder);
        }
    }
}
