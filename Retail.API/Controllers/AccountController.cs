using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Retail.Application.Models;
using Retail.Application.Services;
using Retail.Contracts.Account;

namespace Retail.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IValidator<AccountRegistrationRequest> _accountValidator;

        public AccountController(IAccountService accountService, IValidator<AccountRegistrationRequest> accountValidator)
        {
            _accountService = accountService;
            _accountValidator = accountValidator;
        }

        [HttpPost]
        public async Task<ActionResult<AccountRegistrationResponse>> Register(AccountRegistrationRequest request, CancellationToken token)
        {
            await _accountValidator.ValidateAndThrowAsync(request, cancellationToken: token);
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

