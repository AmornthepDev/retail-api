using Retail.Application.Models;
using Retail.Contracts.Product;

namespace Retail.Application.Services
{
    public interface IProductService
    {
        Task<bool> CreateAsync(Product product, CancellationToken token = default);
        Task<Product?> GetByIdAsync(int id, CancellationToken token = default);
        Task<IEnumerable<Product>> GetAllAsync(GetAllProductsRequest request, CancellationToken token = default);
        Task<bool> UpdateAsync(Product product, CancellationToken token = default);
        Task<bool> DeleteByIdAsync(int id, CancellationToken token = default);
        Task<bool> ExistsByIdAsync(int id, CancellationToken token = default);
        Task<int> GetCountAsync(string? name, string? sku, int categoryId, int categoryNumber, CancellationToken token = default);
    }
}
