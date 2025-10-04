using ProductCatalog.Application.DTOs;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Application.Validators
{
    public static class ProductValidator
    {
        public static ValidationResult Validate(CreateProductDTO product)
        {
            // Using C# pattern matching for validation (requirement from PDF)
            var errors = new List<string>();

            // Name validation using pattern matching
            var nameValidation = product.Name switch
            {
                null or "" => "Product name is required",
                { Length: > 200 } => "Product name must be 200 characters or less",
                _ => null
            };
            if (nameValidation != null) errors.Add(nameValidation);

            // SKU validation
            var skuValidation = product.SKU switch
            {
                null or "" => "SKU is required",
                { Length: < 3 } => "SKU must be at least 3 characters",
                _ => null
            };
            if (skuValidation != null) errors.Add(skuValidation);

            // Price validation with pattern matching
            var priceValidation = product.Price switch
            {
                < 0 => "Price cannot be negative",
                _ => null
            };
            if (priceValidation != null) errors.Add(priceValidation);

            // Quantity validation
            var quantityValidation = product.Quantity switch
            {
                < 0 => "Quantity cannot be negative",
                _ => null
            };
            if (quantityValidation != null) errors.Add(quantityValidation);

            return new ValidationResult(errors.Count == 0, errors);
        }
    }
    public record ValidationResult(bool IsValid, List<string> Errors);
}
