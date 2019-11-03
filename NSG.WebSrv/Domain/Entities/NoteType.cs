//
// ---------------------------------------------------------------------------
// NoteTypes.
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
    /// Entity class of a type of note.
    /// </summary>
    [Table("NoteType")]
    public partial class NoteType
    {
        //
        [Key]
        [Required(ErrorMessage = "'Id' is required.")]
        public int NoteTypeId { get; set; }
        [Required(ErrorMessage = "'Short Desc' is required."), MaxLength(8, ErrorMessage = "'Short Desc' must be 8 or less characters.")]
        public string NoteTypeShortDesc { get; set; }
        [Required(ErrorMessage = "'Description' is required."), MaxLength(50, ErrorMessage = "'Description' must be 50 or less characters.")]
        public string NoteTypeDesc { get; set; }
        [MaxLength(12, ErrorMessage = "'Client Script' must be 12 or less characters.")]
        public string NoteTypeClientScript { get; set; }
        //
        public virtual ICollection<IncidentNote> IncidentNotes { get; set; }
        //
    }
}
// ---------------------------------------------------------------------------
