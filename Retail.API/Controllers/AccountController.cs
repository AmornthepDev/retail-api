using Microsoft.AspNetCore.Mvc;
using Retail.Application.Services;
using Retail.Contracts.Account;

namespace Retail.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost]
        public async Task<ActionResult<AccountRegistrationResponse>> Register(AccountRegistrationRequest request)
        {
            var result = await _accountService.RegisterAsync(request);
            return Ok(result);
        }

        [HttpPost("role")]
        public async Task<IActionResult> AssignRole(string userName, string roleName)
        {
            await _accountService.AssignRoleAsync(userName, roleName);
            return Ok();
        }
    }

}

