using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using TeacherPouch.Data;
using TeacherPouch.Options;
using TeacherPouch.Services;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
services.AddOptions();
services.Configure<PhotoPaths>(builder.Configuration.GetSection("PhotoPaths"));

services.AddDbContext<TeacherPouchDbContext>(o =>
{
    o.EnableSensitiveDataLogging();
    o.UseSqlite(builder.Configuration.GetConnectionString("TeacherPouch"));
});
services.AddDbContext<IdentityDbContext>(o =>
{
    o.UseSqlite(builder.Configuration.GetConnectionString("Identity"));
});

services
    .AddIdentity<IdentityUser, IdentityRole>(opts =>
    {
        opts.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<IdentityDbContext>()
    .AddDefaultTokenProviders();

services.AddRouting(opts => opts.LowercaseUrls = true);

services.AddControllersWithViews();

services.AddMemoryCache();

services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

services.AddScoped<PhotoService>();
services.AddScoped<TagService>();
services.AddScoped<SearchService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
    app.UseExceptionHandler("/Error");

app.UseStaticFiles();
app.UseRouting();

app.UseStatusCodePages();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
