using TechMastery.MarketPlace.Domain.Common;

namespace TechMastery.MarketPlace.Domain.Entities
{
	public class ProductLicense : AuditableEntity
	{
		public Guid Id { get; protected set; }
		public string? Name { get; private set; }
	}
}
