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
namespace NSG.WebSrv.Application.Commands.CompanyEmailTemplates
{
    //
    /// <summary>
    /// Extension method.
    /// </summary>
    public static partial class Extensions
    {
        //
        /// <summary>
        /// Create a 'to string' for IncidentType entity.
        /// </summary>
        public static string EmailTemplateToString(this EmailTemplate entity)
        {
            //
            StringBuilder _return = new StringBuilder("record:[");
            _return.AppendFormat("CompanyId: {0}, ", entity.CompanyId.ToString());
            _return.AppendFormat("IncidentTypeId: {0}, ", entity.IncidentTypeId.ToString());
            _return.AppendFormat("SubjectLine: {0}, ", entity.SubjectLine);
            _return.AppendFormat("EmailBody: {0}, ", entity.EmailBody);
            _return.AppendFormat("TimeTemplate: {0}, ", entity.TimeTemplate);
            _return.AppendFormat("ThanksTemplate: {0}, ", entity.ThanksTemplate);
            _return.AppendFormat("LogTemplate: {0}, ", entity.LogTemplate);
            _return.AppendFormat("Template: {0}, ", entity.Template);
            _return.AppendFormat("FromServer: {0}]", entity.FromServer.ToString());
            return _return.ToString();
            //
        }
        //
        /// <summary>
        /// Extension method that translates from EmailTemplate to EmailTemplateDetailQuery.
        /// </summary>
        /// <param name="entity">The EmailTemplate entity class.</param>
        /// <returns>'EmailTemplateDetailQuery' or EmailTemplate detail query.</returns>
        public static CompanyEmailTemplateDetailQuery ToCompanyEmailTemplateDetailQuery(this EmailTemplate entity)
        {
            return new CompanyEmailTemplateDetailQuery
            {
                CompanyId = entity.CompanyId,
                CompanyShortName = entity.Company.CompanyShortName,
                CompanyName = entity.Company.CompanyName,
                IncidentTypeId = entity.IncidentTypeId,
                IncidentTypeShortDesc = entity.IncidentType.IncidentTypeShortDesc,
                IncidentTypeDesc = entity.IncidentType.IncidentTypeDesc,
                SubjectLine = entity.SubjectLine,
                EmailBody = entity.EmailBody,
                TimeTemplate = entity.TimeTemplate,
                ThanksTemplate = entity.ThanksTemplate,
                LogTemplate = entity.LogTemplate,
                Template = entity.Template,
                FromServer = entity.FromServer,
            };
        }
        //
        /// <summary>
        /// Extension method that translates from EmailTemplate to EmailTemplateListQuery.
        /// </summary>
        /// <param name="entity">The EmailTemplate entity class.</param>
        /// <returns>'EmailTemplateListQuery' or EmailTemplate list query.</returns>
        public static CompanyEmailTemplateListQuery ToCompanyEmailTemplateListQuery(this EmailTemplate entity)
        {
            return new CompanyEmailTemplateListQuery
            {
                CompanyId = entity.CompanyId,
                IncidentTypeId = entity.IncidentTypeId,
                IncidentTypeShortDesc = entity.IncidentType.IncidentTypeShortDesc,
                IncidentTypeDesc = entity.IncidentType.IncidentTypeDesc,
                SubjectLine = entity.SubjectLine,
                EmailBody = entity.EmailBody,
                //TimeTemplate = entity.TimeTemplate,
                //ThanksTemplate = entity.ThanksTemplate,
                //LogTemplate = entity.LogTemplate,
                //Template = entity.Template,
                FromServer = entity.FromServer,
            };
        }
        //
        /// <summary>
        /// Extension method that translates from CompanyEmailTemplateDetailQuery to CompanyEmailTemplateUpdateCommand.
        /// </summary>
        /// <param name="query">The CompanyEmailTemplateDetailQuery class.</param>
        /// <returns>'CompanyEmailTemplateUpdateCommand' or EmailTemplate detail query.</returns>
        public static CompanyEmailTemplateUpdateCommand ToCompanyEmailTemplateUpdateCommand(this CompanyEmailTemplateDetailQuery query)
        {
            return new CompanyEmailTemplateUpdateCommand
            {
                CompanyId = query.CompanyId,
                IncidentTypeId = query.IncidentTypeId,
                SubjectLine = query.SubjectLine,
                EmailBody = query.EmailBody,
                TimeTemplate = query.TimeTemplate,
                ThanksTemplate = query.ThanksTemplate,
                LogTemplate = query.LogTemplate,
                Template = query.Template,
                FromServer = query.FromServer,
            };
        }
        //
        /// <summary>
        /// Extension method that translates from EmailTemplate to EmailTemplateListQuery.
        /// </summary>
        /// <param name="entity">The EmailTemplate entity class.</param>
        /// <returns>'EmailTemplateListQuery' or EmailTemplate list query.</returns>
        public static SelectListItem ToCompanyEmailTemplateSelectionListQuery(this EmailTemplate entity)
        {
            return new SelectListItem
            {
                Value = entity.IncidentTypeId.ToString(),
                Text = entity.IncidentType.IncidentTypeDesc + "(" + entity.IncidentTypeId.ToString() + "-" + entity.IncidentType.IncidentTypeShortDesc + ")"
            };
        }
        //
    }
    //
}
// ---------------------------------------------------------------------------
