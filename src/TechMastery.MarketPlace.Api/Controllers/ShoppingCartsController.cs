using Microsoft.AspNetCore.Mvc;
using MediatR;
using TechMastery.MarketPlace.Application.Features.Checkout.Commands;
using TechMastery.MarketPlace.Application.Features.Checkout.Queries;
using TechMastery.MarketPlace.Application.Features.Checkout.Handlers;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TechMastery.MarketPlace.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShoppingCartsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ShoppingCartsController> _logger;

        public ShoppingCartsController(IMediator mediator, ILogger<ShoppingCartsController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("add-item")]
        [Authorize]
        public async Task<IActionResult> AddItemToCart([FromBody] AddCartItem command)
        {
            var cartItemId = await _mediator.Send(command);
            return Ok(cartItemId);
        }

        [HttpGet("get-cart")]
        [Authorize]
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

        [HttpGet("get-cart-by-user")]
        [Authorize]
        public async Task<IActionResult> GetShoppingCartByUser()
        {
            // Extract the user's ID from the claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Invalid user token.");
            }

            var query = new GetShoppingCartByUserIdQuery { LoggedInUserId = Guid.Parse(userId) };
            var shoppingCart = await _mediator.Send(query);

            if (shoppingCart == null)
            {
                return NotFound();
            }

            return Ok(shoppingCart);
        }

        [HttpPut("update-item")]
        [Authorize]
        public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartItem command)
        {
            await _mediator.Send(command);
            return Ok();
        }

        [HttpDelete("remove-item")]
        [Authorize]
        public async Task<IActionResult> RemoveCartItem([FromQuery] Guid shoppingCartId, [FromQuery] Guid cartItemId)
        {
            var deleteCommand = new DeleteCartItem { CartItemId = cartItemId };
            await _mediator.Send(deleteCommand);
            return Ok();
        }
    }
}
