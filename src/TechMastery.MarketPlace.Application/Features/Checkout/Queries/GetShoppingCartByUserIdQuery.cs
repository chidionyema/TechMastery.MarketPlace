using MediatR;
using TechMastery.MarketPlace.Application.Exceptions;
using TechMastery.MarketPlace.Application.Features.Checkout.ViewModels;
using TechMastery.MarketPlace.Application.Persistence.Contracts;
namespace TechMastery.MarketPlace.Application.Features.Checkout.Queries
{
    public class GetShoppingCartByUserIdQuery : IRequest<ShoppingCartVm>
    {
        public Guid LoggedInUserId { get; set; }
    }

    public class GetShoppingCartByUserIdQueryHandler : IRequestHandler<GetShoppingCartByUserIdQuery, ShoppingCartVm>
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;

        public GetShoppingCartByUserIdQueryHandler(IShoppingCartRepository shoppingCartRepository)
        {
            _shoppingCartRepository = shoppingCartRepository ?? throw new ArgumentNullException(nameof(shoppingCartRepository));
        }

        public async Task<ShoppingCartVm> Handle(GetShoppingCartByUserIdQuery query, CancellationToken cancellationToken)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var shoppingCart = await _shoppingCartRepository.GetByUserIdAsync(query.LoggedInUserId);

            if (shoppingCart == null)
            {
                throw new NotFoundException("shopping cart for user not found", query.LoggedInUserId);
            }

            var shoppingCartVm = new ShoppingCartVm
            {
                UserId = shoppingCart.UserId,
                CartItems = shoppingCart.Items.Select(cartItem => new CartItemVm
                {
                    ProductId = cartItem.ProductId,
                    Price = cartItem.Price,
                    Quantity = cartItem.Quantity
                }).ToList(),
            };

            return shoppingCartVm;
        }
    }
}
