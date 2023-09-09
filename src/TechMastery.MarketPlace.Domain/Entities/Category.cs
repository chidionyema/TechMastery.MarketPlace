using TechMastery.MarketPlace.Domain.Common;
using System;
using System.Collections.Generic;

namespace TechMastery.MarketPlace.Domain.Entities
{
    public class Category : AuditableEntity
    {
        private readonly List<CategoryDependency> _dependencies = new List<CategoryDependency>();
        private readonly List<Category> _subCategories = new List<Category>();

        public Category(string name, Guid? id = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Category name cannot be null or empty.", nameof(name));
            }

            Name = name;
            CategoryId = id ?? Guid.NewGuid();
            CreatedDate = DateTime.UtcNow;
        }

        public Category()
        {
        }

        public Guid CategoryId { get; private set; }
        public string Name { get; private set; }
        public Category? ParentCategory { get; private set; }
        public Guid? ParentCategoryId { get; private set; }
        public IReadOnlyCollection<CategoryDependency> Dependencies => _dependencies.AsReadOnly();
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

        public void AddDependency(CategoryDependency dependency)
        {
            if (dependency == null)
            {
                throw new ArgumentNullException(nameof(dependency));
            }

            _dependencies.Add(dependency);
        }

        public void RemoveDependency(CategoryDependency dependency)
        {
            _dependencies.Remove(dependency);
        }
    }
}
