using TechMastery.MarketPlace.Domain.Common;

namespace TechMastery.MarketPlace.Domain.Entities
{
    public class ProductDependency : AuditableEntity
    {
        public ProductDependency () { }

        public ProductDependency(Guid dependencyId) {

            DependencyId = dependencyId;
        }
        public Guid ProductDependencyId { get; protected set; }
        public Guid ProductId { get; private set; }
        public Guid DependencyId { get; private set; }
        public Product? Product { get; private set; }
        public Dependency Dependency { get; private set; }
    }
}
