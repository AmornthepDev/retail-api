namespace Retail.Application.Models
{
    public class AppUser
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public required string UserName { get; set; }
        public required string NormalizedUserName { get; set; }
        public required string PasswordHash { get; set; }
        public string? Email { get; set; }
        public bool EmailConfirmed { get; set; } = false;
        public string? PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; } = false;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
