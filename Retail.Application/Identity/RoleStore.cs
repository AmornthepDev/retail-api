using Dapper;
using Microsoft.AspNetCore.Identity;
using Retail.Application.Database;
using Retail.Application.Models;

namespace Retail.Application.Identity
{
    public class RoleStore : IRoleStore<AppRole>
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public RoleStore(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<IdentityResult> CreateAsync(AppRole role, CancellationToken token)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
            var result = await connection.ExecuteAsync(new CommandDefinition("""
                INSERT INTO AspNetUsers (Id, Name, NormalizedName) VALUES (@Id, @Name, @NormalizedName)
                """, role, cancellationToken: token));

            return result > 0 ?
                IdentityResult.Success :
                IdentityResult.Failed(new IdentityError { Description = "Role creation failed." });
        }

        public async Task<IdentityResult> DeleteAsync(AppRole role, CancellationToken token)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
            var result = await connection.ExecuteAsync(new CommandDefinition("""
                DELETE FROM AspNetUsers WHERE Id = @Id
                """, new { role.Id }, cancellationToken: token));

            return result > 0 ?
                IdentityResult.Success :
                IdentityResult.Failed(new IdentityError { Description = "Role deletion failed." });
        }

        public void Dispose() { }

        public async Task<AppRole?> FindByIdAsync(string roleId, CancellationToken token)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
            var result = await connection.QuerySingleOrDefaultAsync<AppRole>(new CommandDefinition("""
                SELECT * FROM AspNetRoles WHERE Id = @Id
                """, new { Id = roleId }, cancellationToken: token));

            return result;
        }

        public async Task<AppRole?> FindByNameAsync(string normalizedRoleName, CancellationToken token)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
            var result = await connection.QuerySingleOrDefaultAsync<AppRole>(new CommandDefinition("""
                SELECT * FROM AspNetRoles WHERE NormalizedName = @NormalizedName
                """, new { NormalizedName = normalizedRoleName }, cancellationToken: token));

            return result;
        }

        public Task<string?> GetNormalizedRoleNameAsync(AppRole role, CancellationToken token)
            => Task.FromResult(role.NormalizedName);

        public Task<string> GetRoleIdAsync(AppRole role, CancellationToken token)
            => Task.FromResult(role.Id);

        public Task<string?> GetRoleNameAsync(AppRole role, CancellationToken token)
            => Task.FromResult(role.Name);

        public Task SetNormalizedRoleNameAsync(AppRole role, string? normalizedName, CancellationToken token)
        {
            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(AppRole role, string? roleName, CancellationToken token)
        {
            role.Name = roleName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(AppRole role, CancellationToken token)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
            var result = await connection.ExecuteAsync(new CommandDefinition("""
                UPDATE AspNetRoles
                SET Name = @Name,
                    NormalizedName = @NormalizedName,
                WHERE Id = @Id
                """, role, cancellationToken: token));

            return result > 0 ?
                IdentityResult.Success :
                IdentityResult.Failed(new IdentityError { Description = "Role updated failed" });
        }
    }
}
