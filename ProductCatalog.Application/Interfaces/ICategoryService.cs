using System.Collections.Generic;
using System.Threading.Tasks;
using ProductCatalog.Application.DTOs;

namespace ProductCatalog.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryTreeDto> GetCategoryTreeAsync();
        Task<CategoryDto> CreateCategoryAsync(CategoryDto dto);
    }
}