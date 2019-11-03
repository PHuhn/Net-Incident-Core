// ---------------------------------------------------------------------------
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
//
using NSG.WebSrv.Domain.Entities;
//
namespace NSG.WebSrv.Application.Commands.ApplicationRoles
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
        public static string ApplicationRoleToString(this ApplicationRole entity)
        {
            //
            StringBuilder _return = new StringBuilder("record:[");
            _return.AppendFormat("Id: {0}, ", entity.Id.ToString());
            _return.AppendFormat("Name: {0}, ", entity.Name);
            _return.AppendFormat("NormalizedName: {0}, ", entity.NormalizedName);
            return _return.ToString();
            //
        }
        //
    }
}
