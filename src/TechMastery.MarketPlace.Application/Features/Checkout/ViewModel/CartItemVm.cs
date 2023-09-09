namespace TechMastery.MarketPlace.Application.Features.Checkout.ViewModels
{
    public class CartItemVm
    {
        public Guid CartItemId { get; set; }
        public Guid ProductId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        // Other properties as needed
    }
}
