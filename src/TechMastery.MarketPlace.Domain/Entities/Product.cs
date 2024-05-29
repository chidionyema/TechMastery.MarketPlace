using TechMastery.MarketPlace.Domain.Common;

namespace TechMastery.MarketPlace.Domain.Entities
{
    public class Product : AuditableEntity
    {
        private readonly List<Contribution> _contributions = new();
        private readonly List<ProductTag> _tags = new();
        private readonly List<ProductArtifact> _artifacts = new();
        private readonly List<ProductLicense> _licenses = new();
        private readonly List<ProductReview> _reviews = new();
        private readonly List<ProductArtifactDownloadHistory> _downloads = new();
        private readonly List<Order> _orders = new();
        private readonly List<SaleTransaction> _sales = new();
        public ICollection<ProductLanguage> ProductLanguages { get; set; } = new List<ProductLanguage>();
        public ICollection<ProductFramework> ProductFrameworks { get; set; } = new List<ProductFramework>();
        public ICollection<ProductPlatform> ProductPlatforms { get; set; } = new List<ProductPlatform>();

        public Product(Guid categoryId, string name, string description, string demoURL, decimal price,
                       string license, string owner, string purpose)
        {
            CategoryId = categoryId;
            UpdateCoreProperties(name, description, demoURL, price);
            License = license;
            Owner = owner;
            Purpose = purpose;
            Status = ProductStatusEnum.NewlyListed;
        }
        public Guid CategoryId { get; private set; }
        public Category? Category { get; private set; }
        public string? Name { get; private set; }
        public string? Description { get; private set; }
        public string? DemoURL { get; private set; }
        public decimal Price { get; private set; }
        public string License { get; private set; }
        public string Owner { get; private set; }
        public string Purpose { get; private set; }
        public ProductStatusEnum Status { get; private set; }

        public IReadOnlyCollection<Contribution> Contributions => _contributions.AsReadOnly();
        public IReadOnlyCollection<ProductTag> Tags => _tags.AsReadOnly();
        public IReadOnlyCollection<ProductArtifact> Artifacts => _artifacts.AsReadOnly();
        public IReadOnlyCollection<ProductLicense> Licenses => _licenses.AsReadOnly();
        public IReadOnlyCollection<ProductReview> Reviews => _reviews.AsReadOnly();
        public IReadOnlyCollection<ProductArtifactDownloadHistory> Downloads => _downloads.AsReadOnly();
        public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();
        public IReadOnlyCollection<SaleTransaction> Sales => _sales.AsReadOnly();

        public void UpdateCoreProperties(string name, string description, string demoURL, decimal price)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be empty.", nameof(name));
            }
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Description cannot be empty.", nameof(description));
            }
            /*
            if (string.IsNullOrWhiteSpace(demoURL))
            {
                throw new ArgumentException("Demo URL cannot be empty.", nameof(demoURL));
            }*/
            if (price <= 0)
            {
                throw new ArgumentException("Price must be greater than zero.", nameof(price));
            }

            Name = name;
            Description = description;
            DemoURL = demoURL;
            Price = price;
        }

        public void AddTag(ProductTag tag)
        {
            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            _tags.Add(tag);
        }

        public void RemoveTag(ProductTag tag)
        {
            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            _tags.Remove(tag);
        }

        public void UpdateTags(IEnumerable<ProductTag> updatedTags)
        {
            if (updatedTags == null)
            {
                throw new ArgumentNullException(nameof(updatedTags));
            }

            _tags.Clear();
            _tags.AddRange(updatedTags);
        }

        public void AddReview(ProductReview review)
        {
            if (review == null)
            {
                throw new ArgumentNullException(nameof(review));
            }

            _reviews.Add(review);
        }

        public void RemoveReview(ProductReview review)
        {
            if (review == null)
            {
                throw new ArgumentNullException(nameof(review));
            }

            _reviews.Remove(review);
        }

        public void AddArtifact(ProductArtifact artifact)
        {
            if (artifact == null)
            {
                throw new ArgumentNullException(nameof(artifact));
            }

            _artifacts.Add(artifact);
        }

        public void RemoveArtifact(ProductArtifact artifact)
        {
            if (artifact == null)
            {
                throw new ArgumentNullException(nameof(artifact));
            }

            _artifacts.Remove(artifact);
        }

        public void AddLicense(ProductLicense license)
        {
            if (license == null)
            {
                throw new ArgumentNullException(nameof(license));
            }

            _licenses.Add(license);
        }

        public void RemoveLicense(ProductLicense license)
        {
            if (license == null)
            {
                throw new ArgumentNullException(nameof(license));
            }

            _licenses.Remove(license);
        }

        public void AddDownloadHistory(ProductArtifactDownloadHistory downloadHistory)
        {
            if (downloadHistory == null)
            {
                throw new ArgumentNullException(nameof(downloadHistory));
            }

            _downloads.Add(downloadHistory);
        }

        public void RemoveDownloadHistory(ProductArtifactDownloadHistory downloadHistory)
        {
            if (downloadHistory == null)
            {
                throw new ArgumentNullException(nameof(downloadHistory));
            }

            _downloads.Remove(downloadHistory);
        }

        public void AddOrder(Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            _orders.Add(order);
        }

        public void RemoveOrder(Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            _orders.Remove(order);
        }

        public void AddSale(SaleTransaction sale)
        {
            if (sale == null)
            {
                throw new ArgumentNullException(nameof(sale));
            }

            _sales.Add(sale);
        }

        public void RemoveSale(SaleTransaction sale)
        {
            if (sale == null)
            {
                throw new ArgumentNullException(nameof(sale));
            }

            _sales.Remove(sale);
        }

        public bool IsTransient()
        {
            return Id == Guid.Empty;
        }
        public void ClearTags()
        {
            _tags.Clear();
        }


        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null, empty, or consist only of white-space characters.", nameof(name));
            }

            Name = name;
        }

        public void SetDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Description cannot be null, empty, or consist only of white-space characters.", nameof(description));
            }

            Description = description;
        }
    }

    public class Language : AuditableEntity
    {
        
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
    }

    public class Framework : AuditableEntity
    {
        
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
    }

    public class Platform : AuditableEntity
    {
      
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
    }

    public class ProductLanguage : AuditableEntity
    {
        public Guid ProductId { get; set; }
        public Guid LanguageId { get; set; }
        public string Version { get; set; } = string.Empty;
        public Product? Product { get; set; }
        public Language? Language { get; set; }
    }

    public class ProductFramework : AuditableEntity
    {
        public Guid ProductId { get; set; }
        public Guid FrameworkId { get; set; }
        public string Version { get; set; } = string.Empty;
        public Product? Product { get; set; }
        public Framework? Framework { get; set; }
    }

    public class ProductPlatform : AuditableEntity
    {
        public Guid? ProductId { get; set; } 
        public Guid PlatformId { get; set; }
        public string Version { get; set; } = string.Empty;
        public Product? Product { get; set; }
        public Platform? Platform { get; set; }
    }
}
