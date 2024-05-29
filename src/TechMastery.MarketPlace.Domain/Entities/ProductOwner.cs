
namespace TechMastery.MarketPlace.Domain.Entities
{
    public class ProductOwner //: AuditableEntity
    {
        public ProductOwner(string email) {

            Email = email;
            Id = Guid.NewGuid();

        }
        public Guid Id { get; protected set; }
        public Guid UserId { get; private set; }
        public string Email { get; private set; }
        // Other properties related to the owner
        public ICollection<Product> OwnedProducts { get; private set; } = new List<Product>();
    }
}

