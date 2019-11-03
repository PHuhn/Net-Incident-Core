using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
//
using NSG.WebSrv.Domain.Entities;
using Microsoft.AspNetCore.Identity;
//
namespace NSG.Integration.Helpers
{
    public class TestStartup : NSG.WebSrv.Startup
    {
        //
        public TestStartup(IConfiguration configuration,
            ILoggerFactory loggerFactory) : base(configuration, loggerFactory)
        {
        }

        public void ErrorHandler(object sender, EventArgs e)
        {
        }
        //
        public override void ConfigureDatabase(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("db_context"));
            //
            // Register the database seeder
            services.AddTransient<DatabaseSeeder>();
            //
        }
        // ILoggerFactory loggerFactory
        public override void Configure(
            IApplicationBuilder app, IHostingEnvironment env,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            app.UseExceptionHandler(errorApp =>
            {

                System.Diagnostics.Debug.Write(errorApp.ToString());
                //  ((System.Web.HttpApplication)sender).Server.GetLastError().ToString()
            });
            // Perform all the configuration in the base class
            base.Configure(app, env, context, userManager, roleManager);
            // Now seed the database
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var _seeder = serviceScope.ServiceProvider.GetService<DatabaseSeeder>();
                _seeder.Seed().Wait();
            }
        }
        //
    }
}
