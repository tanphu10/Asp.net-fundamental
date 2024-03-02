using Dapper;
using DemoApi.Models;
using DemoApi.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace DemoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LicenseController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly RoleManager<AppRole> _roleManager;

        public LicenseController(IConfiguration configuration, RoleManager<AppRole> roleManager)
        {
            _connectionString = configuration.GetConnectionString("DevConnection");
            _roleManager = roleManager;

        }
        [HttpGet("function-actions")]
        public async Task<IActionResult> GetAllWithPermission()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == ConnectionState.Closed)
                    await conn.OpenAsync();
                var result = await conn.QueryAsync<FunctionActionViewModel>("Get_Function_WithActions", null, null, null, System.Data.CommandType.StoredProcedure);
                return Ok(result);
            }
        }
        [HttpGet("{role}/role-permissions")]
        public async Task<IActionResult> GetAllRolePermissions(Guid? role)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var paramaters = new DynamicParameters();
                paramaters.Add("@roleId", role);

                var result = await conn.QueryAsync<PermissionViewModel>("Get_Permission_ByRoleId", paramaters, null, null, System.Data.CommandType.StoredProcedure);
                return Ok(result);
            }
        }
        [HttpPost("{role}/save-permissions")]
        public async Task<IActionResult> SavePermissions(Guid role, [FromBody] List<PermissionViewModel> permissions)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var dt = new DataTable();
                dt.Columns.Add("RoleId", typeof(Guid));
                dt.Columns.Add("FunctionId", typeof(string));
                dt.Columns.Add("ActionId", typeof(string));
                foreach (var item in permissions)
                {
                    dt.Rows.Add(role, item.FunctionId, item.ActionId);
                }
                var paramaters = new DynamicParameters();
                paramaters.Add("@roleId", role);
                paramaters.Add("@permissions", dt.AsTableValuedParameter("dbo.Permission"));
                await conn.ExecuteAsync("Create_Permission", paramaters, null, null, System.Data.CommandType.StoredProcedure);
                return Ok();

                //check Action role function có chưa nếu có thì chúng ta sẽ create còn không thì sẽ báo lại từng trường là không hợp lệ
                //var checkRole = await _roleManager.FindByIdAsync(role);
                ////checkfunction 
                //var parameters = new DynamicParameters();
                //parameters.Add("@id", permissions.FunctionId);
                //var checkFunc = await conn.QueryAsync<Function>("Get_Function_ById", parameters, null, null, System.Data.CommandType.StoredProcedure);
                ////checked action xem có tồn tại chưa nếu tồn tại rồi thì thêm vào oke
                //var checkAction = await conn.QueryAsync<Action>("Get_Action_ById",)
            }
        }

    }
}
