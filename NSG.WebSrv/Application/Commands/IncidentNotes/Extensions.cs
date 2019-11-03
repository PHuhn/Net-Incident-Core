// ---------------------------------------------------------------------------
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
//
using NSG.WebSrv.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
//
namespace NSG.WebSrv.Application.Commands.IncidentNotes
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
        /// Should include the following for full results:
        ///  .Include(_r => _r.NoteType)
        ///  .Include(_r => _r.IncidentIncidentNotes)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Formated string</returns>
        public static string IncidentNoteToString(this IncidentNote entity)
        {
            //
            StringBuilder _return = new StringBuilder("record:[");
            long _incidentId = (entity.IncidentIncidentNotes != null && entity.IncidentIncidentNotes.Count != 0
                ? entity.IncidentIncidentNotes.FirstOrDefault().IncidentId: 0);
            _return.AppendFormat("IncidentNoteId: {0}, ", entity.IncidentNoteId.ToString());
            _return.AppendFormat("IncidentId: {0}, ", _incidentId);
            _return.AppendFormat("NoteTypeId: {0}, ", entity.NoteTypeId.ToString(),
                (entity.NoteType != null ? entity.NoteType.NoteTypeShortDesc : "//"));
            _return.AppendFormat("Note: {0}, ", entity.Note);
            _return.AppendFormat("CreatedDate: {0}]", entity.CreatedDate.ToString());
            return _return.ToString();
            //
        }
        //
        /// <summary>
        /// Extension method that translates from IncidentNote to IncidentNoteDetailQuery.
        /// </summary>
        /// <param name="entity">The IncidentNote entity class.</param>
        /// <returns>'IncidentNoteDetailQuery' or IncidentNote detail query.</returns>
        public static IncidentNoteDetailQuery ToIncidentNoteDetailQuery(this IncidentNote entity)
        {
            return new IncidentNoteDetailQuery
            {
                IncidentNoteId = entity.IncidentNoteId,
                NoteTypeId = entity.NoteTypeId,
                Note = entity.Note,
                CreatedDate = entity.CreatedDate,
            };
        }
        //
        /// <summary>
        /// Extension method that translates from IncidentNote to IncidentNoteListQuery.
        /// </summary>
        /// <param name="entity">The IncidentNote entity class.</param>
        /// <returns>'IncidentNoteListQuery' or IncidentNote list query.</returns>
        public static IncidentNoteListQuery ToIncidentNoteListQuery(this IncidentNote entity)
        {
            return new IncidentNoteListQuery
            {
                IncidentNoteId = entity.IncidentNoteId,
                NoteTypeId = entity.NoteTypeId,
                Note = entity.Note,
                CreatedDate = entity.CreatedDate,
            };
        }
    }
    //
}
// ---------------------------------------------------------------------------
