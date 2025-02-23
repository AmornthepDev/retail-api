namespace Retail.Application.Models
{
    public class GetAllProductsOptions
    {
        public string? Name { get; set; }
        public string? Sku { get; set; }
        public int? UserId { get; set; }
        public string? SortColumn { get; set; }
    }
}
