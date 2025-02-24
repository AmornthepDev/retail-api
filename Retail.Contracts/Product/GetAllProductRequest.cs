using Retail.Contracts.GetAllBase;

namespace Retail.Contracts.Product
{
    public class GetAllProductsRequest : PagedRequest
    {
        public string? Name { get; set; }
        public string? Sku { get; set; }
        public decimal? MinimumPrice { get; set; }
        public int? CategoryName { get; set; }
        public int? SubCategoryName { get; set; }
    }
}
