using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestLinks.Extensions;
using TestLinks.Data;
using TestLinks.Models;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Collections.Generic;

namespace TestLinks
{
    public class Program
    {
        public static int Main(string[] args)
        {
            IConfigurationRoot _config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();


            Log.Logger = new LoggerConfiguration()
              .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
              .Enrich.FromLogContext()
              .Enrich.WithClientIp()
              .Enrich.WithClientAgent()
              .WriteTo.Console()
              .WriteTo.Seq("http://localhost:5341/")
              .CreateLogger();

            // Wrap creating and running the host in a try-catch block
            try
            {
                Log.Information("Starting host");

                //CreateHostBuilder(args).Build().Run();
                var host = CreateHostBuilder(args).Build();

                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;

                    var context = services.GetRequiredService<ApplicationDbContext>();
                    context.Database.Migrate();

                    var userManager = services.GetService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetService<RoleManager<IdentityRole>>();

                    List<ApplicationUser> accounts = _config.GetSection("Deparment:Accounts").Get<List<ApplicationUser>>();
                    List<string> passwords = _config.GetSection("Deparment:Passwords").Get<List<string>>();

                    roleManager.EnsureRole(Constants.RoleAdmin).Wait();
                    roleManager.EnsureRole(Constants.RoleTeacher).Wait();

                    userManager.EnsureAccounts(accounts, passwords).Wait();
                    foreach (var account in accounts)
                    {
                        userManager.EnsureUserRole(account.Email, Constants.RoleTeacher).Wait();
                    }

                    userManager.EnsureUserRole(Constants.UserAdmin, Constants.RoleAdmin).Wait();
                    userManager.EnsureUserRole(Constants.UserAdmin, Constants.RoleTeacher).Wait();
                }

                host.Run();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }



        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();

                });


    }
}
