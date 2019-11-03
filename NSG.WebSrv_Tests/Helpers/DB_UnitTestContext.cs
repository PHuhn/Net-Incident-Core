using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
//
using NSG.WebSrv.Domain.Entities;
//
namespace NSG.Integration.Helpers
{
    public static class DB_UnitTestContext
    {
        public static UserManager<ApplicationUser> GetUserManager( ApplicationDbContext db_context)
        {
            UserStore<ApplicationUser> _userStore = new UserStore<ApplicationUser>(db_context, null);
            IOptions<IdentityOptions> _userIdentityOptions = null;
            //_userIdentityOptions =
            //    Options.Create<IdentityOptions>(options => options.RoleClaimType = ClaimTypes.Role);
            PasswordHasher<ApplicationUser> _passwordHasher = new PasswordHasher<ApplicationUser>();
            Mock<ILogger<UserManager<ApplicationUser>>> _mockUserLogger =
                new Mock<ILogger<UserManager<ApplicationUser>>>();
            // userManager = new UserManager<ApplicationUser>(IUserStore<ApplicationUser>, IOptions<IdentityOptions>, IPasswordHasher<TUser>,
            //  IEnumerable<IUserValidator<TUser>>, IEnumerable<IPasswordValidator<TUser>>, ILookupNormalizer, IdentityErrorDescriber, IServiceProvider, ILogger<UserManager<TUser>>)
            return new UserManager<ApplicationUser>(_userStore, _userIdentityOptions, _passwordHasher, null, null, null, null, null, _mockUserLogger.Object);
        }
        //
        public static RoleManager<ApplicationRole> GetRoleManager(ApplicationDbContext db_context)
        {
            RoleStore<ApplicationRole> _roleStore = new RoleStore<ApplicationRole>(db_context, null);
            Mock<ILogger<RoleManager<ApplicationRole>>> _mockRoleLogger =
                new Mock<ILogger<RoleManager<ApplicationRole>>>();
            // RoleManager<TRole>(IRoleStore<TRole>, IEnumerable<IRoleValidator<TRole>>, ILookupNormalizer, IdentityErrorDescriber, ILogger<RoleManager<TRole>>)
            return new RoleManager<ApplicationRole>(_roleStore, null, null, null, _mockRoleLogger.Object);
        }
        //
        public static ApplicationDbContext GetInMemoryApplicationDBContext()
        {
            string _name = "DB_" + Guid.NewGuid().ToString();
            Console.WriteLine(_name);
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(_name)
                .EnableSensitiveDataLogging()
                .Options;
            return new ApplicationDbContext(options);
        }
        //
        public static ApplicationDbContext GetSqliteApplicationDBContext()
        {
            ApplicationDbContext _context = null;
            // Use in memory application DB context
            var _optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite("DataSource=:memory:");
            _context = new ApplicationDbContext(_optionsBuilder.Options);
            _context.Database.OpenConnection();
            _context.Database.EnsureCreated();
            return _context;
        }
        //
    }
}
