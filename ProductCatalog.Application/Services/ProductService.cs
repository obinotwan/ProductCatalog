using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Exceptions;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Application.Validators;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Interfaces;

namespace ProductCatalog.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductSearchEngine _searchEngine;
        private readonly ICacheService _cacheService;

        public ProductService(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IProductSearchEngine searchEngine,
            ICacheService cacheService)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _searchEngine = searchEngine;
            _cacheService = cacheService;
        }

        public async Task<ProductDTO?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return product != null ? MapToDto(product) : null;
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync(int page, int pageSize)
        {
            var cacheKey = $"products_page_{page}_size_{pageSize}";
            var cached = _cacheService.Get<IEnumerable<ProductDTO>>(cacheKey);

            if (cached != null)
                return cached;

            var products = await _productRepository.GetAllAsync();
            var dtos = products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(MapToDto)
                .ToList();

            _cacheService.Set(cacheKey, dtos, TimeSpan.FromMinutes(5));
            return dtos;
        }

        public async Task<IEnumerable<ProductDTO>> SearchProductsAsync(string searchTerm)
        {
            var cacheKey = $"search_{searchTerm}";
            var cached = _cacheService.Get<IEnumerable<ProductDTO>>(cacheKey);

            if (cached != null)
                return cached;

            var allProducts = await _productRepository.GetAllAsync();
            var searchResults = await _searchEngine.SearchAsync(searchTerm, allProducts);

            var dtos = searchResults
                .OrderByDescending(r => r.Score)
                .Select(r => MapToDto(r.Item))
                .ToList();

            _cacheService.Set(cacheKey, dtos, TimeSpan.FromMinutes(2));
            return dtos;
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await _productRepository.GetByCategoryAsync(categoryId);
            return products.Select(MapToDto);
        }

        public async Task<ProductDTO> CreateProductAsync(CreateProductDTO dto)
        {
            var validation = ProductValidator.Validate(dto);
            if (!validation.IsValid)
                throw new ValidationException(string.Join(", ", validation.Errors));

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                SKU = dto.SKU,
                Price = dto.Price,
                Quantity = dto.Quantity,
                CategoryId = dto.CategoryId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var created = await _productRepository.AddAsync(product);
            _cacheService.Clear();

            return MapToDto(created);
        }

        public async Task UpdateProductAsync(int id, UpdateProductDTO dto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new NotFoundException($"Product with ID {id} not found");

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.SKU = dto.SKU;
            product.Price = dto.Price;
            product.Quantity = dto.Quantity;
            product.CategoryId = dto.CategoryId;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
            _cacheService.Clear();
        }

        public async Task DeleteProductAsync(int id)
        {
            await _productRepository.DeleteAsync(id);
            _cacheService.Clear();
        }

        private static ProductDTO MapToDto(Product product)
        {
            return new ProductDTO(
                product.Id,
                product.Name,
                product.Description,
                product.SKU,
                product.Price,
                product.Quantity,
                product.CategoryId,
                product.Category?.Name ?? string.Empty,
                product.CreatedAt,
                product.UpdatedAt
            );
        }
    }
}