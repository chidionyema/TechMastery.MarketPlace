namespace TechMastery.MarketPlace.Application.Features.Checkout.ViewModels
{
    public class ShoppingCartVm
    {
        // Add properties that you want to include in the ViewModel
        public Guid UserId { get; set; }
        public List<CartItemVm> CartItems { get; set; }
        // Other properties as needed
    }
}
