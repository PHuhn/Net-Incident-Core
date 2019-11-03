//
// ---------------------------------------------------------------------------
// Incident.
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
    /// Entity class of a network incident referencing a collection of network log incidents.
    /// </summary>
    [Table("Incident")]
    public class Incident
    {
        //
        [Required(ErrorMessage = "IncidentId is required.")]
        public long IncidentId { get; set; }
        [Required(ErrorMessage = "ServerId is required.")]
        public int ServerId { get; set; }
        [Required(ErrorMessage = "IPAddress is required."), MaxLength(50, ErrorMessage = "'IPAddress' must be 50 or less characters.")]
        public string IPAddress { get; set; }
        [Required(ErrorMessage = "NIC_Id is required."), MaxLength(16, ErrorMessage = "'NIC_Id' must be 16 or less characters.")]
        public string NIC_Id { get; set; }
        [MaxLength(255, ErrorMessage = "'NetworkName' must be 255 or less characters.")]
        public string NetworkName { get; set; }
        [MaxLength(255, ErrorMessage = "'AbuseEmailAddress' must be 255 or less characters.")]
        public string AbuseEmailAddress { get; set; }
        [MaxLength(50, ErrorMessage = "'ISPTicketNumber' must be 50 or less characters.")]
        public string ISPTicketNumber { get; set; }
        [Required(ErrorMessage = "Mailed is required.")]
        public bool Mailed { get; set; }
        [Required(ErrorMessage = "Closed is required.")]
        public bool Closed { get; set; }
        [Required(ErrorMessage = "Special is required.")]
        public bool Special { get; set; }
        [MaxLength(1073741823, ErrorMessage = "'Notes' must be 1073741823 or less characters.")]
        public string Notes { get; set; }
        [Required(ErrorMessage = "CreatedDate is required.")]
        public DateTime CreatedDate { get; set; }
        //
        public NIC NIC { get; set; }
        public Server Server { get; set; }
        public virtual ICollection<IncidentIncidentNote> IncidentIncidentNotes { get; }
            = new List<IncidentIncidentNote>();
        //
    }
}
// ---------------------------------------------------------------------------
