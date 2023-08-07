using MediatR;
using TechMastery.MarketPlace.Application.Contracts.Persistence;
using TechMastery.MarketPlace.Application.Features.Orders.ViewModel;

namespace TechMastery.MarketPlace.Application.Features.Orders.Queries
{
    public class GetByOrderIdQuery : IRequest<OrderListVm>
    {
        public Guid OrderId { get; set; }
    }

    public class GetByOrderIdHandler : IRequestHandler<GetByOrderIdQuery, OrderListVm>
    {
        private readonly IOrderRepository _orderRepository;

        public GetByOrderIdHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public async Task<OrderListVm> Handle(GetByOrderIdQuery query, CancellationToken cancellationToken)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var order = await _orderRepository.GetByCartId(query.OrderId);
            var orderVm = new OrderListVm
            {
                OrderId = order!.OrderId,
                UserId = order.UserId,
                OrderStatus = order.OrderStatus,
                OrderPlaced = order.OrderPlaced,
                BuyerUsername = order.BuyerUsername,
                PurchaseDate = order.PurchaseDate,
                OrderPaid = order.OrderPaid,
                OrderLineItems = order.OrderLineItems.Select(orderItem => new OrderLineItemVm(orderItem.UnitPrice, orderItem.ProductId, orderItem.Quantity)).ToList()
            };

            return orderVm;
        }
    }
}
