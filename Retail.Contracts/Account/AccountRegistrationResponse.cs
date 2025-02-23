namespace Retail.Contracts.Account
{
    public record struct AccountRegistrationResponse(bool IsSuccessful, IEnumerable<string>? Errors = null);
}
