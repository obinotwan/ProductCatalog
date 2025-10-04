using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductCatalog.Application.Services
{
    public class ProductSearchEngine : IProductSearchEngine
    {
        //
        public Task<IEnumerable<SearchResultDTO<Product>>> SearchAsync(string query, IEnumerable<Product> products)
        {
            if (string.IsNullOrEmpty(query))
                return Task.FromResult(Enumerable.Empty<SearchResultDTO<Product>>());

            var queryLower = query.ToLowerInvariant();
            var results = new List<SearchResultDTO<Product>>();

            foreach (var product in products) {
                var score = CalculateScore(queryLower, product);
                if (score > 0) {
                    results.Add(new SearchResultDTO<Product>(product, score));
                }
            
            }

            var sortedResults = results.OrderByDescending(r => r.Score);
            return Task.FromResult(sortedResults.AsEnumerable());
        }

        private double CalculateScore(string query, Product product)
        {
            double score = 0;

            // Name field - highest weight
            score += ScoreField(query, product.Name, weights: new FieldWeights
            {
                ExactMatch = 100,
                StartsWith = 50,
                Contains = 30,
                FuzzyMatch = 20
            });

            // Description field - medium weight
            score += ScoreField(query, product.Description, weights: new FieldWeights
            {
                ExactMatch = 20,
                StartsWith = 10,
                Contains = 10,
                FuzzyMatch = 5
            });

            // SKU field - medium weight
            score += ScoreField(query, product.SKU, weights: new FieldWeights
            {
                ExactMatch = 30,
                StartsWith = 15,
                Contains = 15,
                FuzzyMatch = 10
            });

            return score;
        }

        private double ScoreField(string query, string fieldValue, FieldWeights weights)
        {
            if (string.IsNullOrWhiteSpace(fieldValue))
                return 0;

            var fieldLower = fieldValue.ToLowerInvariant();

            // 1. Exact match on full field - highest score
            if (fieldLower.Equals(query, StringComparison.OrdinalIgnoreCase))
                return weights.ExactMatch;

            // 2. Starts with - high score
            if (fieldLower.StartsWith(query, StringComparison.OrdinalIgnoreCase))
                return weights.StartsWith;

            // 3. Contains - medium score
            if (fieldLower.Contains(query, StringComparison.OrdinalIgnoreCase))
                return weights.Contains;

            // 4. Fuzzy match - check against individual words AND full field
            double bestFuzzyScore = 0;

            // Split field into words and check each word
            var words = fieldLower.Split(new[] { ' ', '-', '_' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                var wordScore = FuzzyMatch(query, word);
                if (wordScore > bestFuzzyScore)
                    bestFuzzyScore = wordScore;
            }

            // Also check against full field (for single-word searches)
            var fullFieldScore = FuzzyMatch(query, fieldLower);
            if (fullFieldScore > bestFuzzyScore)
                bestFuzzyScore = fullFieldScore;

            return bestFuzzyScore * weights.FuzzyMatch;
        }

        private double FuzzyMatch(string query, string target)
        {
            var distance = LevenshteinDistance(query, target);
            var maxLength = Math.Max(query.Length, target.Length);

            if (maxLength == 0) return 1.0;

            // Convert distance to similarity (0 distance = 1.0 similarity)
            var similarity = 1.0 - ((double)distance / maxLength);

            Console.WriteLine($"Fuzzy: '{query}' vs '{target}' = distance:{distance}, similarity:{similarity:F2}");

            // Only return score if similarity is above threshold
            return similarity > 0.6 ? similarity : 0;
        }

        private int LevenshteinDistance(string source, string target)
        {
            // Handle edge cases
            if (string.IsNullOrEmpty(source))
                return target?.Length ?? 0;

            if (string.IsNullOrEmpty(target))
                return source.Length;

            var sourceLength = source.Length;
            var targetLength = target.Length;

            // Create a matrix to store distances
            // matrix[i,j] = distance between first i chars of source and first j chars of target
            var matrix = new int[sourceLength + 1, targetLength + 1];

            // Initialize first column (distance from empty string)
            for (var i = 0; i <= sourceLength; i++)
                matrix[i, 0] = i;

            // Initialize first row (distance from empty string)
            for (var j = 0; j <= targetLength; j++)
                matrix[0, j] = j;

            // Fill in the rest of the matrix
            for (var i = 1; i <= sourceLength; i++)
            {
                for (var j = 1; j <= targetLength; j++)
                {
                    // Cost is 0 if characters match, 1 if they don't
                    var cost = target[j - 1] == source[i - 1] ? 0 : 1;

                    // Calculate minimum of three operations:
                    matrix[i, j] = Math.Min(
                        Math.Min(
                            matrix[i - 1, j] + 1,      // Deletion
                            matrix[i, j - 1] + 1),     // Insertion
                        matrix[i - 1, j - 1] + cost    // Substitution
                    );
                }
            }

            // Bottom-right cell contains the final distance
            return matrix[sourceLength, targetLength];
        }
        private class FieldWeights
        {
            public double ExactMatch { get; set; }
            public double StartsWith { get; set; }
            public double Contains { get; set; }
            public double FuzzyMatch { get; set; }
        }
    }
}
