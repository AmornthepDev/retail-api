namespace Retail.Application.Models
{
    public class Product
    {
        public required int Id { get; set; }
        public required string Sku { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }

        #region Navigation Property
        public required List<ProductVariants> Variants { get; set; }
        public required List<Category> Categories { get; set; } = new();
        public required List<SubCategory> subCategories { get; set; } = new();
        #endregion

    }
}
