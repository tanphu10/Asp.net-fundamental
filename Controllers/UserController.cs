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
            _userManager = userManager;
            _configuration = configuration;
            _signinManager = signInManager;
            _connectionString = configuration.GetConnectionString("DevConnection");

        }
        [HttpGet]
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
        [HttpGet("{id}")]
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
    }
}
