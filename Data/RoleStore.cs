using Dapper;
using DemoApi.Data.Models;
using DemoApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;

namespace DemoApi.Data
{
    public class RoleStore : IRoleStore<AppRole>
    {
        private readonly string _connectionString;
        public RoleStore(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DevConnection");
        }
        public async Task<IdentityResult> CreateAsync(AppRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                role.Id = Guid.NewGuid();
                await connection.ExecuteAsync($@"INSERT INTO [AspNetRoles] ([Id], [Name], [NormalizedName])
                    VALUES (@{nameof(AppRole.Id)},@{nameof(AppRole.Name)}, @{nameof(AppRole.NormalizedName)});", role);
            }

            //using (var connection = new SqlConnection(_connectionString))
            //{
            //    await connection.OpenAsync(cancellationToken);
            //    role.Id = Guid.NewGuid();
            //    await connection.ExecuteAsync($@"INSERT INTO [AspNetRoles] ([Id],[Name],[NormalizedName])
            //    VALUES(@{nameof(AppRole.Id)},@{nameof(AppRole.Name)},@{nameof(AppRole.NormalizedName)});", role);
            //}
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(AppRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                await connection.ExecuteAsync($"DELETE FROM [AspNetRoles] WHERE [Id]=@{nameof(AppRole.Id)}", role);
            }
            return IdentityResult.Success;
        }



        public async Task<AppRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                return await connection.QuerySingleOrDefaultAsync<AppRole>($@"SELECT * FROM [AspNetRoles]
                    WHERE [Id] =@{nameof(roleId)}", new { roleId });
            }
        }

        public async Task<AppRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                return await connection.QuerySingleOrDefaultAsync<AppRole>(
                    $@"SELECT * FROM [AspNetRoles] WHERE [NormalizedName] = @{nameof(normalizedRoleName)}", new { normalizedRoleName });
            }
        }

        public Task<string> GetNormalizedRoleNameAsync(AppRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.NormalizedName);
        }

        public Task<string> GetRoleIdAsync(AppRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Id.ToString());
        }

        public Task<string> GetRoleNameAsync(AppRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }


        public Task SetNormalizedRoleNameAsync(AppRole role, string normalizedName, CancellationToken cancellationToken)
        {
            role.NormalizedName = normalizedName;
            return Task.FromResult(0);
        }

        public Task SetRoleNameAsync(AppRole role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return Task.FromResult(0);
        }

        public async Task<IdentityResult> UpdateAsync(AppRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                await connection.ExecuteAsync($@"UPDATE [AspNetRoles] SET
                    [Name] = @{nameof(AppRole.Name)},
                    [NormalizedName] = @{nameof(AppRole.NormalizedName)}
                    WHERE [Id] = @{nameof(AppRole.Id)}", role);

            }
            return IdentityResult.Success;
        }
        public void Dispose()
        {
            // Nothing to dispose.
        }
    }
}
