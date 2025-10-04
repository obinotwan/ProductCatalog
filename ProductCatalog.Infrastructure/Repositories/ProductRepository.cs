using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Interfaces;

namespace ProductCatalog.Infrastructure.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly Dictionary<string, List<int>> _nameIndex = new();

        protected override int GetId(Product entity) => entity.Id;

        protected override void SetId(Product entity, int id)
        {
            entity.Id = id;
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        public override Task<Product> AddAsync(Product entity)
        {
            var result = base.AddAsync(entity).Result;
            IndexProduct(result);
            return Task.FromResult(result);
        }

        /// <summary>
        /// Builds an inverted index for fast product name searches
        /// Using Dictionary (requirement from PDF)
        /// </summary>
        private void IndexProduct(Product product)
        {
            var words = product.Name.ToLowerInvariant().Split(' ');
            foreach (var word in words)
            {
                if (!_nameIndex.ContainsKey(word))
                    _nameIndex[word] = new List<int>();

                if (!_nameIndex[word].Contains(product.Id))
                    _nameIndex[word].Add(product.Id);
            }
        }

        public Task<IEnumerable<Product>> SearchByNameAsync(string searchTerm)
        {
            var results = _store
                .Where(p => p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return Task.FromResult<IEnumerable<Product>>(results);
        }

        public Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId)
        {
            var results = _store.Where(p => p.CategoryId == categoryId).ToList();
            return Task.FromResult<IEnumerable<Product>>(results);
        }
    }
}