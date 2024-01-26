using DemoApi.Models;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Reflection;
using DemoApi.Resources;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var supportedCultures = new[]
              {
                    new CultureInfo("en-US"),
                    new CultureInfo("vi-VN"),
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
    .AddDataAnnotationsLocalization(options => {
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

app.Run();
