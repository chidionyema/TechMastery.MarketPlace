using TechMastery.MarketPlace.Domain.Common;

namespace TechMastery.MarketPlace.Domain.Entities
{
    public class OrderLineItem : AuditableEntity
    {
        public Guid OrderLineItemId { get; protected set; }
        public decimal UnitPrice { get; set; }
        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }
        public int Quantity { get; set; }

        public OrderLineItem() { }

        public OrderLineItem(decimal unitPrice, Guid productId, int quantity, string name = "", string description = "")
        {
            UnitPrice = unitPrice;
            ProductId = productId;
            Quantity = quantity;
            ProductName = name;
            Description = description;
        }

        public string GetProductArtifactBlobName()
        {
            if (Product != null && Product.Artifacts != null)
            {
                var artifact = Product.Artifacts.FirstOrDefault(a => a.ArtifactType == ProductArtifactTypeEnum.Artifact);
                if (artifact != null)
                {
                    return artifact.BlobUrl;
                }
            }

            throw new InvalidOperationException("Blob URL for product artifact is not available.");
        }
    }
}
