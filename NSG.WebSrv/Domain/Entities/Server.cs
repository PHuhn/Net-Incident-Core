//
// ---------------------------------------------------------------------------
// Servers.
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
    /// Entity class of a server.
    /// </summary>
    [Table("Servers")]
    public class Server : IServer
    {
        //
        [Required(ErrorMessage = "'Server Id' is required.")]
        public int ServerId { get; set; }
        [Required(ErrorMessage = "'Company Id' is required.")]
        public int CompanyId { get; set; }      // FK of the company that manages the server.
        [Required(ErrorMessage = "'Server Short Name' is required."),
            MinLength(6, ErrorMessage = "'Server Short Name' must be 6 or up to 12 characters."),
            MaxLength(12, ErrorMessage = "'Server Short Name' must be 12 or less characters.")]
        public string ServerShortName { get; set; }  // for login
        [Required(ErrorMessage = "'Server Name' is required."), MaxLength(80, ErrorMessage = "'Server Name' must be 80 or less characters.")]
        public string ServerName { get; set; }      // internal 
        [Required(ErrorMessage = "'Server Description' is required."), MaxLength(255, ErrorMessage = "'Server Description' must be 255 or less characters.")]
        public string ServerDescription { get; set; } // line as appears on report to ISP
        [Required(ErrorMessage = "'Device' is required."), MaxLength(255, ErrorMessage = "'Device' must be 255 or less characters.")]
        public string WebSite { get; set; }  // line as appears on report to ISP
        [Required(ErrorMessage = "'Server Location' is required."), MaxLength(255, ErrorMessage = "'Server Location' must be 255 or less characters.")]
        public string ServerLocation { get; set; }  // line as appears on report to ISP
        [Required(ErrorMessage = "'From Name' is required."), MaxLength(255, ErrorMessage = "'From Name' must be 255 or less characters.")]
        public string FromName { get; set; }        // item as appears on report to ISP
        [Required(ErrorMessage = "'From Nic Name' is required."), MaxLength(16, ErrorMessage = "'From Nic Name' must be 16 or less characters.")]
        public string FromNicName { get; set; }     // item as appears on report to ISP
        [Required(ErrorMessage = "'From Email Address' is required."), MaxLength(255, ErrorMessage = "'From Email Address' must be 255 or less characters.")]
        public string FromEmailAddress { get; set; }    // item as appears on report to ISP
        [Required(ErrorMessage = "'Time-Zone' is required."), MaxLength(16, ErrorMessage = "'Time-Zone' must be 16 or less characters.")]
        public string TimeZone { get; set; }        // item as appears on report to ISP
        [Required(ErrorMessage = "'DST' is required.")]
        public bool DST { get; set; }           // flag used for report to ISP
        [MaxLength(16, ErrorMessage = "'Time-Zone DST' must be 16 or less characters.")]
        public string TimeZone_DST { get; set; }    // item that may appears on report to ISP
        [DataType(DataType.DateTime)]
        public DateTime? DST_Start { get; set; }    // value used for report to ISP
        [DataType(DataType.DateTime)]
        public DateTime? DST_End { get; set; }      // value used for report to ISP
        //
        public virtual Company Company { get; set; }
        //
        public virtual ICollection<ApplicationUserServer> UserServers { get; }
            = new List<ApplicationUserServer>();
        public virtual ICollection<NetworkLog> NetworkLogs { get; }
            = new List<NetworkLog>();
        public virtual ICollection<Incident> Incidents { get; }
            = new List<Incident>();
        //
    }
}
// ---------------------------------------------------------------------------
