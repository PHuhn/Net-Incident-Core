// ---------------------------------------------------------------------------
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
//
using NSG.WebSrv.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
//
namespace NSG.WebSrv.Application.Commands.CompanyServers
{
    //
    /// <summary>
    /// Extension method.
    /// </summary>
    public static partial class Extensions
    {
        //
        /// <summary>
        /// Extension method that translates from Company to CompanyDetailQuery.
        /// </summary>
        /// <param name="entity">The Company entity class.</param>
        /// <returns>'CompanyDetailQuery' or Company detail query.</returns>
        public static CompanyServerDetailQuery ToCompanyDetailQuery(this Company entity)
        {
            return new CompanyServerDetailQuery
            {
                CompanyId = entity.CompanyId,
                CompanyShortName = entity.CompanyShortName,
                CompanyName = entity.CompanyName,
                Address = entity.Address,
                City = entity.City,
                State = entity.State,
                PostalCode = entity.PostalCode,
                Country = entity.Country,
                PhoneNumber = entity.PhoneNumber,
                Notes = entity.Notes,
                ServerList = entity.Servers
                    .Select(srv => srv.ToServerDetailQuery()).ToList()
            };
        }
        //
        /// <summary>
        /// Extension method that translates from Server to ServerDetailQuery.
        /// </summary>
        /// <param name="entity">The Server entity class.</param>
        /// <returns>'ServerDetailQuery' or Server detail query.</returns>
        public static ServerDetailQuery ToServerDetailQuery(this Server entity)
        {
            return new ServerDetailQuery
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
        /// <returns>'CompanyListQuery' or Company list query.</returns>
        public static CompanyServerListQuery ToCompanyListQuery(this Company entity)
        {
            return new CompanyServerListQuery
            {
                CompanyId = entity.CompanyId,
                CompanyName = entity.CompanyName,
                CompanyShortName = entity.CompanyShortName,
                ServerList = entity.Servers
                    .Select(srv => srv.ToServerListQuery()).ToList()
            };
        }
        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static ServerListQuery ToServerListQuery(this Server entity)
        {
            return new ServerListQuery
            {
                ServerId = entity.ServerId,
                ServerShortName = entity.ServerShortName
            };
        }
    }
    //
}
// ---------------------------------------------------------------------------
