// ---------------------------------------------------------------------------
using System;
using System.Text;
//
using NSG.WebSrv.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
//
namespace NSG.WebSrv.Application.Commands.Incidents
{
    //
    /// <summary>
    /// Extension method.
    /// </summary>
    public static partial class Extensions
    {
        //
        /// <summary>
        /// Extension method that translates from Incident to IncidentDetailQuery.
        /// </summary>
        /// <param name="entity">The Incident entity class.</param>
        /// <returns>'IncidentDetailQuery' or Incident detail query.</returns>
        public static IncidentDetailQuery ToIncidentDetailQuery(this Incident entity)
        {
            return new IncidentDetailQuery
            {
                IncidentId = entity.IncidentId,
                ServerId = entity.ServerId,
                IPAddress = entity.IPAddress,
                NIC_Id = entity.NIC_Id,
                NetworkName = entity.NetworkName,
                AbuseEmailAddress = entity.AbuseEmailAddress,
                ISPTicketNumber = entity.ISPTicketNumber,
                Mailed = entity.Mailed,
                Closed = entity.Closed,
                Special = entity.Special,
                Notes = entity.Notes,
                CreatedDate = entity.CreatedDate,
            };
        }
        //
        /// <summary>
        /// Extension method that translates from Incident to IncidentListQuery.
        /// </summary>
        /// <param name="entity">The Incident entity class.</param>
        /// <returns>'IncidentListQuery' or Incident list query.</returns>
        public static IncidentListQuery ToIncidentListQuery(this Incident entity)
        {
            return new IncidentListQuery
            {
                IncidentId = entity.IncidentId,
                ServerId = entity.ServerId,
                IPAddress = entity.IPAddress,
                NIC_Id = entity.NIC_Id,
                NetworkName = entity.NetworkName,
                AbuseEmailAddress = entity.AbuseEmailAddress,
                ISPTicketNumber = entity.ISPTicketNumber,
                Mailed = entity.Mailed,
                Closed = entity.Closed,
                Special = entity.Special,
                Notes = entity.Notes,
                CreatedDate = entity.CreatedDate,
            };
        }
        //
        /// <summary>
        /// Create a 'to string'.
        /// </summary>
        public static string IncidentToString(this Incident entity)
        {
            //
            StringBuilder _return = new StringBuilder("record:[");
            _return.AppendFormat("IncidentId: {0}, ", entity.IncidentId.ToString());
            _return.AppendFormat("ServerId: {0}, ", entity.ServerId.ToString());
            _return.AppendFormat("IPAddress: {0}, ", entity.IPAddress);
            _return.AppendFormat("NIC_Id: {0}, ", entity.NIC_Id);
            _return.AppendFormat("NetworkName: {0}, ", entity.NetworkName);
            _return.AppendFormat("AbuseEmailAddress: {0}, ", entity.AbuseEmailAddress);
            _return.AppendFormat("ISPTicketNumber: {0}, ", entity.ISPTicketNumber);
            _return.AppendFormat("Mailed: {0}, ", entity.Mailed.ToString());
            _return.AppendFormat("Closed: {0}, ", entity.Closed.ToString());
            _return.AppendFormat("Special: {0}, ", entity.Special.ToString());
            _return.AppendFormat("Notes: {0}, ", entity.Notes);
            _return.AppendFormat("CreatedDate: {0}]", entity.CreatedDate.ToString());
            return _return.ToString();
        }
    }
    //
}
// ---------------------------------------------------------------------------
