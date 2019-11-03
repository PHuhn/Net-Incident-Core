// ---------------------------------------------------------------------------
using System;
using System.Text;
//
using NSG.WebSrv.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
//
namespace NSG.WebSrv.Application.Commands.Servers
{
    /// <summary>
    /// Extension method.
    /// </summary>
    public static partial class Extensions
    {
        //
        /// <summary>
        /// Create a 'to string'.
        /// </summary>
        public static string ServerToString(this Server entity)
        {
            StringBuilder _return = new StringBuilder("record:[");
            _return.AppendFormat("ServerId: {0}, ", entity.ServerId.ToString());
            _return.AppendFormat("CompanyId: {0}, ", entity.CompanyId.ToString());
            _return.AppendFormat("ServerShortName: {0}, ", entity.ServerShortName);
            _return.AppendFormat("ServerName: {0}, ", entity.ServerName);
            _return.AppendFormat("ServerDescription: {0}, ", entity.ServerDescription);
            _return.AppendFormat("WebSite: {0}, ", entity.WebSite);
            _return.AppendFormat("ServerLocation: {0}, ", entity.ServerLocation);
            _return.AppendFormat("FromName: {0}, ", entity.FromName);
            _return.AppendFormat("FromNicName: {0}, ", entity.FromNicName);
            _return.AppendFormat("FromEmailAddress: {0}, ", entity.FromEmailAddress);
            _return.AppendFormat("TimeZone: {0}, ", entity.TimeZone);
            _return.AppendFormat("DST: {0}, ", entity.DST.ToString());
            _return.AppendFormat("TimeZone_DST: {0}, ", entity.TimeZone_DST);
            if (entity.DST_Start.HasValue)
                _return.AppendFormat("DST_Start: {0}, ", entity.DST_Start.ToString());
            else
                _return.AppendFormat("/DST_Start/, ");
            if (entity.DST_End.HasValue)
                _return.AppendFormat("DST_End: {0}]", entity.DST_End.ToString());
            else
                _return.AppendFormat("/DST_End/]");
            return _return.ToString();
        }
        //
    }
    //
}
// ---------------------------------------------------------------------------
