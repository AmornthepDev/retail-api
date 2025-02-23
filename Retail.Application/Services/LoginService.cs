using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Retail.Application.Models;
using Retail.Contracts.Login;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Retail.Application.Services
{
    public class LoginService : ILoginService
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _config;
        private readonly UserManager<AppUser> _userManager;
        private readonly string? _secret;
        private readonly string? _issuer;
        private readonly string? _audeince;
        private readonly string? _expirtyInDays;

        public LoginService(SignInManager<AppUser> signInManager, IConfiguration config, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _secret = config["JWT:Secret"];
            _issuer = config["JWT:Issuer"];
            _audeince = config["JWT:Audience"];
            _expirtyInDays = config["JWT:ExpiryInDays"];
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName.ToUpper());
            if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return new LoginResponse(false, "Invalid credentials");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, request.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.Now.AddDays(Convert.ToInt32(_expirtyInDays));

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audeince,
                claims: claims,
                expires: expiry,
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginResponse(true, Token: jwt);
        }
    }
}
