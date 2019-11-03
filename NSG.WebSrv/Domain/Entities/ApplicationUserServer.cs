using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//
namespace NSG.WebSrv.Domain.Entities
{
    [Table("ApplicationUserApplicationServer")]
    public class ApplicationUserServer
    {
        [Required, MaxLength(128)]
        public string Id { get; set; }
        public virtual ApplicationUser User { get; set; }
        //
        [Required]
        public int ServerId { get; set; }
        public virtual Server Server { get; set; }
    }
}
//