using Retail.Contracts.Login;

namespace Retail.Application.Services
{
    public interface ILoginService
    {
        Task<LoginResponse> Login(LoginRequest request);
    }
}
