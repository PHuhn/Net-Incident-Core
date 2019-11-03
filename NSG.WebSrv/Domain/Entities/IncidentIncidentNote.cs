//
// ---------------------------------------------------------------------------
// Incident-IncidentNotes.
//
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//
namespace NSG.WebSrv.Domain.Entities
{
    //
    /// <summary>
    /// Entity class for intersection of incident and incident notes.
    /// </summary>
    [Table("IncidentIncidentNotes")]
    public class IncidentIncidentNote
    {
        [Required]
        public long IncidentId { get; set; }
        public virtual Incident Incident { get; set; }
        //
        [Required]
        public long IncidentNoteId { get; set; }
        public virtual IncidentNote IncidentNote { get; set; }
    }
}
// ---------------------------------------------------------------------------
