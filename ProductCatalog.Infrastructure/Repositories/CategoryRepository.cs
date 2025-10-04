using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Interfaces;

namespace ProductCatalog.Infrastructure.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        protected override int GetId(Category entity) => entity.Id;

        protected override void SetId(Category entity, int id)
        {
            entity.Id = id;
        }

        public Task<IEnumerable<Category>> GetRootCategoriesAsync()
        {
            var roots = _store.Where(c => c.ParentCategoryId == null).ToList();
            return Task.FromResult<IEnumerable<Category>>(roots);
        }

        public Task<IEnumerable<Category>> GetSubCategoriesAsync(int parentId)
        {
            var subs = _store.Where(c => c.ParentCategoryId == parentId).ToList();
            return Task.FromResult<IEnumerable<Category>>(subs);
        }
    }
}