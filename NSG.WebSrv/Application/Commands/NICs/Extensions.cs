// ---------------------------------------------------------------------------
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
//
using NSG.WebSrv.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
//
namespace NSG.WebSrv.Application.Commands.NICs
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
        public static string NICToString(this NIC entity)
        {
            //
            StringBuilder _return = new StringBuilder("record:[");
            _return.AppendFormat("NIC_Id: {0}, ", entity.NIC_Id);
            _return.AppendFormat("NICDescription: {0}, ", entity.NICDescription);
            _return.AppendFormat("NICAbuseEmailAddress: {0}, ", entity.NICAbuseEmailAddress);
            _return.AppendFormat("NICRestService: {0}, ", entity.NICRestService);
            _return.AppendFormat("NICWebSite: {0}]", entity.NICWebSite);
            return _return.ToString();
            //
        }
        //
        /// <summary>
        /// Extension method that translates from NIC to NICDetailQuery.
        /// </summary>
        /// <param name="entity">The NIC entity class.</param>
        /// <returns>'NICDetailQuery' or NIC detail query.</returns>
        public static NICDetailQuery ToNICDetailQuery(this NIC entity)
        {
            return new NICDetailQuery
            {
                NIC_Id = entity.NIC_Id,
                NICDescription = entity.NICDescription,
                NICAbuseEmailAddress = entity.NICAbuseEmailAddress,
                NICRestService = entity.NICRestService,
                NICWebSite = entity.NICWebSite,
            };
        }
        //
        /// <summary>
        /// Extension method that translates from NIC to NICListQuery.
        /// </summary>
        /// <param name="entity">The NIC entity class.</param>
        /// <returns>'NICListQuery' or NIC list query.</returns>
        public static NICListQuery ToNICListQuery(this NIC entity)
        {
            return new NICListQuery
            {
                NIC_Id = entity.NIC_Id,
                NICDescription = entity.NICDescription,
                NICAbuseEmailAddress = entity.NICAbuseEmailAddress,
                NICRestService = entity.NICRestService,
                NICWebSite = entity.NICWebSite,
            };
        }
    }
    //
}
// ---------------------------------------------------------------------------
