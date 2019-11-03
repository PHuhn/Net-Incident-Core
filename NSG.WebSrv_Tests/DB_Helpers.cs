using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
//
using NSG.WebSrv.Domain.Entities;
//
namespace NSG.WebSrv_Tests
{
    public static class DB_Helpers
    {
        public static UserManager<ApplicationUser> GetUserManager( ApplicationDbContext db_context)
        {
            UserStore<ApplicationUser> _userStore = new UserStore<ApplicationUser>(db_context, null);
            IOptions<IdentityOptions> _userIdentityOptions = null;
            PasswordHasher<ApplicationUser> _passwordHasher = new PasswordHasher<ApplicationUser>();
            Mock<ILogger<UserManager<ApplicationUser>>> _mockUserLogger =
                new Mock<ILogger<UserManager<ApplicationUser>>>();
            // userManager = new UserManager<ApplicationUser>(IUserStore<ApplicationUser>, IOptions<IdentityOptions>, IPasswordHasher<TUser>,
            //  IEnumerable<IUserValidator<TUser>>, IEnumerable<IPasswordValidator<TUser>>, ILookupNormalizer, IdentityErrorDescriber, IServiceProvider, ILogger<UserManager<TUser>>)
            return new UserManager<ApplicationUser>(_userStore, _userIdentityOptions, _passwordHasher, null, null, null, null, null, _mockUserLogger.Object);
        }
    }
}
