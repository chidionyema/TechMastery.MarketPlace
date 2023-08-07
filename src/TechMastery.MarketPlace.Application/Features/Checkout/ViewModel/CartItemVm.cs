namespace TechMastery.MarketPlace.Application.Features.Checkout.ViewModels
{
    public class CartItemVm
    {
        // Add properties that you want to include in the ViewModel
        public Guid ProductId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        // Other properties as needed
    }
}
