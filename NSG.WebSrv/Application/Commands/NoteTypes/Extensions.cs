// ---------------------------------------------------------------------------
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
//
using NSG.WebSrv.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
//
namespace NSG.WebSrv.Application.Commands.NoteTypes
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
        /// </summary>
        public static string NoteTypeToString(this NoteType entity)
        {
            //
            StringBuilder _return = new StringBuilder("record:[");
            _return.AppendFormat("NoteTypeId: {0}, ", entity.NoteTypeId.ToString());
            _return.AppendFormat("NoteTypeShortDesc: {0}, ", entity.NoteTypeShortDesc);
            _return.AppendFormat("NoteTypeDesc: {0}, ", entity.NoteTypeDesc);
            _return.AppendFormat("NoteTypeClientScript: {0}]", entity.NoteTypeClientScript);
            return _return.ToString();
            //
        }
        //
        /// <summary>
        /// Extension method that translates from NoteType to NoteTypeDetailQuery.
        /// </summary>
        /// <param name="entity">The NoteType entity class.</param>
        /// <returns>'NoteTypeDetailQuery' or NoteType detail query.</returns>
        public static NoteTypeDetailQuery ToNoteTypeDetailQuery(this NoteType entity)
        {
            return new NoteTypeDetailQuery
            {
                NoteTypeId = entity.NoteTypeId,
                NoteTypeShortDesc = entity.NoteTypeShortDesc,
                NoteTypeDesc = entity.NoteTypeDesc,
                NoteTypeClientScript = entity.NoteTypeClientScript
            };
        }
        //
        /// <summary>
        /// Extension method that translates from NoteType to NoteTypeListQuery.
        /// </summary>
        /// <param name="entity">The NoteType entity class.</param>
        /// <returns>'NoteTypeListQuery' or NoteType list query.</returns>
        public static NoteTypeListQuery ToNoteTypeListQuery(this NoteType entity)
        {
            return new NoteTypeListQuery
            {
                NoteTypeId = entity.NoteTypeId,
                NoteTypeShortDesc = entity.NoteTypeShortDesc,
                NoteTypeDesc = entity.NoteTypeDesc,
                NoteTypeClientScript = entity.NoteTypeClientScript
            };
        }
    }
    //
}
// ---------------------------------------------------------------------------
