namespace TechMastery.MarketPlace.Domain.Common
{
    public interface IEntityWithGuid
    {
        Guid Id { get; }
    }

    public class AuditableEntity : IEntityWithGuid
    {
        protected AuditableEntity()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.UtcNow;
        }
        public Guid Id { get; protected set; }
        public string? CreatedBy { get; set; } 
        public DateTime CreatedDate { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
