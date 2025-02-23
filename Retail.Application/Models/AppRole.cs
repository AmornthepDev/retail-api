namespace Retail.Application.Models
{
    public class AppRole
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string NormalizedName { get; set; } = string.Empty;

        #region Navigation Property
        public List<AppUser> Users { get; set; } = new();
        #endregion
    }
}
