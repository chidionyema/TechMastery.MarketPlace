using TechMastery.MarketPlace.Domain.Common;

namespace TechMastery.MarketPlace.Domain.Entities
{
    public class CategoryDependency : AuditableEntity
    {
        public CategoryDependency() { }
        public Guid CategoryDependencyId { get; protected set; }
        public Guid CategoryId { get; protected set; }
        public Guid DependencyId { get; private set; }
        public Category? Category { get; private set; }
        public Dependency Dependency { get; private set; }
    }
}
