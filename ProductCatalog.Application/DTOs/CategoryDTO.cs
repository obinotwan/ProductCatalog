using System.Collections.Generic;

namespace ProductCatalog.Application.DTOs
{
    public record CategoryDto(
        int Id,
        string Name,
        string Description,
        int? ParentCategoryId
    );

    public record CategoryTreeDto(
        int Id,
        string Name,
        string Description,
        int? ParentCategoryId,
        List<CategoryTreeDto> SubCategories
    );
}