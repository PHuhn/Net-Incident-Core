//
// ---------------------------------------------------------------------------
// Application Users.
//
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
//
namespace NSG.WebSrv.Domain.Entities
{
    //
    // https://docs.microsoft.com/en-us/aspnet/core/security/authentication/customize-identity-model?view=aspnetcore-2.2
    // https://adrientorris.github.io/aspnet-core/identity/extend-user-model.html
    // Extending and Seeding Identity Users & Roles in ASP.NET Core 2.1
    // https://www.youtube.com/watch?v=LFGt84fZlAM 
    //
    // Contains:
    //  ApplicationUser,
    //  ApplicationUserStore,
    //  ApplicationUserManager.
    //
    /// <summary>
    /// Custom Users
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        //
        // [Id] [nvarchar](450) NOT NULL,
        // [UserName] [nvarchar](256) NULL,
        // [NormalizedUserName] [nvarchar](256) NULL,
        // [Email] [nvarchar](256) NULL,
        // [NormalizedEmail] [nvarchar](256) NULL,
        // [EmailConfirmed] [bit] NOT NULL,
        // [PasswordHash] [nvarchar](max) NULL,
        // [SecurityStamp] [nvarchar](max) NULL,
        // [ConcurrencyStamp] [nvarchar](max) NULL,
        // [PhoneNumber] [nvarchar](max) NULL,
        // [PhoneNumberConfirmed] [bit] NOT NULL,
        // [TwoFactorEnabled] [bit] NOT NULL,
        // [LockoutEnd] [datetimeoffset](7) NULL,
        // [LockoutEnabled] [bit] NOT NULL,
        // [AccessFailedCount] [int] NOT NULL,
        //
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }
        [Required]
        [MaxLength(16)]
        public string UserNicName { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public DateTime CreateDate { get; set; }
        //
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }
        //
        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }
        public virtual ICollection<IdentityUserToken<string>> Tokens { get; set; }
        public virtual ICollection<ApplicationUserServer> UserServers { get; }
            = new List<ApplicationUserServer>();
        //
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
            = new List<ApplicationUserRole>();
        //
        /// <summary>
        /// No parameter constructor, assigns a guid to the Id.
        /// </summary>
        public ApplicationUser() : base() { }
        //
    }
    //
}
//