namespace Retail.Application.Models
{
    public class AppRole
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public required string Name { get; set; }
        public required string NormalizedName { get; set; }
    }
}
