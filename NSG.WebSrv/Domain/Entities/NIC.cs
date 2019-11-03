//
// ---------------------------------------------------------------------------
// NICs.
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
    /// Entity class of a Network Information Center.
    /// </summary>
    [Table("NIC")]
    public partial class NIC
    {
        public NIC() { }
        //
        [Required(ErrorMessage = "NIC_Id is required."), MaxLength(16, ErrorMessage = "'NIC_Id' must be 16 or less characters.")]
        public string NIC_Id { get; set; }
        [Required(ErrorMessage = "NICDescription is required."), MaxLength(255, ErrorMessage = "'NICDescription' must be 255 or less characters.")]
        public string NICDescription { get; set; }
        [MaxLength(50, ErrorMessage = "'NICAbuseEmailAddress' must be 50 or less characters.")]
        public string NICAbuseEmailAddress { get; set; }
        [MaxLength(255, ErrorMessage = "'NICRestService' must be 255 or less characters.")]
        public string NICRestService { get; set; }
        [MaxLength(255, ErrorMessage = "'NICWebSite' must be 255 or less characters.")]
        public string NICWebSite { get; set; }
        //
        public virtual ICollection<Incident> Incidents { get; set; }
        //
    }
}
