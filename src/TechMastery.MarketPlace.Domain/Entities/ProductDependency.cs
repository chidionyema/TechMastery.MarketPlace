using TechMastery.MarketPlace.Domain.Common;

namespace TechMastery.MarketPlace.Domain.Entities
{
    public class ProductDependency : AuditableEntity
    {
        public ProductDependency () { }
        public ProductDependency(string name, string version, ProductDependencyTypeEnum dependencyType)
        {
            Name = name;
            Version = version;
            DependencyTypeEnum = dependencyType;
        }

        public Guid ProductDependencyId { get; protected set; }
        public Guid ProductId { get; private set; }
        public Product? Product { get; private set; }
        public string Name { get; private set; }
        public string Version { get; private set; }
        public int DependencyTypeId { get; private set; }
        public ProductDependencyTypeEnum DependencyTypeEnum { get; private set; }
        public ProductDependencyType? DependencyType { get; private set; }

        public void UpdateVersionAndType(string version, ProductDependencyTypeEnum dependencyType)
        {
            Version = version;
            DependencyTypeEnum = dependencyType;
        }
    }
}
