using FluentValidation;
using Retail.Contracts.Account;

namespace Retail.Application.Validators
{
    public class AccountRegistrationRequestValidator : AbstractValidator<AccountRegistrationRequest>
    {
        public AccountRegistrationRequestValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .WithMessage("Username is required.")
                .Matches(@"^[a-zA-Z0-9]+$")
                .WithMessage("Username must contain only letters and numbers.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MinimumLength(5)
                .WithMessage("Password must be at least 5 characters long.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .WithMessage("Confirm Password is required.")
                .Equal(x => x.Password)
                .WithMessage("Passwords do not match.");

            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("Invalid email address.")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?[1-9][0-9]{7,14}$")
                .WithMessage("Invalid phone number.")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        }
    }
}
