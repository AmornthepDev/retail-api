using Microsoft.AspNetCore.Mvc;
using Retail.Application.Services;
using Retail.Contracts.Login;

namespace Retail.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;

        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
        {
            var result = await _loginService.Login(request);
            return Ok(result);
        }
    }

}

