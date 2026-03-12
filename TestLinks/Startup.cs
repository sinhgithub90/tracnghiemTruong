using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using SpaWebApp.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TestLinks.Data;
using TestLinks.Models;
using TestLinks.Services;

namespace TestLinks
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();
            
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
            })
                .AddErrorDescriber<CustomIdentityErrorDescriber>() // Add this line
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>(TokenOptions.DefaultProvider);
            ;

            services.AddControllersWithViews();

            IFileProvider physicalProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
            services.AddSingleton<IFileProvider>(physicalProvider);

            services.AddSingleton<FileUploadService>();
            services.AddScoped<UploadLinkService>();

            services.AddHttpContextAccessor();

            services.AddTransient<IEmailSender, MailjetService>();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                //other properties
            });
            //services.AddScoped<RoleManager<IdentityRole>>();
            services.AddRazorPages();

            services.AddHttpClient<ReCaptcha>(x =>
            {
                x.BaseAddress = new Uri("https://www.google.com/recaptcha/api/siteverify");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseDeveloperExceptionPage();
                //app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }

            //app.UseSerilogRequestLogging(); // <-- Add this line
            app.UseSerilogRequestLogging(options =>
            {
                // Customize the message template
                //options.MessageTemplate = "Handled {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed: 0.0000}";
                options.MessageTemplate = "[{ClientIp}] {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed: 0.0000}";

                // Emit debug-level events instead of the defaults
                //options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Debug;

                // Attach additional properties to the request completion event
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                    diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                };
            });

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
