using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MediatR;
using System;
using System.Threading.Tasks;
using TechMastery.MarketPlace.Application.Features.Checkout.Commands;
using TechMastery.MarketPlace.Application.Features.Checkout.Queries;
using TechMastery.MarketPlace.Application.Features.Checkout.Handlers;

namespace TechMastery.MarketPlace.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ShoppingCartController> _logger;

        public ShoppingCartController(IMediator mediator, ILogger<ShoppingCartController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("add-item")]
        public async Task<IActionResult> AddItemToCart([FromBody] AddCartItem command)
        {
            var cartItemIds = await _mediator.Send(command);
            return Ok(cartItemIds);
        }

        [HttpGet("get-cart")]
        public async Task<IActionResult> GetShoppingCart([FromQuery] Guid shoppingCartId)
        {
            var query = new GetShoppingCartQuery { ShoppingCartId = shoppingCartId };
            var shoppingCart = await _mediator.Send(query);

            if (shoppingCart == null)
            {
                return NotFound();
            }

            return Ok(shoppingCart);
        }

        [HttpPut("update-item")]
        public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartItem command)
        {
            await _mediator.Send(command);
            return Ok();
        }

        [HttpDelete("remove-item")]
        public async Task<IActionResult> RemoveCartItem([FromQuery] Guid shoppingCartId, [FromQuery] Guid cartItemId)
        {
            var deleteCommand = new DeleteCartItem { CartItemId = cartItemId };
            await _mediator.Send(deleteCommand);
            return Ok();
        }

        // Other actions for updating cart items, removing items, etc.
    }
}
