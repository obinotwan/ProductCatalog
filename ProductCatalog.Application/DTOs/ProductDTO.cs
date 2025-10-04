using System;


namespace ProductCatalog.Application.DTOs
{
    public record ProductDTO (
        int Id,
        string Name,
        string Description,
        string SKU,
        decimal Price,
        int Quantity,
        int CategoryId,
        string CategoryName,
        DateTime CreatedAt,
        DateTime UpdatedAt
        );

    public record CreateProductDTO (
        string Name,
        string Description,
        string SKU,
        decimal Price,
        int Quantity,
        int CategoryId
        );

    public record UpdateProductDTO(
        string Name,
        string Description,
        string SKU,
        decimal Price,
        int Quantity,
        int CategoryId
        );

    public record SearchResultDTO<T>(
        T Item,
        double Score
        );
}
