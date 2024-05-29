using MediatR;
using TechMastery.MarketPlace.Application.Features.Orders.ViewModel;
using TechMastery.MarketPlace.Application.Persistence.Contracts;
using TechMastery.MarketPlace.Domain.Entities;

namespace TechMastery.MarketPlace.Application.Features.Orders.Queries
{
    public class GetByCartIdQuery : IRequest<OrderListVm>
    {
        public Guid ShoppingCartId { get; set; }
    }

    public class GetByCartIdHandler : IRequestHandler<GetByCartIdQuery, OrderListVm>
    {
        private readonly IOrderRepository _orderRepository;

        public GetByCartIdHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public async Task<OrderListVm> Handle(GetByCartIdQuery query, CancellationToken cancellationToken)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var order = await _orderRepository.GetByCartId(query.ShoppingCartId);

            if (order == null)
            {
                throw new KeyNotFoundException($"Order associated with ShoppingCartId {query.ShoppingCartId} not found.");
            }

            return MapToViewModel(order);
        }

        private OrderListVm MapToViewModel(Order order)
        {
            var orderVm = new OrderListVm
            {
                OrderId = order.Id,
                UserId = order.UserId,
                OrderStatus = order.OrderStatus,
                OrderPlaced = order.OrderPlaced,
                BuyerUsername = order.BuyerUsername,
                PurchaseDate = order.PurchaseDate,
                OrderPaid = order.OrderPaid,
                OrderLineItems = order.OrderLineItems.Select(orderItem =>
                    new OrderLineItemVm(orderItem.UnitPrice, orderItem.ProductId, orderItem.Quantity)).ToList()
            };

            return orderVm;
        }
    }
}
