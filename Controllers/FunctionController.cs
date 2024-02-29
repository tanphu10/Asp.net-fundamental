using Dapper;
using DemoApi.Dtos;
using DemoApi.Filters;
using DemoApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DemoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FunctionController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public FunctionController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DevConnection");
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (
                var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == System.Data.ConnectionState.Open)
                    conn.OpenAsync();
                var parameters = new DynamicParameters();
                var res = await conn.QueryAsync<Function>("Get_Function_All", parameters, null
                , null, System.Data.CommandType.StoredProcedure);
                return Ok(res);
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@id", id);
                var result = await conn.QueryAsync<Function>("Get_Function_ById", parameters, null, null, System.Data.CommandType.StoredProcedure);
                return Ok(result.Single());
            }
        }
        //get Paginate
        [HttpGet("Paging")]
        public async Task<PagedResult<Function>> GetPaging(string keyword, int pageIndex, int pageSize)
        {

            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@keyword", keyword);
                parameters.Add("@pageIndex", pageIndex);
                parameters.Add("@pageSize", pageSize);
                parameters.Add("@totalRow", dbType: DbType.Int32, direction: ParameterDirection.Output);
                var result = await conn.QueryAsync<Function>("Get_Function_AllPaging", parameters, null, null, System.Data.CommandType.StoredProcedure);
                int totalRow = parameters.Get<int>("totalRow");
                var pageResult = new PagedResult<Function>()
                {
                    Items = result.ToList(),
                    TotalRow = totalRow,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                };
                return pageResult;
            }
        }

        // POST api/<>
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Post([FromBody] Function function)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@id", function.Id);
                parameters.Add("@name", function.Name);
                parameters.Add("@url", function.Url);
                parameters.Add("@parentId", function.ParentId);
                parameters.Add("@cssClass", function.CssClass);
                parameters.Add("@sortOrder", function.SortOrder);
                parameters.Add("@isActive", function.IsActive);
                //parameters.Add("@id", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);
                var result = await conn.ExecuteAsync("Create_Function", parameters, null, null, System.Data.CommandType.StoredProcedure);
                return Ok();
            }
        }
        //// PUT: api/Role/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> Put([Required] Guid id,[FromBody] Function function)
        //{
        //    using (var conn = new SqlConnection(_connectionString))
        //    {
        //        if (conn.State == System.Data.ConnectionState.Closed)
        //            conn.Open();
        //        var paramaters = new DynamicParameters();
        //        paramaters.Add("@id", function.Id);
        //        paramaters.Add("@name", function.Name);
        //        paramaters.Add("@url", function.Url);
        //        paramaters.Add("@parentId", function.ParentId);
        //        paramaters.Add("@sortOrder", function.SortOrder);
        //        paramaters.Add("@cssClass", function.CssClass);
        //        paramaters.Add("@isActive", function.IsActive);

        //        await conn.ExecuteAsync("Update_Function", paramaters, null, null, System.Data.CommandType.StoredProcedure);
        //        return Ok();
        //    }
        //}


        [HttpPut("{id}")]
        public async Task<IActionResult> Put([Required] Guid id, [FromBody] Function function)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();
                var paramaters = new DynamicParameters();
                paramaters.Add("@id", function.Id);
                paramaters.Add("@name", function.Name);
                paramaters.Add("@url", function.Url);
                paramaters.Add("@parentId", function.ParentId);
                paramaters.Add("@sortOrder", function.SortOrder);
                paramaters.Add("@cssClass", function.CssClass);
                paramaters.Add("@isActive", function.IsActive);

                await conn.ExecuteAsync("Update_Function", paramaters, null, null, System.Data.CommandType.StoredProcedure);
                return Ok();
            }
        }


        // DELETE api/<functioncontrollers>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@id", id);
                await conn.ExecuteAsync("Delete_Function_ById", parameters, null, null, System.Data.CommandType.StoredProcedure);
                return Ok();

            }
        }
    }
}
