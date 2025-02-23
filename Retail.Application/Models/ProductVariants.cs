namespace Retail.Application.Models
{
    public class ProductVariants
    {
        public required int Id { get; set; }
        public decimal Size { get; set; }
        public string Unit { get; set; } = default!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
