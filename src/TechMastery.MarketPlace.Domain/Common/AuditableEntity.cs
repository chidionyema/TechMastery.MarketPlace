namespace TechMastery.MarketPlace.Domain.Common
{
    public class AuditableEntity
    {
        protected AuditableEntity()
        {
            CreatedDate = DateTime.UtcNow;
        }
        public string? CreatedBy { get; set; } 
        public DateTime CreatedDate { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
