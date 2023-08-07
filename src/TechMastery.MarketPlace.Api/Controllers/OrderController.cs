using TechMastery.MarketPlace.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using TechMastery.MarketPlace.Application.Features.Orders.Queries;
using MediatR;
using TechMastery.MarketPlace.Application.Features.Orders.ViewModel;
using TechMastery.MarketPlace.Application.Features.Orders.Commands;
using Microsoft.AspNetCore.Authorization;

namespace TechMastery.MarketPlace.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpGet("{orderId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<OrderListVm>> GetOrderSummary(Guid orderId)
        {
            var query = new GetByOrderIdQuery
            {
                OrderId = orderId
            };

            var orderListVm = await _mediator.Send(query);

            return orderListVm;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<List<Guid>>> CreateOrder([FromBody] CreateOrder command)
        {
            var orderId = await _mediator.Send(command);

            return Ok(orderId);
        }

        [Authorize]
        [HttpPost("process/{cartId}")]
        public async Task<ActionResult<Order>> CompleteOrder([FromBody] ProcessOrder command)
        {
            var orderId = await _mediator.Send(command);

            return Ok(orderId);
        }
    }
}

