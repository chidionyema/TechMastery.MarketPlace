using TechMastery.MarketPlace.Domain.Common;

namespace TechMastery.MarketPlace.Domain.Entities
{
    public class Dependency : AuditableEntity
    {
        public Dependency() { }
        public Dependency(string name, string version, DependencyTypeEnum dependencyType)
        {
            Name = name;
            Version = version;
            DependencyTypeEnum = dependencyType;
        }

        public Guid DependencyId { get; protected set; }
        public string Name { get; private set; }
        public string Version { get; private set; }
        public int DependencyTypeId { get; private set; }
        public DependencyTypeEnum DependencyTypeEnum { get; private set; }
        public DependencyType? DependencyType { get; private set; }

        public void UpdateVersionAndType(string version, DependencyTypeEnum dependencyType)
        {
            Version = version;
            DependencyTypeEnum = dependencyType;
        }
    }
}
