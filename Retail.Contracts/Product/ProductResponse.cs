namespace Retail.Contracts.Product
{
    public class ProductResponse
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public float? Size { get; set; }
        public string? Unit { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public required IEnumerable<string> Categories { get; set; } = Enumerable.Empty<string>();
        public required IEnumerable<string> SubCategories { get; set; } = Enumerable.Empty<string>();
    }
}
