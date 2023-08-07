using TechMastery.MarketPlace.Domain.Common;

namespace TechMastery.MarketPlace.Domain.Entities
{
    public class Category : AuditableEntity
    {
        public Category() { }

        public Category(string name)
        {
            Name = name;
        }

        public Category(string name, Guid Id)
        {
            Name = name;
            CategoryId = Id;
            CreatedDate = DateTime.UtcNow;
        }

        public Guid CategoryId { get; protected set; }
        public string Name { get; private set; } = string.Empty;
        public Category? ParentCategory { get; private set; }
        public Guid? ParentCategoryId { get; private set; }
        public ICollection<CategoryDependency>? Dependencies { get; private set; } = new List<CategoryDependency>();
        public ICollection<Category>? SubCategories { get; private set; } = new List<Category>();
        public ICollection<Product>? ProductListings { get; private  set; }

        public void AddSubCategory(Category subCategory)
        {
            SubCategories?.Add(subCategory);
        }

        public void SetId(Guid Id)
        {
            CategoryId = Id;
        }
        public void SetName(string name)
        {
            Name = name;
        }

        public void RemoveSubCategory(Category subCategory)
        {
            SubCategories?.Remove(subCategory);
        }

        public void AddDependency(CategoryDependency dependency)
        {
            Dependencies?.Add(dependency);
        }

        public void RemoveDependency(CategoryDependency dependency)
        {
            Dependencies?.Remove(dependency);
        }

        public void UpdateCategory(string name)
        {
            Name = name;
        }
    }
}
