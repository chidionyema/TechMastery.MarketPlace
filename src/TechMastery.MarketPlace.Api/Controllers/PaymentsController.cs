
using Microsoft.AspNetCore.Mvc;
using TechMastery.MarketPlace.Application.Models.Payment;
using Microsoft.AspNetCore.Authorization;
using TechMastery.MarketPlace.Application.Features.Orders.Commands;
using MediatR;

namespace TechMastery.MarketPlace.Api.Controllers
{
    [Route("api/[controller]")]
    public class PaymentsController : Controller
    {
        private readonly IMediator _mediator;

        public PaymentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("processpayment")]
        [Authorize]
        public async Task<ActionResult<Unit>> ProcessPayment(
            [FromBody] Guid orderId, PaymentInfo paymentInfo,
            CancellationToken ct)
        {
            var createdPayment = await _mediator.Send(new PaymentCommand { OrderId = orderId, PaymentInfo = paymentInfo}, ct);

            return StatusCode(StatusCodes.Status200OK, createdPayment);
        }
    }
}

