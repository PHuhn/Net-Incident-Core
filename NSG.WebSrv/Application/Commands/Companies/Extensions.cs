// ---------------------------------------------------------------------------
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//
using NSG.WebSrv.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
//
namespace NSG.WebSrv.Application.Commands.Companies
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
        public static string CompanyToString(this Company entity)
        {
            //
            StringBuilder _return = new StringBuilder("record:[");
            _return.AppendFormat("CompanyId: {0}, ", entity.CompanyId.ToString());
            _return.AppendFormat("CompanyShortName: {0}, ", entity.CompanyShortName);
            _return.AppendFormat("CompanyName: {0}, ", entity.CompanyName);
            _return.AppendFormat("Address: {0}, ", entity.Address);
            _return.AppendFormat("City: {0}, ", entity.City);
            _return.AppendFormat("State: {0}, ", entity.State);
            _return.AppendFormat("PostalCode: {0}, ", entity.PostalCode);
            _return.AppendFormat("Country: {0}, ", entity.Country);
            _return.AppendFormat("PhoneNumber: {0}, ", entity.PhoneNumber);
            _return.AppendFormat("Notes: {0}]", entity.Notes);
            return _return.ToString();
            //
        }
        //
        /// <summary>
        /// Extension method that translates from Company to CompanyListQuery.
        /// <note>
        /// 
        /// SELECT cmp.CompanyId, cmp.CompanyName, cmp.CompanyShortName, srv.ServerId, srv.ServerShortName
        ///   FROM [dbo].[Companies] cmp
        ///   JOIN [dbo].[Servers] AS Srv ON cmp.CompanyId = srv.CompanyId
        /// </note>
        /// </summary>
        /// <param name="entity">The Company entity class.</param>
        /// <param name="selectCompanyId">The Company id to select8.</param>
        /// <returns>'SelectListItem' item.</returns>
        public static SelectListItem ToUserCompanySelectListItem(this Company entity, int selectCompanyId)
        {
            return new SelectListItem
            {
                Value = entity.CompanyId.ToString(),
                Text = entity.CompanyName + "(" + entity.CompanyShortName + ")",
                Selected = (entity.CompanyId == selectCompanyId ? true : false)
            };
        }
    }
    //
}
// ---------------------------------------------------------------------------
