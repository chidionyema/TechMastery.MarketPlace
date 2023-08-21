using MediatR;
using TechMastery.MarketPlace.Application.Contracts.Persistence;
using TechMastery.MarketPlace.Application.Exceptions;
using TechMastery.MarketPlace.Application.Features.Checkout.ViewModels;

namespace TechMastery.MarketPlace.Application.Features.Checkout.Queries
{
    public class GetShoppingCartQuery : IRequest<ShoppingCartVm>
    {
        public Guid ShoppingCartId { get; set; }
    }

    public class GetShoppingCartQueryHandler : IRequestHandler<GetShoppingCartQuery, ShoppingCartVm>
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;

        public GetShoppingCartQueryHandler(IShoppingCartRepository shoppingCartRepository)
        {
            _shoppingCartRepository = shoppingCartRepository ?? throw new ArgumentNullException(nameof(shoppingCartRepository));
        }

        public async Task<ShoppingCartVm> Handle(GetShoppingCartQuery query, CancellationToken cancellationToken)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var shoppingCart = await _shoppingCartRepository.GetByIdAsync(query.ShoppingCartId);

            if (shoppingCart == null)
            {
                throw new NotFoundException("shopping cart not found", query.ShoppingCartId);
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
