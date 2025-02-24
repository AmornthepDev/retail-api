namespace Retail.Application.Models
{
    public class ProductVariants
    {
        public required int ProductVariantId { get; set; }
        public required int ProductId { get; set; }
        public float Size { get; set; }
        public string Unit { get; set; } = default!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
