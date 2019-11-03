//
// ---------------------------------------------------------------------------
// IncidentNote.
//
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
//
namespace NSG.WebSrv.Domain.Entities
{
    //
    /// <summary>
    /// Entity class of a network incident note.
    /// This will include communications to and from the ISP.
    /// </summary>
    [Table("IncidentNote")]
    public partial class IncidentNote
    {
        //
        public IncidentNote() { }
        //
        [Required(ErrorMessage = "IncidentNoteId is required.")]
        public long IncidentNoteId { get; set; }
        [Required(ErrorMessage = "NoteTypeId is required.")]
        public int NoteTypeId { get; set; }
        [Required(ErrorMessage = "Note is required."), MaxLength(1073741823, ErrorMessage = "'Note' must be 1073741823 or less characters.")]
        public string Note { get; set; }
        public DateTime CreatedDate { get; set; }
        //
        public virtual NoteType NoteType { get; set; }
        public virtual ICollection<IncidentIncidentNote> IncidentIncidentNotes { get; }
            = new List<IncidentIncidentNote>();
        //
        //
        /// <summary>
        /// Create a 'to string'.
        /// </summary>
        public override string ToString()
        {
            //
            StringBuilder _return = new StringBuilder("record:[");
            _return.AppendFormat("IncidentNoteId: {0}, ", IncidentNoteId.ToString());
            _return.AppendFormat("NoteTypeId: {0}, ", NoteTypeId.ToString());
            _return.AppendFormat("Note: {0}, ", Note);
            _return.AppendFormat("CreatedDate: {0}]", CreatedDate.ToString());
            return _return.ToString();
        }
    }
}
// ---------------------------------------------------------------------------
