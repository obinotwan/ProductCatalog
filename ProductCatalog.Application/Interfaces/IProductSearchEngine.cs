using System.Collections.Generic;
using System.Threading.Tasks;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.Interfaces
{
    public interface IProductSearchEngine
    {
        Task<IEnumerable<SearchResultDTO<Product>>> SearchAsync(
            string query,
            IEnumerable<Product> products
        );
    }
}