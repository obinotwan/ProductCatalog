using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductCatalog.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// GET /api/categories - Get all categories (flat list)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            return Ok(await _categoryService.GetAllCategoriesAsync());
        }

        /// <summary>
        /// GET /api/categories/tree - Get categories as hierarchical tree structure
        /// Requirement: Build a category tree structure that supports hierarchical categories
        /// </summary>
        [HttpGet("tree")]
        public async Task<ActionResult<CategoryTreeDto>> GetCategoryTree()
        {
            return Ok(await _categoryService.GetCategoryTreeAsync());
        }

        /// <summary>
        /// POST /api/categories - Create a new category
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategory(CategoryDto dto)
        {
            var category = await _categoryService.CreateCategoryAsync(dto);
            return CreatedAtAction(nameof(GetCategories), new { id = category.Id }, category);
        }
    }
}