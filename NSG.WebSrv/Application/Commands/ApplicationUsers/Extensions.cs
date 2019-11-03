// ---------------------------------------------------------------------------
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
//
using NSG.WebSrv.Domain.Entities;
//
namespace NSG.WebSrv.Application.Commands.ApplicationUsers
{
    //
    /// <summary>
    /// Extension method.
    /// </summary>
    public static partial class Extensions
    {
        //
        /// <summary>
        /// Create a 'to string'.
        /// </summary>
        public static string ApplicationUserToString(this ApplicationUser entity)
        {
            //
            StringBuilder _return = new StringBuilder("record:[");
            _return.AppendFormat("Id: {0}, ", entity.Id);
            _return.AppendFormat("UserName: {0}, ", entity.UserName);
            _return.AppendFormat("NormalizedUserName: {0}, ", entity.NormalizedUserName);
            _return.AppendFormat("Email: {0}, ", entity.Email);
            _return.AppendFormat("NormalizedEmail: {0}, ", entity.NormalizedEmail);
            _return.AppendFormat("EmailConfirmed: {0}, ", entity.EmailConfirmed.ToString());
            _return.AppendFormat("PasswordHash: {0}, ", entity.PasswordHash);
            _return.AppendFormat("SecurityStamp: {0}, ", entity.SecurityStamp);
            _return.AppendFormat("ConcurrencyStamp: {0}, ", entity.ConcurrencyStamp);
            _return.AppendFormat("PhoneNumber: {0}, ", entity.PhoneNumber);
            _return.AppendFormat("PhoneNumberConfirmed: {0}, ", entity.PhoneNumberConfirmed.ToString());
            _return.AppendFormat("TwoFactorEnabled: {0}, ", entity.TwoFactorEnabled.ToString());
            // if (LockoutEnd.HasValue)
            _return.AppendFormat("LockoutEnd: {0}, ", entity.LockoutEnd.ToString());
            _return.AppendFormat("LockoutEnabled: {0}, ", entity.LockoutEnabled.ToString());
            _return.AppendFormat("AccessFailedCount: {0}, ", entity.AccessFailedCount.ToString());
            _return.AppendFormat("CompanyId: {0}, ", entity.CompanyId.ToString());
            _return.AppendFormat("CreateDate: {0}, ", entity.CreateDate.ToString());
            _return.AppendFormat("FirstName: {0}, ", entity.FirstName);
            _return.AppendFormat("FullName: {0}, ", entity.FullName);
            _return.AppendFormat("LastName: {0}, ", entity.LastName);
            _return.AppendFormat("UserNicName: {0}]", entity.UserNicName);
            return _return.ToString();
            //
        }
        //
    }
}
