using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
//
using FluentValidation.AspNetCore;
using MediatR;
//
using NSG.WebSrv.Domain.Entities;
using NSG.WebSrv.Infrastructure.Notification;
using NSG.WebSrv.Infrastructure.Common;
using NSG.WebSrv.Infrastructure.Services;
using NSG.WebSrv.Infrastructure.Authentication;
using System.IdentityModel.Tokens.Jwt;
//
namespace NSG.WebSrv
{
    public class Startup
    {
        //
        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            LoggerFactory = loggerFactory;
        }
        //
        /// <summary>
        /// IConfiguration, common configuration provider.
        /// </summary>
        public IConfiguration Configuration { get; }
        //
        /// <summary>
        /// The logger factory
        /// </summary>
        public static ILoggerFactory LoggerFactory = null;
        //
        /// <summary>
        /// The configured logger (console).
        /// </summary>
        public static ILogger<ConsoleLoggerProvider> AppLogger = null;
        private object options;

        //
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Configure logging
            // Unable to resolve service for type 'Microsoft.Extensions.Logging.ILogger' while attempting to activate 'NSG.WebSrv.Infrastructure.Notification.NotificationService'
            services.AddLogging(builder => builder
                .AddConfiguration(Configuration.GetSection("Logging"))
                .AddConsole()
                .AddDebug()
            );
            AppLogger = LoggerFactory.CreateLogger<ConsoleLoggerProvider>();
            services.TryAdd(ServiceDescriptor.Singleton<ILoggerFactory, LoggerFactory>());
            services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)));
            // Add and configure email/notification services
            services.Configure<MimeKit.NSG.EmailSettings>(Configuration.GetSection("EmailSettings"));
            services.Configure<ServicesSettings>(Configuration.GetSection("ServicesSettings"));
            // services.Configure<AuthSettings>(Configuration.GetSection("AuthSettings"));
            AuthSettings _authSettings = new AuthSettings();
            _authSettings = Options.Create<AuthSettings>(
                Configuration.GetSection("AuthSettings").Get<AuthSettings>()).Value;
            services.AddSingleton<AuthSettings>(_authSettings);
            //
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            // Add MediatR from MediatR.Extensions.Microsoft.DependencyInjection package
            services.AddMediatR(System.Reflection.Assembly.GetExecutingAssembly());
            // overridable
            ConfigureDatabase(services);
            services.AddScoped<IDb_Context, ApplicationDbContext>();
            // inject HttpContext accessor into controller
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //
            // call services.AddIdentity<ApplicationUser, ApplicationRole>()
            services.AddApplicationIdentity();
            //
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminRole", policy => policy.RequireRole("Admin"));
                options.AddPolicy("CompanyAdminRole", policy => policy.RequireRole("Admin", "CompanyAdmin"));
                options.AddPolicy("AnyUserRole", policy => policy.RequireRole("User", "Admin", "CompanyAdmin"));
                Console.WriteLine(options);
            });
            //
            services.AddHttpContextAccessor();
            // JWT Authentication
            // https://stackoverflow.com/questions/56093946/how-can-i-implement-cookie-base-authentication-and-jwt-in-asp-net-core-2-2
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // clear defaults
            services.AddAuthentication()
                .AddCookie(cfg => cfg.SlidingExpiration = true)
                .AddJwtBearer(options =>
                {
                    //
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_authSettings.JwtSecret)),
                        ValidIssuer = _authSettings.JwtIssuer,
                        ValidAudience = _authSettings.JwtAudience,
                        ClockSkew = TimeSpan.Zero, // remove expire delay
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
            //
            // Add email/notification services
            //
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddSingleton<IEmailSender, NotificationService>();
            // Add framework services.
            services.AddTransient<IApplication, ApplicationImplementation>();
            //
            services.AddMvc()
                .AddApplicationPart(typeof(Startup).Assembly)
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddSessionStateTempDataProvider()
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>())
                .AddRazorPagesOptions(options =>
                {
                    options.AllowAreas = false;
                    options.Conventions.Clear();
                    options.RootDirectory = "/UI/Identity";
                    Console.WriteLine(options);
                    //options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
                    //options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
                });
            //
            services.Configure<RazorViewEngineOptions>(o =>
            {
                // https://github.com/OdeToCode/AddFeatureFolders
                // {2} is area, {1} is controller,{0} is the action
                o.ViewLocationFormats.Clear();
                o.ViewLocationFormats.Add("/UI/Views/{1}/{0}" + RazorViewEngine.ViewExtension);
                o.ViewLocationFormats.Add("/UI/Views/Admin/{1}/{0}" + RazorViewEngine.ViewExtension);
                o.ViewLocationFormats.Add("/UI/Views/CompanyAdmin/{1}/{0}" + RazorViewEngine.ViewExtension);
                o.ViewLocationFormats.Add("/UI/Views/Shared/{0}" + RazorViewEngine.ViewExtension);
                // now razor pages
                o.PageViewLocationFormats.Clear();
                o.PageViewLocationFormats.Add("/UI/Views/{1}/{0}" + RazorViewEngine.ViewExtension);
                o.PageViewLocationFormats.Add("/UI/Views/Admin/{1}/{0}" + RazorViewEngine.ViewExtension);
                o.PageViewLocationFormats.Add("/UI/Views/CompanyAdmin/{1}/{0}" + RazorViewEngine.ViewExtension);
                o.PageViewLocationFormats.Add("/UI/Views/Shared/{0}" + RazorViewEngine.ViewExtension);
                o.PageViewLocationFormats.Add("/UI/Identity/Account/Manage/{0}" + RazorViewEngine.ViewExtension);
                //
                o.AreaViewLocationFormats.Clear();
                o.AreaViewLocationFormats.Add("/UI/Views/{1}/{0}" + RazorViewEngine.ViewExtension);
                o.AreaViewLocationFormats.Add("/UI/Views/Admin/{1}/{0}" + RazorViewEngine.ViewExtension);
                o.AreaViewLocationFormats.Add("/UI/Views/CompanyAdmin/{1}/{0}" + RazorViewEngine.ViewExtension);
                o.AreaViewLocationFormats.Add("/UI/Views/Shared/{0}" + RazorViewEngine.ViewExtension);
                // now razor areas
                o.AreaPageViewLocationFormats.Clear();
                o.AreaPageViewLocationFormats.Add("/UI/Views/{1}/{0}" + RazorViewEngine.ViewExtension);
                o.AreaPageViewLocationFormats.Add("/UI/Views/Admin/{1}/{0}" + RazorViewEngine.ViewExtension);
                o.AreaPageViewLocationFormats.Add("/UI/Views/CompanyAdmin/{1}/{0}" + RazorViewEngine.ViewExtension);
                o.AreaPageViewLocationFormats.Add("/UI/Views/Shared/{0}" + RazorViewEngine.ViewExtension);
                o.AreaPageViewLocationFormats.Add("/UI/Identity/Account/Manage/{0}" + RazorViewEngine.ViewExtension);
                Console.WriteLine(o);
            });
            services.AddSession();
        }
        //
        /// <summary>
        /// An overridable method, that allows for different configuration.
        /// </summary>
        /// <param name="services">The current collection of services, 
        /// add DB contect to the container.
        /// </param>
        public virtual void ConfigureDatabase(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
        }
        //
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseSession();
            //
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            //
            // Domain.Entities.Migrations.SeedData.Initialize(context, roleManager, false).Wait();
            // Domain.Entities.Migrations.SeedData.AddRoleToPhil(context, userManager).Wait();
            // Domain.Entities.Migrations.SeedData.TestUserManager(context, userManager, roleManager).Wait();
            //
        }
        //
    }
    //
    /// <summary>
    /// Startup Extensions
    /// </summary>
    public static class StartupExtensions
    {
        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddApplicationIdentity(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, ApplicationRole>(
                options => {
                    options.Stores.MaxLengthForKeys = 128;
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 8;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddUserManager<UserManager<ApplicationUser>>()
                .AddRoleManager<RoleManager<ApplicationRole>>()
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddDefaultTokenProviders();
            //
            //  .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, string>>()
            //  .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, string>>()
            return services;
        }
        //
    }
    //
}
