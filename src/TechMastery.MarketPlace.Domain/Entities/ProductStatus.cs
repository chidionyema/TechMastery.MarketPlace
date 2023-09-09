using System;
namespace TechMastery.MarketPlace.Domain.Entities
{
	public class ProductStatus
    {
        public int Id { get;  set; }
        public string? Name { get;  set; }
        public ProductStatusEnum StatusEnum { get; private set; }
    }
}

