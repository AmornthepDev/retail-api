using Retail.Application.Enums;
using Retail.Application.Models;
using Retail.Contracts.Product;

namespace Retail.Application.Mapping
{
    public static class ContractMapping
    {
        public static GetAllProductsOptions MapToOptions(this GetAllProductsRequest request)
        {
            return new GetAllProductsOptions
            {
                Name = request.Name,
                Sku = request.Sku,
                MinimumPrice = request.MinimumPrice,
                SortColumn = request.SortColumn,
                SortOrder = request.SortOrder?.ToUpper() == "ASC" ? SortOrder.Ascending : SortOrder.Descending,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

    }
}
