using Dapper;
using DemoApi.Dtos;
using DemoApi.Extensions;
using DemoApi.Filters;
using DemoApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace DemoApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly SignInManager<AppUser> _signinManager;
        private readonly UserManager<AppUser> _userManager;
        public UserController(UserManager<AppUser>
            userManager, IConfiguration configuration, SignInManager<AppUser> signInManager)
        {
            _signinManager = signInManager;
            _configuration = configuration;
            _userManager = userManager;
            _connectionString = configuration.GetConnectionString("DevConnection");

        }
        [HttpGet, Authorize]
        [ClaimRequirementAttribuite(FunctionCode.SYSTEM_USER,ActionCode.VIEW)]
        public async Task<IActionResult> Get()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.OpenAsync();
                var parameters = new DynamicParameters();
                var result = await conn.QueryAsync<AppUser>("Get_User_All", parameters, null, null, System.Data.CommandType.StoredProcedure);
                return Ok(result);
            }
        }
        [HttpGet("{id}"), Authorize]
        public async Task<IActionResult> Get(string id)
        {
            return Ok(await _userManager.FindByIdAsync(id));
        }
        [HttpGet("paging")]
        public async Task<IActionResult> GetPaging(string keyword, int pageIndex, int pageSize)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("keyword", keyword);
                parameters.Add("pageIndex", pageIndex);
                parameters.Add("pageSize", pageSize);
                parameters.Add("totalRow", dbType: DbType.Int32, direction: ParameterDirection.Output);
                var result = await conn.QueryAsync<AppUser>(
                    "Get_User_AllPaging", parameters, null, null, System.Data.CommandType.StoredProcedure);
                int totalRow = parameters.Get<int>("totalRow");
                var pageResult = new PagedResult<AppUser>()
                {
                    Items = result.ToList(),
                    TotalRow = totalRow,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                };
                return Ok(pageResult);
            }
        }
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Post([FromBody] AppUser user)
        {
            var res = await _userManager.CreateAsync(user);
            if (res.Succeeded)
            {
                return Ok(res);
            }
            return BadRequest();
        }
        [HttpPut]
        [ValidateModel]
        public async Task<IActionResult> Put([Required] Guid id, [FromBody] AppUser user)
        {
            user.Id = id;
            var res = await _userManager.UpdateAsync(user);
            if (res.Succeeded)
            {
                return Ok(res);
            }
            return BadRequest();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var res = await _userManager.DeleteAsync(user);
            if (res.Succeeded)
            {
                return Ok();
            }
            return BadRequest();
        }
        [HttpPut("{id}/{roleName}/assign-to-roles")]
        public async Task<IActionResult> AssignToRoles([Required] Guid id, [Required] string roleName)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            using (var connection = new SqlConnection(_connectionString))
            {
                //if (connection.State == System.Data.ConnectionState.Closed)
                await connection.OpenAsync();
                var normalizedName = roleName.ToUpper();
                var roleId = await connection.ExecuteScalarAsync<Guid?>($"SELECT [Id] FROM [AspNetRoles] WHERE [NormalizedName] = @{nameof(normalizedName)}", new { normalizedName });
                if (!roleId.HasValue)
                {
                    roleId = Guid.NewGuid();
                    await connection.ExecuteAsync($"INSERT INTO [AspNetRoles]([Id],[Name], [NormalizedName]) VALUES(@{nameof(roleId)},@{nameof(roleName)}, @{nameof(normalizedName)})",
                       new { roleName, normalizedName });
                }


                await connection.ExecuteAsync($"IF NOT EXISTS(SELECT 1 FROM [AspNetUserRoles] WHERE [UserId] = @userId AND [RoleId] = @{nameof(roleId)}) " +
                    $"INSERT INTO [AspNetUserRoles]([UserId], [RoleId]) VALUES(@userId, @{nameof(roleId)})",
                    new { userId = user.Id, roleId });
                return Ok();
            }
        }
        [HttpGet("{id}/roles")]
        public async Task<IActionResult> GetUserRole(string id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            var res = await _userManager.GetRolesAsync(user);
            return Ok(res);
        }
        [HttpDelete("{id}/{roleName}/remove-roles")]
        [ValidateModel]
        public async Task<IActionResult> RemoveRoleToUser([Required] Guid id, [Required] string roleName)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            using (var connection = new SqlConnection(_connectionString))
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                    await connection.OpenAsync();
                var normalizedName = roleName.ToUpper();
                var roleId = await connection.ExecuteScalarAsync<Guid?>($"SELECT [Id] FROM [AspNetRole] WHERE [NormalizedName]=@{nameof(normalizedName)}", new { normalizedName });
                if (roleId.HasValue)

                    await connection.ExecuteAsync($"DELETE FROM [AspNetUserRoles] WHERE [UserId]=@userId AND [RoleId]=@{nameof(roleId)}", new { userId = user.Id, roleId });
                return Ok();
            }
        }
    }
}
