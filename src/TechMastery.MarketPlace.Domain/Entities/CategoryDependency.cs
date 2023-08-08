using System;
using TechMastery.MarketPlace.Domain.Common;

namespace TechMastery.MarketPlace.Domain.Entities
{
    public class CategoryDependency : AuditableEntity
    {
        public CategoryDependency() { }
        public CategoryDependency(Guid categoryDependencyId, Guid? categoryId, string? name, string? version, CategoryDependencyTypeEnum? dependencyType)
        {
            CategoryId = categoryId;
            Name = name;
            Version = version;
            DependencyTypeEnum = dependencyType;
            CategoryDependencyId = categoryDependencyId;
        }

        public Guid CategoryDependencyId { get; set; }
        public Guid? CategoryId { get; private set; }
        public Category? Category { get; private set; }
        public string? Name { get; private set; }
        public string? Version { get; private set; }
        public int DependencyTypeId { get; private set; }
        public CategoryDependencyTypeEnum? DependencyTypeEnum { get; private set; }
        public CategoryDependencyType? DependencyTypeEntity { get; private set; }
    }
}
