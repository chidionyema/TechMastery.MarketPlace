namespace TechMastery.MarketPlace.Application.Features.Orders.ViewModel
{
    public class OrderLineItemVm
    {
        public Guid OrderLineItemId { get; set; }
        public decimal UnitPrice { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }

        public OrderLineItemVm(decimal unitPrice, Guid productId, int quantity, string name = "", string description = "")
        {
            UnitPrice = unitPrice;
            ProductId = productId;
            Quantity = quantity;
            ProductName = name;
            Description = description;
        }
    }
}