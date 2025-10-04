using System;

namespace ProductCatalog.Domain.Entities
{
    public class Product : IComparable<Product>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Category? Category { get; set; }

        public int CompareTo(Product? other)
        {
            if (other == null) return 1;

            // Primary sort by category, then by name
            var categoryComparison = CategoryId.CompareTo(other.CategoryId);
            return categoryComparison != 0
                ? categoryComparison
                : string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }
    }
}
