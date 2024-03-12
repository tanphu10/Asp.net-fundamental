using Dapper;
using DemoApi.Dtos;
using DemoApi.Extensions;
using DemoApi.Filters;
using DemoApi.Models;
using DemoApi.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Localization;
using System;
using System.Data;
using System.Globalization;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DemoApi.Controllers
{
    [Route("api/{culture}/[controller]")]
    [Authorize]
    [ApiController]
    [MiddlewareFilter(typeof(LocalizationPipeline))]
    public class ProductController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly IStringLocalizer<ProductController> _localizer;
        private readonly LocService _locService;

        public ProductController(IConfiguration configuration, LocService locService, IStringLocalizer<ProductController> localizer)
        {
            _connectionString = configuration.GetConnectionString("DevConnection");
            _localizer = localizer;
            _locService = locService;
        }
        // GET: api/<ProductController>
        [HttpGet]
        public async Task<IEnumerable<Product>> Get()
        {
            var culture = CultureInfo.CurrentCulture.Name;
            //lấy tên theo đặt bên resource
            //string text = _localizer["Test"];
            //string text1= _locService.GetLocalizedHtmlString("ForgotPassword");
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@language", culture);
                var result = await conn.QueryAsync<Product>("Get_Product_All", parameters, null, null, System.Data.CommandType.StoredProcedure);
                return result;
            }
        }
        // GET api/<ProductController>/5
        [HttpGet("{id}")]
        public async Task<Product> Get(int id)
        {
            var culture = CultureInfo.CurrentCulture.Name;

            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@id", id);
                parameters.Add("@language", culture);
                var result = await conn.QueryAsync<Product>("Get_Product_By_Id", parameters, null, null, System.Data.CommandType.StoredProcedure);
                return result.Single();
            }
        }

        //get Paginate
        [HttpGet("Paging", Name = "GetPaging")]
        public async Task<PagedResult<Product>> GetPaging(string keyword, int categoryId, int pageIndex, int pageSize)
        {
            var culture = CultureInfo.CurrentCulture.Name;

            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@keyword", keyword);
                parameters.Add("@language", culture);
                parameters.Add("@categoryId", categoryId);
                parameters.Add("@pageIndex", pageIndex);
                parameters.Add("@pageSize", pageSize);
                parameters.Add("@totalRow", dbType: DbType.Int32, direction: ParameterDirection.Output);
                var result = await conn.QueryAsync<Product>("Get_Product_AllPaging", parameters, null, null, System.Data.CommandType.StoredProcedure);
                int totalRow = parameters.Get<int>("totalRow");
                var pageResult = new PagedResult<Product>()
                {
                    Items = result.ToList(),
                    TotalRow = totalRow,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                };
                return pageResult;
            }
        }

        // POST api/<ProductController>
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Post([FromBody] Product product)
        {
            var culture = CultureInfo.CurrentCulture.Name;
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@language", culture);
                parameters.Add("@sku", product.Sku);
                parameters.Add("@price", product.Price);
                parameters.Add("@isActive", product.IsActive);
                parameters.Add("@imageUrl", product.ImageUrl);
                parameters.Add("@name", product.Name);
                parameters.Add("@description", product.Description);
                parameters.Add("@content", product.Content);
                parameters.Add("@seoDescription", product.SeoDescription);
                parameters.Add("@seoAlias", product.SeoAlias);
                parameters.Add("@seoTitle", product.SeoKeyword);
                parameters.Add("@seoKeyword", product.SeoKeyword);
                parameters.Add("@categoryIds", product.CategoryIds);
                parameters.Add("@id", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);
                var result = await conn.ExecuteAsync("Create_Product", parameters, null, null, System.Data.CommandType.StoredProcedure);
                int newId = parameters.Get<int>("@id");
                return Ok(newId);
            }
        }

        // PUT api/<ProductController>/5
        [HttpPut("{id}")]
        [ValidateModel]
        public async Task<IActionResult> Put(int id, [FromBody] Product product)
        {
            var culture = CultureInfo.CurrentCulture.Name;
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@id", id);
                parameters.Add("@language", culture);
                parameters.Add("@sku", product.Sku);
                parameters.Add("@price", product.Price);
                parameters.Add("@isActive", product.IsActive);
                parameters.Add("@imageUrl", product.ImageUrl);
                parameters.Add("@name", product.Name);
                parameters.Add("@description", product.Description);
                parameters.Add("@content", product.Content);
                parameters.Add("@seoDescription", product.SeoDescription);
                parameters.Add("@seoAlias", product.SeoAlias);
                parameters.Add("@seoTitle", product.SeoKeyword);
                parameters.Add("@seoKeyword", product.SeoKeyword);
                parameters.Add("@categoryIds", product.CategoryIds);
                await conn.ExecuteAsync("Update_Product", parameters, null, null, System.Data.CommandType.StoredProcedure);
                return Ok();
            }
        }

        // DELETE api/<ProductController>/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            var culture = CultureInfo.CurrentCulture.Name;
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@id", id);
                await conn.ExecuteAsync("Delete_Product_ById", parameters, null, null, System.Data.CommandType.StoredProcedure);
            }
        }
    }
}
