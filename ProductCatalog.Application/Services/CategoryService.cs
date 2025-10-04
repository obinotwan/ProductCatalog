using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Interfaces;

namespace ProductCatalog.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return categories.Select(c => new CategoryDto(
                c.Id,
                c.Name,
                c.Description,
                c.ParentCategoryId
            ));
        }

        public async Task<CategoryTreeDto> GetCategoryTreeAsync()
        {
            var allCategories = (await _categoryRepository.GetAllAsync()).ToList();
            var rootCategories = allCategories.Where(c => c.ParentCategoryId == null);

            var tree = new CategoryTreeDto(
                0,
                "Root",
                "Root category",
                null,
                rootCategories.Select(c => BuildTree(c, allCategories)).ToList()
            );

            return tree;
        }

        private CategoryTreeDto BuildTree(Category category, List<Category> allCategories)
        {
            var children = allCategories
                .Where(c => c.ParentCategoryId == category.Id)
                .Select(c => BuildTree(c, allCategories))
                .ToList();

            return new CategoryTreeDto(
                category.Id,
                category.Name,
                category.Description,
                category.ParentCategoryId,
                children
            );
        }

        public async Task<CategoryDto> CreateCategoryAsync(CategoryDto dto)
        {
            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description,
                ParentCategoryId = dto.ParentCategoryId
            };

            var created = await _categoryRepository.AddAsync(category);
            return new CategoryDto(
                created.Id,
                created.Name,
                created.Description,
                created.ParentCategoryId
            );
        }
    }
}