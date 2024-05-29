
using Microsoft.AspNetCore.Mvc;
using TechMastery.MarketPlace.Application.Features.Catgegory.ViewModel;
using TechMastery.MarketPlace.Application.Persistence.Contracts;

namespace TechMastery.MarketPlace.Api.Controllers
{
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CategoryListVm>>> GetBestSellingProducts()
        {
            var query = await _categoryRepository.ListAllAsync();
       
            return Ok(query.Select(s => new CategoryListVm { CategoryId = s.Id, Name = s.Name}));
        }
    }
}

