//
using System;
using System.Security.Claims;
//
namespace NSG.WebSrv.Infrastructure.Common
{
    public interface IApplication
    {
        //
        /// <summary>
        /// Return a string of the Application Name.
        /// </summary>
        /// <returns>string of Application Name</returns>
        string GetApplicationName();
        //
        /// <summary>
        /// Return a date-time of the current date/time.
        /// </summary>
        /// <returns>DateTime of the current date/time.</returns>
        DateTime Now();
        //
        /// <summary>
        /// Get the current user's ClaimsPrincipal via HttpContext.
        /// </summary>
        /// <returns>ClaimsPrincipal of the current user.</returns>
        ClaimsPrincipal GetUserClaimsPrincipal();
        //
        /// <summary>
        /// Get the current user's authentication status.
        /// </summary>
        /// <returns>boolean value if user is authenticated.</returns>
        bool IsAuthenticated();
        //
        /// <summary>
        /// Get the current user's user name identity.
        /// </summary>
        /// <returns>String of the current user.</returns>
        string GetUserAccount();
        //
        /// <summary>
        /// Is the user in Admin role.
        /// </summary>
        /// <returns>boolean value if user is in role.</returns>
        bool IsAdmin();
        //
        /// <summary>
        /// Is the user in either Admin or CompanyAdmin roles.
        /// </summary>
        /// <returns>boolean value if user is in role.</returns>
        bool IsCompanyAdmin();
        //
        /// <summary>
        /// Is the user in either Admin or CompanyAdmin roles.
        /// </summary>
        /// <returns>boolean value if user is in role.</returns>
        bool IsEditable();
        //
    }
}
