using ProductCatalog.Application.Services;
using ProductCatalog.Domain.Entities;
using Xunit;

namespace ProductCatalog.Tests
{
    public class ProductSearchEngineTests
    {
        private readonly ProductSearchEngine _searchEngine;
        private readonly List<Product> _testProducts;

        public ProductSearchEngineTests()
        {
            _searchEngine = new ProductSearchEngine();

            // Setup test data
            _testProducts = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Gaming Laptop",
                    Description = "High-performance gaming laptop",
                    SKU = "LAP-001",
                    Price = 1499.99m,
                    Quantity = 10,
                    CategoryId = 1
                },
                new Product
                {
                    Id = 2,
                    Name = "Business Laptop",
                    Description = "Professional laptop for work",
                    SKU = "LAP-002",
                    Price = 899.99m,
                    Quantity = 15,
                    CategoryId = 1
                },
                new Product
                {
                    Id = 3,
                    Name = "Wireless Mouse",
                    Description = "Ergonomic wireless mouse",
                    SKU = "MOU-001",
                    Price = 29.99m,
                    Quantity = 50,
                    CategoryId = 2
                },
                new Product
                {
                    Id = 4,
                    Name = "Mechanical Keyboard",
                    Description = "RGB mechanical keyboard",
                    SKU = "KEY-001",
                    Price = 149.99m,
                    Quantity = 30,
                    CategoryId = 2
                }
            };
        }

        [Fact]
        public async Task SearchAsync_WithExactMatch_ReturnsHighScoreResults()
        {
            // Arrange
            var query = "laptop";

            // Act
            var results = await _searchEngine.SearchAsync(query, _testProducts);
            var resultList = results.ToList();

            // Assert
            Assert.NotEmpty(resultList);
            Assert.Equal(2, resultList.Count); // Should find both laptops
            Assert.All(resultList, r => Assert.Contains("laptop", r.Item.Name.ToLower()));
        }

        [Fact]
        public async Task SearchAsync_WithFuzzyMatch_FindsTypoResults()
        {
            // Arrange
            var query = "lptop"; // Typo: missing 'a'

            // Act
            var results = await _searchEngine.SearchAsync(query, _testProducts);
            var resultList = results.ToList();

            // Assert
            Assert.NotEmpty(resultList);
            // Should still find laptop products despite typo
            var hasLaptops = resultList.Any(r => r.Item.Name.Contains("Laptop"));
            Assert.True(hasLaptops, "Fuzzy search should find 'Laptop' when searching for 'lptop'");
        }

        [Fact]
        public async Task SearchAsync_WithEmptyQuery_ReturnsEmpty()
        {
            // Arrange
            var query = "";

            // Act
            var results = await _searchEngine.SearchAsync(query, _testProducts);

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public async Task SearchAsync_WithNoMatch_ReturnsEmpty()
        {
            // Arrange
            var query = "smartphone"; // Not in our test data

            // Act
            var results = await _searchEngine.SearchAsync(query, _testProducts);

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public async Task SearchAsync_OrdersByScore_HighestFirst()
        {
            // Arrange
            var query = "laptop";

            // Act
            var results = await _searchEngine.SearchAsync(query, _testProducts);
            var resultList = results.ToList();

            // Assert
            Assert.NotEmpty(resultList);
            // Verify results are ordered by score (descending)
            for (int i = 0; i < resultList.Count - 1; i++)
            {
                Assert.True(resultList[i].Score >= resultList[i + 1].Score,
                    "Results should be ordered by score (highest first)");
            }
        }

        [Fact]
        public async Task SearchAsync_SearchesByDescription_FindsResults()
        {
            // Arrange
            var query = "ergonomic";

            // Act
            var results = await _searchEngine.SearchAsync(query, _testProducts);
            var resultList = results.ToList();

            // Assert
            Assert.NotEmpty(resultList);
            var mouseFound = resultList.Any(r => r.Item.Name == "Wireless Mouse");
            Assert.True(mouseFound, "Should find mouse by description");
        }

        [Fact]
        public async Task SearchAsync_WithMultipleTypos_StillFindsResults()
        {
            // Arrange
            var query = "keyborad"; // Typo: 'a' and 'r' swapped

            // Act
            var results = await _searchEngine.SearchAsync(query, _testProducts);
            var resultList = results.ToList();

            // Assert
            Assert.NotEmpty(resultList);
            var keyboardFound = resultList.Any(r => r.Item.Name.Contains("Keyboard"));
            Assert.True(keyboardFound, "Fuzzy search should handle multiple character differences");
        }
    }
}