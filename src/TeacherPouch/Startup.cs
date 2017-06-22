using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TeacherPouch.Data;
using TeacherPouch.Options;
using TeacherPouch.Services;

namespace TeacherPouch
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            builder.AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<PhotoPaths>(Configuration.GetSection("PhotoPaths"));

            services.AddDbContext<TeacherPouchDbContext>(o =>
            {
                o.EnableSensitiveDataLogging();
                o.UseSqlite(Configuration.GetConnectionString("TeacherPouch"));
            });
            services.AddDbContext<Data.IdentityDbContext>(o =>
            {
                o.UseSqlite(Configuration.GetConnectionString("Identity"));
            });

            services
                .AddIdentity<IdentityUser, IdentityRole>(opts =>
                {
                    opts.Password.RequireNonAlphanumeric = false;
                })
                .AddEntityFrameworkStores<Data.IdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddRouting(opts => opts.LowercaseUrls = true);

            services.AddMvc();

            services.AddMemoryCache();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddScoped<PhotoService>();
            services.AddScoped<TagService>();
            services.AddScoped<SearchService>();

            // Add application services.
            // TODO: implement me
            //services.AddTransient<IEmailSender, AuthMessageSender>();
            //services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseIdentity();

            app.UseMvc();
        }
    }
}
