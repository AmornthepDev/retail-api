using Azure.Identity;
using Microsoft.AspNetCore.Identity;
using Retail.Application.Models;
using Retail.Contracts.Account;

namespace Retail.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public AccountService(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task AssignRoleAsync(string userName, string roleName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user is null)
                throw new Exception("User not found.");

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var result = await _roleManager.CreateAsync(new AppRole { Name = roleName, NormalizedName = roleName.ToUpper() });
                if (!result.Succeeded)
                    throw new Exception("Failed to create role.");
            }

            await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<AccountRegistrationResponse> RegisterAsync(AccountRegistrationRequest request)
        {
            var newUser = new AppUser
            {
                UserName = request.UserName,
                NormalizedUserName = request.UserName.ToUpper(),
                Email = request.Email,
                PhoneNumber = request.PhoneNumber
            };
            var result = await _userManager.CreateAsync(newUser, request.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return new AccountRegistrationResponse(false, errors);
            }

            return new AccountRegistrationResponse(true);
        }
    }
}
