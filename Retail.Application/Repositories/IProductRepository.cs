using Retail.Application.Models;

namespace Retail.Application.Repositories
{
    public interface IProductRepository
    {
        Task<bool> CreateAsync(Product product, CancellationToken token = default);
        Task<Product?> GetByIdAsync(int id, CancellationToken token = default);
        Task<IEnumerable<Product>> GetAllAsync(CancellationToken token = default);
    }
}
