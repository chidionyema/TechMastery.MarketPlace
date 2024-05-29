using System;
using TechMastery.MarketPlace.Domain.Common;

namespace TechMastery.MarketPlace.Domain.Entities
{
	public class ProductStatus : AuditableEntity
    {
        public string? Name { get;  set; }
        public ProductStatusEnum StatusEnum { get; private set; }
    }
}

