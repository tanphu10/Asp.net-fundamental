using Dapper;
using DemoApi.Data.Models;
using DemoApi.Dtos;
using DemoApi.Filters;
using Microsoft.AspNetCore.Authorization;
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
    public class RoleController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly RoleManager<AppRole> _roleManager;
        public RoleController(RoleManager<AppRole> roleManager, IConfiguration configuration)
        {
            _roleManager = roleManager;
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DevConnection");
        }
        [HttpGet, Authorize]
        public async Task<IActionResult> Get()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.OpenAsync();
                var parameters = new DynamicParameters();
                var result = await conn.QueryAsync<AppRole>("Get_Role_All", parameters, null, null, System.Data.CommandType.StoredProcedure);
                return Ok(result);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            return Ok(await _roleManager.FindByIdAsync(id));

        }
        [HttpGet("paging")]
        public async Task<IActionResult> GetPaging(string keyword, int pageIndex, int pageSize)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@keyword", keyword);
                parameters.Add("@pageIndex", pageIndex);
                parameters.Add("@pageSize", pageSize);
                parameters.Add("@totalRow", dbType: DbType.Int32, direction: ParameterDirection.Output);
                var result = await conn.QueryAsync<AppRole>("Get_Role_AllPaging", parameters, null, null, System.Data.CommandType.StoredProcedure);
                int totalRow = parameters.Get<int>("totalRow");
                var pageResult = new PagedResult<AppRole>()
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
        public async Task<IActionResult> Post([FromBody] AppRole role)
        {
            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                return Ok(result);
            }
            return BadRequest();

        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([Required] Guid id, [FromBody] AppRole role)
        {
            role.Id = id;
            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                return Ok(result);
            }
            return BadRequest();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
                return Ok();
            return BadRequest();

        }

    }
}
