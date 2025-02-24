using Dapper;
using Retail.Application.Database;
using Retail.Application.Enums;
using Retail.Application.Models;
using Retail.Application.Repositories;

namespace Retail.Application.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public ProductRepository(IDbConnectionFactory _dbConnectionFactory)
        {
            this._dbConnectionFactory = _dbConnectionFactory;
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

        public async Task<IEnumerable<Product>> GetAllAsync(GetAllProductsOptions options, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);

            var orderClause = string.Empty;
            if (options.SortColumn is not null)
            {
                orderClause = $"""
                    ORDER BY p.{options.SortColumn} {(options.SortOrder == SortOrder.Ascending ? "ASC" : "DESC")}
                    """;
            }

            var sql = $"""
                SELECT 
                    p.Id, p.Sku, p.Name, p.Description,
                    pv.Id as VariantId, pv.Size, pv.Unit, pv.Price, pv.Quantity,
                    c.Id as CategoryId, c.Name as CategoryName,
                    sc.Id as SubCategoryId, sc.Name as SubCategoryName
                FROM Products p
                LEFT JOIN ProductVariants pv ON pv.ProductId = p.Id
                LEFT JOIN ProductCategories pc ON pc.ProductId = p.Id
                LEFT JOIN Categories c ON c.Id = pc.CategoryId
                LEFT JOIN ProductSubCategories psc ON psc.ProductId = p.Id
                LEFT JOIN SubCategories sc ON sc.Id = psc.SubCategoryId
                """;

            var products = await connection.QueryAsync<Product, ProductVariants, Category, SubCategory, Product>(
                sql,
                (product, variant, category, subCategory) =>
                {
                    product.Variants ??= new List<ProductVariants>();
                    product.Categories ??= new List<Category>();
                    product.SubCategories ??= new List<SubCategory>();


                    if (variant != null && variant.ProductVariantId != 0)
                            product.Variants.Add(variant);
                    if (category != null && category.CategoryId != 0)
                        product.Categories.Add(category);
                    if (subCategory != null && subCategory.SubCategoryId != 0)
                        product.SubCategories.Add(subCategory);

                    return product;
                },
                splitOn: "VariantId,CategoryId,SubCategoryId"
            );

            //var results = products.GroupBy(p => p.Id).Select(g =>
            //{
            //    var groupedProduct = g.First();
            //    groupedProduct.Variants = g.Select(p => p.Variants.Single()).ToList();
            //    groupedProduct.Categories = g.Select(p => p.Categories.Single()).ToList();
            //    groupedProduct.SubCategories = g.Select(p => p.SubCategories.Single()).ToList();
            //    return groupedProduct;
            //});

            var results = products
                .GroupBy(p => p.Id)
                .Select(g =>
                {
                    var groupedProduct = g.First();
                    groupedProduct.Variants = g.SelectMany(p => p.Variants)
                                                .GroupBy(v => v.ProductVariantId)
                                                .Select(vg => vg.First())
                                                .ToList();
                    groupedProduct.Categories = g.SelectMany(p => p.Categories)
                                                .GroupBy(c => c.CategoryId)
                                                .Select(cg => cg.First())
                                                .ToList();
                    groupedProduct.SubCategories = g.SelectMany(p => p.SubCategories)
                                                .GroupBy(sc => sc.SubCategoryId)
                                                .Select(scg => scg.First())
                                                .ToList();

                    return groupedProduct;
                })
                .ToList();

            return results;

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
