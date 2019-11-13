// ---------------------------------------------------------------------------
using System;
using System.Text;
//
using NSG.WebSrv.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using NSG.WebSrv.Application.Commands.Incidents;
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
        /// <summary>
        /// Extension method that translates from Server to ServerData.
        /// </summary>
        /// <param name="entity">The Server entity class.</param>
        /// <returns>'ServerData' or Server detail query.</returns>
        public static ServerData ToServerData(this Server entity)
        {
            return new ServerData
            {
                ServerId = entity.ServerId,
                CompanyId = entity.CompanyId,
                ServerShortName = entity.ServerShortName,
                ServerName = entity.ServerName,
                ServerDescription = entity.ServerDescription,
                WebSite = entity.WebSite,
                ServerLocation = entity.ServerLocation,
                FromName = entity.FromName,
                FromNicName = entity.FromNicName,
                FromEmailAddress = entity.FromEmailAddress,
                TimeZone = entity.TimeZone,
                DST = entity.DST,
                TimeZone_DST = entity.TimeZone_DST,
                DST_Start = entity.DST_Start,
                DST_End = entity.DST_End,
            };
        }
        //
    }
    //
}
// ---------------------------------------------------------------------------
