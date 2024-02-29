using DemoApi.Extensions;
using DemoApi.Filters;
using DemoApi.Models;
using DemoApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPICoreDapper.ViewModels;

namespace DemoApi.Controllers
{
    [Route("api/{culture}/[controller]")]
    [ApiController]
    [MiddlewareFilter(typeof(LocalizationPipeline))]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        public AuthController(UserManager<AppUser> userManager, IConfiguration configuration, SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
        }
        //POST api/<ProductController> login
       [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        [ValidateModel]

        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, true);
                if (!result.Succeeded)
                    return BadRequest("Mật khẩu không đúng");
                var roles = await _userManager.GetRolesAsync(user);
                //var permissions = await _permissionService.GetPermissionStringByUserId(user.Id.ToString());
                var claims = new[]
                {
                    new Claim("Email", user.Email),
                    //new Claim(SystemConstants.UserClaim.Id, user.Id.ToString()),
                    //new Claim(ClaimTypes.NameIdentifier, user.UserName),
                    //new Claim(ClaimTypes.Name, user.UserName),
                    //new Claim(SystemConstants.UserClaim.FullName, user.FullName),
                    //new Claim(SystemConstants.UserClaim.Avatar, string.IsNullOrEmpty(user.Avatar) ? string.Empty : user.Avatar),
                    //new Claim(SystemConstants.UserClaim.Roles, string.Join(";", roles)),
                    //new Claim(SystemConstants.UserClaim.Permissions, JsonConvert.SerializeObject(permissions)),
                    //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(_configuration["Tokens:Issuer"],
                    _configuration["Tokens:Issuer"],
                    // claims,
                    expires: DateTime.Now.AddHours(2),
                    signingCredentials: creds);

                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            }
            return NotFound($"Không tìm thấy tài khoản {model.UserName}");
        }



        // POST api/<ProductController>
        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        [ValidateModel]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var user = new AppUser { FullName = model.FullName, UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                return Ok(model);
            }
            return BadRequest();
        }


    }
}
