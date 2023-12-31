﻿
using Microsoft.AspNetCore.Mvc;
using TechMastery.MarketPlace.Application.Contracts.Persistence;
using TechMastery.MarketPlace.Application.Features.Catgegory.ViewModel;

namespace TechMastery.MarketPlace.Api.Controllers
{
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CategoryListVm>>> GetBestSellingProducts()
        {
            var query = await _categoryRepository.ListAllAsync();
       
            return Ok(query.Select(s => new CategoryListVm { CategoryId = s.CategoryId, Name = s.Name}));
        }
    }
}

