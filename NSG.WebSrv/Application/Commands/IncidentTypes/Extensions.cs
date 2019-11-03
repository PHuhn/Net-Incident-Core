// ---------------------------------------------------------------------------
using System;
using System.Text;
//
using NSG.WebSrv.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
//
namespace NSG.WebSrv.Application.Commands.IncidentTypes
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
        public static string IncidentTypeToString(this IncidentType entity)
        {
            //
            StringBuilder _return = new StringBuilder("record:[");
            _return.AppendFormat("IncidentTypeId: {0}, ", entity.IncidentTypeId.ToString());
            _return.AppendFormat("IncidentTypeShortDesc: {0}, ", entity.IncidentTypeShortDesc);
            _return.AppendFormat("IncidentTypeDesc: {0}, ", entity.IncidentTypeDesc);
            _return.AppendFormat("IncidentTypeFromServer: {0}, ", entity.IncidentTypeFromServer.ToString());
            _return.AppendFormat("IncidentTypeSubjectLine: {0}, ", entity.IncidentTypeSubjectLine);
            _return.AppendFormat("IncidentTypeEmailTemplate: {0}, ", entity.IncidentTypeEmailTemplate);
            _return.AppendFormat("IncidentTypeTimeTemplate: {0}, ", entity.IncidentTypeTimeTemplate);
            _return.AppendFormat("IncidentTypeThanksTemplate: {0}, ", entity.IncidentTypeThanksTemplate);
            _return.AppendFormat("IncidentTypeLogTemplate: {0}, ", entity.IncidentTypeLogTemplate);
            _return.AppendFormat("IncidentTypeTemplate: {0}]", entity.IncidentTypeTemplate);
            return _return.ToString();
            //
        }
        //
        /// <summary>
        /// Extension method that translates from IncidentType to IncidentTypeDetailQuery.
        /// </summary>
        /// <param name="entity">The IncidentType entity class.</param>
        /// <returns>'IncidentTypeDetailQuery' or IncidentType detail query.</returns>
        public static IncidentTypeDetailByShortDescQuery ToIncidentTypeDetailByShortDescQuery(this IncidentType entity)
        {
            return new IncidentTypeDetailByShortDescQuery
            {
                IncidentTypeId = entity.IncidentTypeId,
                IncidentTypeShortDesc = entity.IncidentTypeShortDesc,
                IncidentTypeDesc = entity.IncidentTypeDesc,
                IncidentTypeFromServer = entity.IncidentTypeFromServer,
                IncidentTypeSubjectLine = entity.IncidentTypeSubjectLine,
                IncidentTypeEmailTemplate = entity.IncidentTypeEmailTemplate,
                IncidentTypeTimeTemplate = entity.IncidentTypeTimeTemplate,
                IncidentTypeThanksTemplate = entity.IncidentTypeThanksTemplate,
                IncidentTypeLogTemplate = entity.IncidentTypeLogTemplate,
                IncidentTypeTemplate = entity.IncidentTypeTemplate,
            };
        }
        //
        /// <summary>
        /// Extension method that translates from IncidentType to IncidentTypeDetailQuery.
        /// </summary>
        /// <param name="entity">The IncidentType entity class.</param>
        /// <returns>'IncidentTypeDetailQuery' or IncidentType detail query.</returns>
        public static IncidentTypeDetailQuery ToIncidentTypeDetailQuery(this IncidentType entity)
        {
            return new IncidentTypeDetailQuery
            {
                IncidentTypeId = entity.IncidentTypeId,
                IncidentTypeShortDesc = entity.IncidentTypeShortDesc,
                IncidentTypeDesc = entity.IncidentTypeDesc,
                IncidentTypeFromServer = entity.IncidentTypeFromServer,
                IncidentTypeSubjectLine = entity.IncidentTypeSubjectLine,
                IncidentTypeEmailTemplate = entity.IncidentTypeEmailTemplate,
                IncidentTypeTimeTemplate = entity.IncidentTypeTimeTemplate,
                IncidentTypeThanksTemplate = entity.IncidentTypeThanksTemplate,
                IncidentTypeLogTemplate = entity.IncidentTypeLogTemplate,
                IncidentTypeTemplate = entity.IncidentTypeTemplate,
            };
        }
        //
        /// <summary>
        /// Extension method that translates from IncidentType to IncidentTypeListQuery.
        /// </summary>
        /// <param name="entity">The IncidentType entity class.</param>
        /// <returns>'IncidentTypeListQuery' or IncidentType list query.</returns>
        public static IncidentTypeListQuery ToIncidentTypeListQuery(this IncidentType entity)
        {
            return new IncidentTypeListQuery
            {
                IncidentTypeId = entity.IncidentTypeId,
                IncidentTypeShortDesc = entity.IncidentTypeShortDesc,
                IncidentTypeDesc = entity.IncidentTypeDesc,
                IncidentTypeFromServer = entity.IncidentTypeFromServer,
                IncidentTypeSubjectLine = entity.IncidentTypeSubjectLine,
                IncidentTypeEmailTemplate = entity.IncidentTypeEmailTemplate,
                IncidentTypeTimeTemplate = entity.IncidentTypeTimeTemplate,
                IncidentTypeThanksTemplate = entity.IncidentTypeThanksTemplate,
                IncidentTypeLogTemplate = entity.IncidentTypeLogTemplate,
                IncidentTypeTemplate = entity.IncidentTypeTemplate,
            };
        }
        //
        /// <summary>
        /// Extension method that translates from IncidentType to IncidentTypeSelectionListQuery.
        /// </summary>
        /// <param name="entity">The IncidentType entity class.</param>
        /// <returns>'SelectItemList' list.</returns>
        public static SelectListItem ToIncidentTypeSelectList(this IncidentType entity, int selectIncidentTypeId)
        {
            return new SelectListItem
            {
                Value = entity.IncidentTypeId.ToString(),
                Text = entity.IncidentTypeDesc + "(" + entity.IncidentTypeId.ToString() + "-" + entity.IncidentTypeShortDesc + ")",
                Selected = (entity.IncidentTypeId == selectIncidentTypeId ? true : false)
            };
        }
    }
    //
}
// ---------------------------------------------------------------------------
