using FluentValidation;
using Retail.Application.Mapping;
using Retail.Application.Models;
using Retail.Application.Repositories;
using Retail.Contracts.Product;

namespace Retail.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IValidator<GetAllProductsOptions> _optionsValidator;

        public ProductService(IProductRepository productRepository, IValidator<GetAllProductsOptions> optionsValidator)
        {
            _productRepository = productRepository;
            _optionsValidator = optionsValidator;
        }
        public Task<bool> CreateAsync(Product product, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteByIdAsync(int id, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsByIdAsync(int id, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Product>> GetAllAsync(GetAllProductsRequest request, CancellationToken token = default)
        {
            var options = request.MapToOptions();
            // should validate dto GetAllProductRequest instead
            await _optionsValidator.ValidateAndThrowAsync(options, cancellationToken: token);
            return await _productRepository.GetAllAsync(options, token);
        }

        public Task<Product?> GetByIdAsync(int id, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetCountAsync(string? name, string? sku, int categoryId, int categoryNumber, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(Product product, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
    }
}
