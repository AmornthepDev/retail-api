using Microsoft.AspNetCore.Identity;
using Retail.Contracts.Account;

namespace Retail.Application.Services
{
    public interface IAccountService
    {
        Task<AccountRegistrationResponse> RegisterAsync(AccountRegistrationRequest request);
        Task AssignRoleAsync(string userName, string roleName);
    }
}
