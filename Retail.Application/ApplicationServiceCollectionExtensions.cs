using Microsoft.Extensions.DependencyInjection;
using Retail.Application.Database;

namespace Retail.Application
{
    public static class ApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            throw new NotImplementedException();
        }

        public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton<IDbConnectionFactory>(_ => new SqlServerConnectionFactory(connectionString));
            services.AddSingleton<DbInitializer>();

            return services;
        }
    }
}
