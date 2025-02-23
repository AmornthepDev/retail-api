using Dapper;
using Microsoft.AspNetCore.Identity;
using Retail.Application.Database;
using Retail.Application.Models;

namespace Retail.Application.Identity
{
    public class UserStore : 
        IUserStore<AppUser>, 
        IUserPasswordStore<AppUser>,
        IUserRoleStore<AppUser>
    {
        private readonly IDbConnectionFactory _dbconnectionFactory;

        public UserStore(IDbConnectionFactory dbconnectionFactory)
        {
            _dbconnectionFactory = dbconnectionFactory;
        }

        public async Task AddToRoleAsync(AppUser user, string roleName, CancellationToken token)
        {
            using var connection = await _dbconnectionFactory.CreateConnectionAsync(token);

            var role = await connection.QuerySingleOrDefaultAsync<AppRole>(new CommandDefinition("""
                SELECT * FROM AspNetRoles WHERE NormalizedName = @NormalizedName
                """, new { NormalizedName = roleName.ToUpper() }, cancellationToken: token));
            if (role is null)
            {
                throw new ArgumentException("Role not found.");
            }

            var userRoleExists = await connection.QuerySingleOrDefaultAsync<int?>(new CommandDefinition(
            @"SELECT 1 FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @RoleId",
            new { UserId = user.Id, RoleId = role.Id },
            cancellationToken: token));

            if (userRoleExists.HasValue)
                return;

            await connection.ExecuteAsync(new CommandDefinition("""
                INSERT INTO AspNetUserRoles (UserId, RoleId)
                VALUES (@UserId, @RoleId);
                """, new { UserId = user.Id, RoleId = role.Id }, cancellationToken: token));
        }

        public async Task<IdentityResult> CreateAsync(AppUser user, CancellationToken token)
        {
            using var connection = await _dbconnectionFactory.CreateConnectionAsync(token);
            var result = await connection.ExecuteAsync(new CommandDefinition("""
                INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, PasswordHash, Email, PhoneNumber, FirstName, LastName)
                VALUES (@Id, @UserName, @NormalizedUserName, @PasswordHash, @Email, @PhoneNumber, @FirstName, @LastName)
                """, user, cancellationToken: token));

            return result > 0 ?
                IdentityResult.Success :
                IdentityResult.Failed(new IdentityError { Description = "User creation failed." });
        }

        public async Task<IdentityResult> DeleteAsync(AppUser user, CancellationToken token)
        {
            using var connection = await _dbconnectionFactory.CreateConnectionAsync(token);
            var result = await connection.ExecuteAsync(new CommandDefinition("""
                DELETE FROM AspNetUsers WHERE Id = @Id;
                """, user.Id, cancellationToken: token));

            return result > 0 ?
                IdentityResult.Success :
                IdentityResult.Failed(new IdentityError { Description = "User deletion failed" });
        }

        public void Dispose() { }

        public async Task<AppUser?> FindByIdAsync(string userId, CancellationToken token)
        {
            using var connection = await _dbconnectionFactory.CreateConnectionAsync(token);
            var result = await connection.QuerySingleOrDefaultAsync<AppUser>(new CommandDefinition("""
                SELECT * FROM AspNetUsers WHERE Id = @Id
                """, new { Id = userId }, cancellationToken: token));

            return result;
        }

        public async Task<AppUser?> FindByNameAsync(string normalizedUserName, CancellationToken token)
        {
            using var connection = await _dbconnectionFactory.CreateConnectionAsync(token);
            var result = await connection.QuerySingleOrDefaultAsync<AppUser>(new CommandDefinition("""
                SELECT * FROM AspNetUsers WHERE NormalizedUserName = @NormalizedUserName;
                """, new { NormalizedUserName = normalizedUserName }, cancellationToken: token));

            return result;
        }

        public Task<string?> GetNormalizedUserNameAsync(AppUser user, CancellationToken token)
            => Task.FromResult(user.NormalizedUserName);

        public Task<string?> GetPasswordHashAsync(AppUser user, CancellationToken token)
            => Task.FromResult(user.PasswordHash);

        public async Task<IList<string>> GetRolesAsync(AppUser user, CancellationToken token)
        {
            using var connection = await _dbconnectionFactory.CreateConnectionAsync(token);
            var roles = await connection.QueryAsync<string>(new CommandDefinition("""
                SELECT r.Name
                FROM AspNetRoles r
                INNER JOIN AspNetUserRoles ur 
                    ON r.Id = ur.RoleId
                WHERE ur.UserId = @UserId
                """, new { UserId = user.Id }, cancellationToken: token));
            return roles.ToList();
        }

        public Task<string> GetUserIdAsync(AppUser user, CancellationToken token)
            => Task.FromResult(user.Id);

        public Task<string?> GetUserNameAsync(AppUser user, CancellationToken token)
            => Task.FromResult(user.UserName);

        public Task<IList<AppUser>> GetUsersInRoleAsync(string roleName, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasPasswordAsync(AppUser user, CancellationToken token)
            => Task.FromResult(user.PasswordHash != null);

        public async Task<bool> IsInRoleAsync(AppUser user, string roleName, CancellationToken token)
        {
            var roles = await GetRolesAsync(user, token);
            return roles.Contains(roleName);
        }

        public async Task RemoveFromRoleAsync(AppUser user, string roleName, CancellationToken token)
        {
            using var connection = await _dbconnectionFactory.CreateConnectionAsync();
            var role = await connection.QuerySingleOrDefaultAsync(new CommandDefinition("""
                SELECT * FROM AspNetRoles WHERE NormalizedName = @NormalizedName
                """, new { NormalizedName = roleName.ToUpper() }, cancellationToken: token));
            if (role is not null)
            {
                await connection.ExecuteAsync(new CommandDefinition("""
                    DELETE FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @RoleId
                    """, new { UserId = user.Id, RoleId = role.Id }, cancellationToken: token));
            }
        }

        public Task SetNormalizedUserNameAsync(AppUser user, string? normalizedName, CancellationToken token)
        {
            if (normalizedName is null) throw new ArgumentNullException(nameof(normalizedName));

            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(AppUser user, string? passwordHash, CancellationToken token)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(AppUser user, string? userName, CancellationToken token)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(AppUser user, CancellationToken token)
        {
            using var connection = await _dbconnectionFactory.CreateConnectionAsync(token);
            var result = await connection.ExecuteAsync(new CommandDefinition("""
                UPDATE AspNetUsers
                SET UserName = @UserName,
                    NormalizedUserName = @NormalizedUserName,
                    PasswordHash = @PasswordHash,
                    Email = @Email,
                    PhoneNumber = @PhoneNumber
                WHERE Id = @Id
                """, user, cancellationToken: token));

            return result > 0 ?
                IdentityResult.Success :
                IdentityResult.Failed(new IdentityError { Description = "User updated failed" });
        }
    }
}
