using TechMastery.MarketPlace.Domain.Common;

namespace TechMastery.MarketPlace.Domain.Entities
{
    public class Product : AuditableEntity
    {
        public Product(Guid categoryId, string name, string description, string demoURL, decimal price,
                    string license, string owner, string purpose)
        {
            CategoryId = categoryId;
            Name = name;
            Description = description;
            DemoURL = demoURL;
            Price = price;
            License = license;
            Owner = owner;
            Purpose = purpose;
            Status = ProductStatusEnum.NewlyListed;
        }

        public Guid ProductId { get; protected set; }
        public Guid CategoryId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public string DemoURL { get; private set; } = string.Empty;
        public Category? Category { get; private set; }
        public decimal Price { get; private set; }
        public string License { get; private set; } = string.Empty;
        public string Owner { get; private set; } = string.Empty;
        public string Purpose { get; private set; } = string.Empty;
        public ProductOwner ProductOwner { get; private set; }
        public List<Contribution> Contributions { get; private set; } = new List<Contribution>();
        public ICollection<ProductTag> Tags { get; private set; } = new List<ProductTag>();
        public ICollection<ProductArtifact> Artifacts { get; private set; } = new List<ProductArtifact>();
        public ICollection<ProductLicense> Licenses { get; private set; } = new List<ProductLicense>();
        public ICollection<ProductDependency> Dependencies { get; private set; } = new List<ProductDependency>();
        public ICollection<ProductReview> Reviews { get; private set; } = new List<ProductReview>();
        public ICollection<ProductArtifactDownloadHistory> Downloads { get; private set; } = new List<ProductArtifactDownloadHistory>();
        public ICollection<Order> Orders { get; private set; } = new List<Order>();
        public ICollection<SaleTransaction> Sales { get; private set; } = new List<SaleTransaction>();
        public ProductStatusEnum Status { get; private set; }

        public void SetName(string name)
        {
            Name = name;
        }

        public void SetDescription(string description)
        {
            Description = description;
        }

        public void AddLicense(ProductLicense license)
        {
            Licenses.Add(license);
        }

        public void AddDependency(ProductDependency dependency)
        {
            Dependencies.Add(dependency);
        }

        public void AddReview(ProductReview review)
        {
            Reviews.Add(review);
        }

        public void AddDownloadHistory(ProductArtifactDownloadHistory downloadHistory)
        {
            Downloads.Add(downloadHistory);
        }

        public void AddOrder(Order order)
        {
            Orders.Add(order);
        }

        public void UpdateListing(string name, string description, string demoUrl, decimal price)
        {
            Name = name;
            Description = description;
            DemoURL = demoUrl;
            Price = price;
        }

        public bool IsTransient()
        {
            return ProductId == Guid.Empty;
        }

        public void AddArtifact(ProductArtifact productArtifact)
        {
            Artifacts.Add(productArtifact);
        }

        public void RemoveDependency(ProductDependency removedDependency)
        {
            Dependencies.Remove(removedDependency);
        }

        public void ClearDependencies()
        {
            Dependencies.Clear();
        }

        public void ClearTags()
        {
            Tags.Clear();
        }

        public void AddTag(string tag)
        {
            Tags.Add(new ProductTag (tag));
        }

        public void RemoveTag(ProductTag existingTag)
        {
            Tags.Remove(existingTag);
        }

        public void UpdateCoreProperties(string name, string description, string demoUrl, decimal price)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be empty.", nameof(name));
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Description cannot be empty.", nameof(description));
            }

            if (string.IsNullOrWhiteSpace(demoUrl))
            {
                throw new ArgumentException("Demo URL cannot be empty.", nameof(demoUrl));
            }

            if (price <= 0)
            {
                throw new ArgumentException("Price must be greater than zero.", nameof(price));
            }

            Name = name;
            Description = description;
            DemoURL = demoUrl;
            Price = price;
        }
    }
}