using System.ComponentModel.DataAnnotations;

namespace Retail.Contracts.Account
{
    public class AccountRegistrationRequest
    {
        [Required]
        public required string UserName { get; set; } = string.Empty;
        [Required, MinLength(6)]
        public required string Password { get; set; } = string.Empty;
        [Required, Compare("Password", ErrorMessage = "The password do not match.")]
        public required string ConfirmPassword { get; set; } = string.Empty;
        [EmailAddress]
        public string? Email { get; set; }
        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
