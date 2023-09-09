using TechMastery.MarketPlace.Domain.Common;

namespace TechMastery.MarketPlace.Domain.Entities
{
	public class ProductLicense : AuditableEntity
	{
		public Guid ProductLicenseId { get; protected set; }
		public string? Name { get; private set; }
	}
}
