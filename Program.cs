using DemoApi.Models;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Reflection;
using DemoApi.Resources;
using System.Text.Json.Serialization;
using DemoApi.Data;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IUserStore<AppUser>, UserStore>();
builder.Services.AddTransient<IRoleStore<AppRole>, RoleStore>();

builder.Services.AddIdentity<AppUser, AppRole>()
  .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(opt =>
{
    // Default Password settings.
    opt.Password.RequireDigit = true;
    opt.Password.RequireLowercase = false;
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequireUppercase = false;
    opt.Password.RequiredLength = 6;
    opt.Password.RequiredUniqueChars = 1;
});
var supportedCultures = new[]
              {
                    new CultureInfo("vi-VN"),
                    new CultureInfo("en-US"),
                };

var options = new RequestLocalizationOptions()
{
    DefaultRequestCulture = new RequestCulture(culture: "vi-VN", uiCulture: "vi-VN"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};
options.RequestCultureProviders = new[]
{
    new RouteDataRequestCultureProvider() { Options = options }
};
builder.Services.AddSingleton(options);
//cấu hình lại singleton khi multiple file
builder.Services.AddSingleton<LocService>();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddMvc()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
            factory.Create(typeof(SharedResource));
    });
//---------------

builder.Services.AddDbContext<APIDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DevConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseAuthentication();
app.Run();
