using System.Collections.Generic;
using System.Linq;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.Extensions
{
    public static class ProductQueryExtensions
    {
        public static IEnumerable<Product> FilterByPriceRange(
            this IEnumerable<Product> products,
            decimal? minPrice,
            decimal? maxPrice)
        {
            if (minPrice.HasValue)
                products = products.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                products = products.Where(p => p.Price <= maxPrice.Value);

            return products;
        }

        public static IEnumerable<Product> FilterInStock(
            this IEnumerable<Product> products)
        {
            return products.Where(p => p.Quantity > 0);
        }

        public static IEnumerable<Product> SortByAvailability(
            this IEnumerable<Product> products)
        {
            return products.OrderByDescending(p => p.Quantity > 0)
                          .ThenBy(p => p.Name);
        }
    }
}