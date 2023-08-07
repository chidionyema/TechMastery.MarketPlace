using System;
namespace TechMastery.MarketPlace.Domain.Entities
{
    public class ProductOwner 
    {
        public Guid ProductOwnerId { get; protected set; }
        public Guid UserId { get; private set; }
        public string Email { get; private set; }
        // Other properties related to the owner
        public ICollection<Product> OwnedProducts { get; private set; } = new List<Product>();
    }
}

