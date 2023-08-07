using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechMastery.MarketPlace.Application.Features.Product.Queries.GetProductsList;
using TechMastery.MarketPlace.Application.Features.ProductListing.Handlers;

namespace TechMastery.MarketPlace.Api.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddListing([FromBody] AddOrUpdateListing command)
        {
            var productId = await _mediator.Send(command);
                    
            return Ok(productId);
        }

        [HttpGet("best-selling")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductListVm>>> GetBestSellingProducts()
        {
            var query = new GetBestSellingProductsQuery();
            var products = await _mediator.Send(query);

            return Ok(products);
        }

        [HttpGet("categories/{categoryId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductListVm>>> GetProductsByCategory(Guid categoryId)
        {
            var query = new GetProductsByCategoryQuery
            {
                CategoryId = categoryId
            };
            var products = await _mediator.Send(query);

            return Ok(products);
        }

        [HttpGet("categories/{categoryId}/subcategories/{subcategoryId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductListVm>>> GetProductsBySubcategory(Guid categoryId, Guid subcategoryId)
        {
            var query = new GetProductsBySubCategoryQuery
            {
                CategoryId = categoryId,
                SubCategoryId = subcategoryId
            };
            var products = await _mediator.Send(query);

            return Ok(products);
        }

        [HttpGet("latest")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductListVm>>> GetLatestProducts()
        {
            var query = new GetLatestProductsQuery();
            var products = await _mediator.Send(query);

            return Ok(products);
        }
    }
}
