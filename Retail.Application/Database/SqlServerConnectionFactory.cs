using Microsoft.Data.SqlClient;
using System.Data;

namespace Retail.Application.Database
{
    public class SqlServerConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public SqlServerConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default)
        {
            var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(token);

            return connection;
        }
    }
}
