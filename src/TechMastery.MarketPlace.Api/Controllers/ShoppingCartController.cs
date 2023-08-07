using Microsoft.AspNetCore.Mvc;
using TechMastery.MarketPlace.Application.Features.Checkout.Handlers;
using TechMastery.MarketPlace.Application.Features.Checkout.Dto;
using MediatR;
using TechMastery.MarketPlace.Application.Features.Checkout.Queries;
using TechMastery.MarketPlace.Application.Features.Checkout.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace TechMastery.MarketPlace.Api.Controllers
{
    [Route("api/shopping-cart")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ShoppingCartController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpGet("{userId}")]
        public async Task<ActionResult<ShoppingCartVm>> GetShoppingCart(Guid userId)
        {
            var query = new GetShoppingCartQuery { UserId = userId };
            var shoppingCartVm = await _mediator.Send(query);

            if (shoppingCartVm == null)
            {
                return NotFound($"Shopping cart not found for user with ID {userId}");
            }

            return Ok(shoppingCartVm);
        }

        [Authorize]
        [HttpPost("{userId}/items")]
        public async Task<IActionResult> AddToCart(Guid userId, [FromBody] CartItemDto cartItem)
        {
            var command = new AddItemToCart { ShoppingCartId = userId, CartItems = new List<CartItemDto> { cartItem } };
            var cartItemIds = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetShoppingCart), new { userId }, cartItemIds);
        }

        [Authorize]
        [HttpPut("{userId}/items/{productId}")]
        public async Task<IActionResult> UpdateCartItem(Guid userId, Guid productId, [FromBody] CartItemDto cartItem)
        {
            return NoContent();
        }
        [Authorize]
        [HttpDelete("{userId}/items/{productId}")]
        public async Task<IActionResult> RemoveCartItem(Guid userId, Guid productId)
        {
            return NoContent();
        }
    }
}
