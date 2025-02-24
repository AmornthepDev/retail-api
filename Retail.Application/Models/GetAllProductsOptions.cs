using Retail.Application.Enums;

namespace Retail.Application.Models
{
    public class GetAllProductsOptions
    {
        public string? Name { get; set; }
        public string? Sku { get; set; }
        public decimal? MinimumPrice { get; set; }
        public string? SortColumn { get; set; }
        public SortOrder? SortOrder { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
}
