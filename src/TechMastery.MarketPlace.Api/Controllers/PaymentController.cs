using System;
using TechMastery.MarketPlace.Application.Contracts.Infrastructure;
using TechMastery.MarketPlace.Infrastructure.Payments.Models;
using Microsoft.AspNetCore.Mvc;
using TechMastery.MarketPlace.Application.Models.Payment;
using TechMastery.MarketPlace.Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace TechMastery.MarketPlace.Api.Controllers
{
    [Route("api/[controller]")]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("processpayment")]
        [Authorize]
        public async Task<ActionResult<PaymentResult>> ProcessPayment(
            [FromBody] PaymentInfo paymentInfo,
            CancellationToken ct)
        {
            var createdPayment = await _paymentService.ProcessPaymentAsync(paymentInfo, ct);

            return StatusCode(StatusCodes.Status200OK, createdPayment);
        }
    }
}

