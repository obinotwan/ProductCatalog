using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductCatalog.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// GET /api/products - Get all products with pagination, filtering, and search
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? categoryId = null,
            [FromQuery] string? search = null)
        {
            // If search term provided, use search
            if (!string.IsNullOrWhiteSpace(search))
                return Ok(await _productService.SearchProductsAsync(search));

            // If category filter provided, filter by category
            if (categoryId.HasValue)
                return Ok(await _productService.GetProductsByCategoryAsync(categoryId.Value));

            // Otherwise, return paginated results
            return Ok(await _productService.GetAllProductsAsync(page, pageSize));
        }

        /// <summary>
        /// GET /api/products/{id} - Get a single product by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            return product != null ? Ok(product) : NotFound();
        }

        /// <summary>
        /// POST /api/products - Create a new product
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ProductDTO>> CreateProduct(CreateProductDTO dto)
        {
            var product = await _productService.CreateProductAsync(dto);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        /// <summary>
        /// PUT /api/products/{id} - Update an existing product
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, UpdateProductDTO dto)
        {
            await _productService.UpdateProductAsync(id, dto);
            return NoContent();
        }

        /// <summary>
        /// DELETE /api/products/{id} - Delete a product
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteProductAsync(id);
            return NoContent();
        }

        [HttpGet("custom-search")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> CustomSearch()
        {
            var query = Request.Query["q"].ToString();
            var minPrice = decimal.TryParse(Request.Query["minPrice"], out var min) ? min : (decimal?)null;
            var maxPrice = decimal.TryParse(Request.Query["maxPrice"], out var max) ? max : (decimal?)null;

            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query parameter 'q' is required");

            var results = await _productService.SearchProductsAsync(query);

            // Apply price filters manually
            var filtered = results.Where(p =>
                (!minPrice.HasValue || p.Price >= minPrice.Value) &&
                (!maxPrice.HasValue || p.Price <= maxPrice.Value)
            );

            return Ok(filtered);
        }
    }
}