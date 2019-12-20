//
using System;
using System.Security.Claims;
//
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
//
namespace NSG.WebSrv.Infrastructure.Common
{
    public class ApplicationImplementation : IApplication
    {
        //
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationSettings _applicationSettings;
        //
        // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-context?view=aspnetcore-2.2
        /// <summary>
        /// In startup:
        ///     services.AddHttpContextAccessor();
        ///     services.AddTransient<IUserRepository, UserRepository>();
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        public ApplicationImplementation(IHttpContextAccessor httpContextAccessor, IOptions<ApplicationSettings> applicationSettings)
        {
            _httpContextAccessor = httpContextAccessor;
            _applicationSettings = applicationSettings.Value;
        }
        //
        /// <summary>
        /// Return a string of the Application Name.
        /// </summary>
        /// <returns>string of Application Name</returns>
        public string GetApplicationName()
        {
            return _applicationSettings.Name;
        }
        //
        /// <summary>
        /// Return a string of the Application Name.
        /// </summary>
        /// <returns>string of Application phone #</returns>
        public string GetApplicationPhoneNumber()
        {
            return _applicationSettings.Phone;
        }
        //
        /// <summary>
        /// Return a date-time of the current date/time.
        /// </summary>
        /// <returns>DateTime of the current date/time.</returns>
        public DateTime Now()
        {
            return DateTime.Now;
        }
        //
        /// <summary>
        /// Get the current user's ClaimsPrincipal via HttpContext.
        /// </summary>
        /// <returns>ClaimsPrincipal of the current user.</returns>
        public ClaimsPrincipal GetUserClaimsPrincipal()
        {
            ClaimsPrincipal _claimsPrincipal = new ClaimsPrincipal();
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                _claimsPrincipal = _httpContextAccessor.HttpContext.User;
            return _claimsPrincipal;
        }
        //
        /// <summary>
        /// Get the current user's user name identity via HttpContext.
        /// </summary>
        /// <returns>String of the current user.</returns>
        public string GetUserAccount()
        {
            var _currentUserName = "- Not Authenticated -";
            ClaimsPrincipal _claimsPrincipal = GetUserClaimsPrincipal();
            if (_claimsPrincipal.Identity.IsAuthenticated)
                _currentUserName = _httpContextAccessor.HttpContext.User.Identity.Name;
            return _currentUserName;
        }
        //
        /// <summary>
        /// Get the current user's authentication status.
        /// </summary>
        /// <returns>boolean value if user is authenticated.</returns>
        public bool IsAuthenticated()
        {
            ClaimsPrincipal _claimsPrincipal = GetUserClaimsPrincipal();
            return _claimsPrincipal.Identity.IsAuthenticated;
        }
        //
        // Base roles are as follows:
        //  Public          has access to basic grid of incidents for given company/server
        //  User            has create and edit of incidents for given company/server
        //  CompanyAdmin    has create, edit and delete of incidents for given company/server
        //  Admin           administrator
        static string adminRole = "admin";
        static string companyadminRole = "companyadmin";
        static string userRole = "user";
        //
        /// <summary>
        /// Is the user in Admin role.
        /// </summary>
        /// <returns>boolean value if user is in role.</returns>
        public bool IsAdminRole()
        {
            bool _admin = false;
            ClaimsPrincipal _claimsPrincipal = GetUserClaimsPrincipal();
            if (_claimsPrincipal.Identity.IsAuthenticated)
                _admin = _claimsPrincipal.IsInRole(adminRole);
            return _admin;
        }
        //
        /// <summary>
        /// Is the user in either Admin or CompanyAdmin roles.
        /// </summary>
        /// <returns>boolean value if user is in role.</returns>
        public bool IsCompanyAdminRole()
        {
            bool _company = false;
            ClaimsPrincipal _claimsPrincipal = GetUserClaimsPrincipal();
            if (_claimsPrincipal.Identity.IsAuthenticated)
            {
                _company = IsAdminRole();
                if (!_company)
                    _company = _claimsPrincipal.IsInRole(companyadminRole);
            }
            return _company;
        }
        //
        /// <summary>
        /// Is the user in either Admin or CompanyAdmin roles.
        /// </summary>
        /// <returns>boolean value if user is in role.</returns>
        public bool IsEditableRole()
        {
            bool _editable = false;
            ClaimsPrincipal _claimsPrincipal = GetUserClaimsPrincipal();
            if (_claimsPrincipal.Identity.IsAuthenticated)
            {
                _editable = IsAdminRole();
                if (!_editable)
                    _editable = _claimsPrincipal.IsInRole(companyadminRole);
                if (!_editable)
                    _editable = _claimsPrincipal.IsInRole(userRole);
            }
            return _editable;
        }
        //
    }
}
//
