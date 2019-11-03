// ---------------------------------------------------------------------------
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
//
using NSG.WebSrv.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
//
namespace NSG.WebSrv.Application.Commands.NetworkLogs
{
    //
    /// <summary>
    /// Extension method.
    /// </summary>
    public static partial class Extensions
    {
        //
    }
    /// <summary>
    /// Extension method.
    /// </summary>
    public static partial class Extensions
    {
        //
        /// <summary>
        /// Extension method that translates from NetworkLog to NetworkLogDetailQuery.
        /// </summary>
        /// <param name="entity">The NetworkLog entity class.</param>
        /// <returns>'NetworkLogDetailQuery' or NetworkLog detail query.</returns>
        public static NetworkLogDetailQuery ToNetworkLogDetailQuery(this NetworkLog entity)
        {
            return new NetworkLogDetailQuery
            {
                NetworkLogId = entity.NetworkLogId,
                ServerId = entity.ServerId,
                IncidentId = entity.IncidentId,
                IPAddress = entity.IPAddress,
                NetworkLogDate = entity.NetworkLogDate,
                Log = entity.Log,
                IncidentTypeId = entity.IncidentTypeId,
            };
        }
        //
        /// <summary>
        /// Extension method that translates from NetworkLog to NetworkLogListQuery.
        /// </summary>
        /// <param name="entity">The NetworkLog entity class.</param>
        /// <returns>'NetworkLogListQuery' or NetworkLog list query.</returns>
        public static NetworkLogListQuery ToNetworkLogListQuery(this NetworkLog entity)
        {
            return new NetworkLogListQuery
            {
                NetworkLogId = entity.NetworkLogId,
                ServerId = entity.ServerId,
                IncidentId = entity.IncidentId,
                IPAddress = entity.IPAddress,
                NetworkLogDate = entity.NetworkLogDate,
                Log = entity.Log,
                IncidentTypeId = entity.IncidentTypeId,
            };
        }
    }
    //
}
// ---------------------------------------------------------------------------
