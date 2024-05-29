using TechMastery.MarketPlace.Domain.Common;

namespace TechMastery.MarketPlace.Domain.Entities
{
    public class Category : AuditableEntity
    {
        private readonly List<Category> _subCategories = new List<Category>();

        public Category(string name, Guid? id = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Category name cannot be null or empty.", nameof(name));
            }

            Name = name;
            Id = id ?? Guid.NewGuid();
            CreatedDate = DateTime.UtcNow;
        }

        public Category()
        {
        }

        public string Name { get; private set; }
        public Category? ParentCategory { get; private set; }
        public Guid? ParentCategoryId { get; private set; }
        public IReadOnlyCollection<Category> SubCategories => _subCategories.AsReadOnly();

        public void AddSubCategory(Category subCategory)
        {
            if (subCategory == null)
            {
                throw new ArgumentNullException(nameof(subCategory));
            }

            _subCategories.Add(subCategory);
        }

        public void UpdateName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
            {
                throw new ArgumentException("Category name cannot be null or empty.", nameof(newName));
            }

            Name = newName;
        }

        public void RemoveSubCategory(Category subCategory)
        {
            _subCategories.Remove(subCategory);
        }
    }
}
