namespace Retail.Application.Models
{
    public class Product
    {
        public required int Id { get; set; }
        public required string Sku { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }

        #region Navigation Property
        public List<ProductVariants> Variants { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public List<SubCategory> SubCategories { get; set; } = new();
        #endregion

    }
}
