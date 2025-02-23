using Microsoft.Extensions.DependencyInjection;
using Retail.Application.Database;
using Retail.Application.Services;

namespace Retail.Application
{
    public static class ApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ILoginService, LoginService>();

            return services;
        }

        public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton<IDbConnectionFactory>(_ => new SqlServerConnectionFactory(connectionString));
            services.AddSingleton<DbInitializer>();

            return services;
        }
    }
}
