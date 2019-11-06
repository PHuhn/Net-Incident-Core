// ---------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
//
using NSG.WebSrv.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
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
        public static NetworkIncidentDetailQuery ToNetworkIncidentDetailQuery(this Incident entity)
        {
            return new NetworkIncidentDetailQuery
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
                // CreatedDate = entity.CreatedDate,
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
        //
        public static IncidentNoteData ToIncidentNoteData(this IncidentNote entity)
        {
            return new IncidentNoteData()
            {
                IncidentNoteId = entity.IncidentNoteId,
                NoteTypeId = entity.NoteTypeId,
                NoteTypeShortDesc = entity.NoteType.NoteTypeShortDesc,
                Note = entity.Note,
                CreatedDate = entity.CreatedDate,
                IsChanged = false
            };
        }
        //
        //  GetNoteTypes
        //  GetNICs
        //  GetIncidentTypes
        //
        #region "Get SelectItems"
        //
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<SelectItem> GetNoteTypes(IDb_Context context)
        {
            return
                context.NoteTypes
                .Select(_nt => new SelectItem
                {
                    value = _nt.NoteTypeId.ToString(),
                    label = _nt.NoteTypeShortDesc
                }).ToList();
        }
        //
        public static List<SelectItem> GetNICs(IDb_Context context)
        {
            return
                context.NICs
                .Select(_n => new SelectItem
                {
                    value = _n.NIC_Id,
                    label = _n.NIC_Id
                }).ToList();
        }
        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<SelectItem> GetIncidentTypes(IDb_Context context)
        {
            return
                context.IncidentTypes
                .Where(_it => _it.IncidentTypeId > 0)
                .Select(_it => new SelectItem
                {
                    value = _it.IncidentTypeId.ToString(),
                    label = _it.IncidentTypeShortDesc
                }).ToList();
        }
        //
        #endregion
        //
        /// <summary>
        /// Get an entity record via the primary key.
        /// </summary>
        /// <param name="incidentId">long key</param>
        /// <returns>Returns a Incident entity record.</returns>
        public static Task<Server> GetServerByKey(IDb_Context context, int serverId)
        {
            return context.Servers.SingleOrDefaultAsync(r => r.ServerId == serverId);
        }
        //
        /// <summary>
        /// Return a list with all rows of default IncidentType and
        /// overridden specific EmailTemplate for a company.
        /// </summary>
        /// <param name="serverId"></param>
        /// <returns></returns>
        public static List<IncidentTypeData> GetCompanyIncidentType(IDb_Context context, int companyId)
        {
            string _companyNumber = " (" + companyId.ToString() + ")";
            return
                (from _i in context.IncidentTypes
                 join _r in context.EmailTemplates
                 on new { _i.IncidentTypeId, CompanyId = companyId } equals new { _r.IncidentTypeId, _r.CompanyId } into g
                 from _et in g.DefaultIfEmpty()
                 select new IncidentTypeData()
                 {
                     IncidentTypeId = _i.IncidentTypeId,
                     IncidentTypeShortDesc = _et == null ? _i.IncidentTypeShortDesc : _i.IncidentTypeShortDesc + _companyNumber,
                     IncidentTypeDesc = _et == null ? _i.IncidentTypeDesc : _i.IncidentTypeDesc + _companyNumber,
                     IncidentTypeFromServer = _et == null ? _i.IncidentTypeFromServer : _et.FromServer,
                     IncidentTypeSubjectLine = _et == null ? _i.IncidentTypeSubjectLine : _et.SubjectLine,
                     IncidentTypeEmailTemplate = _et == null ? _i.IncidentTypeEmailTemplate : _et.EmailBody,
                     IncidentTypeTimeTemplate = _et == null ? _i.IncidentTypeTimeTemplate : _et.TimeTemplate,
                     IncidentTypeThanksTemplate = _et == null ? _i.IncidentTypeThanksTemplate : _et.ThanksTemplate,
                     IncidentTypeLogTemplate = _et == null ? _i.IncidentTypeLogTemplate : _et.LogTemplate,
                     IncidentTypeTemplate = _et == null ? _i.IncidentTypeTemplate : _et.Template
                 }).ToList();
        }
        //
    }
    //
}
// ---------------------------------------------------------------------------
