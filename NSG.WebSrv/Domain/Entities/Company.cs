//
// ---------------------------------------------------------------------------
// Companies.
//
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
// using System.ComponentModel.DataAnnotations.Schema;
//
namespace NSG.WebSrv.Domain.Entities
{
    //
    /// <summary>
    /// Entity class of a company.
    /// </summary>
    public class Company
    {
        //
        [Required(ErrorMessage = "'Company Id' is required.")]
        public int CompanyId { get; set; }
        [Required(ErrorMessage = "'Company Short Name' is required."), MaxLength(12, ErrorMessage = "'CompanyShortName' must be 12 or less characters.")]
        public string CompanyShortName { get; set; }
        [Required(ErrorMessage = "'Company Name' is required."), MaxLength(80, ErrorMessage = "'CompanyName' must be 80 or less characters.")]
        public string CompanyName { get; set; }
        [MaxLength(80, ErrorMessage = "'Address' must be 80 or less characters.")]
        public string Address { get; set; }
        [MaxLength(50, ErrorMessage = "'City' must be 50 or less characters.")]
        public string City { get; set; }
        [MaxLength(4, ErrorMessage = "'State' must be 4 or less characters.")]
        public string State { get; set; }
        [MaxLength(15, ErrorMessage = "'Postal/Zip Code' must be 15 or less characters.")]
        public string PostalCode { get; set; }
        [MaxLength(50, ErrorMessage = "'Country' must be 50 or less characters.")]
        public string Country { get; set; }
        [MaxLength(50, ErrorMessage = "'Phone #' must be 50 or less characters.")]
        public string PhoneNumber { get; set; }
        public string Notes { get; set; }
        //
        public virtual ICollection<Server> Servers { get; } = new List<Server>();
        public virtual ICollection<ApplicationUser> Users { get; } = new List<ApplicationUser>();
        public virtual ICollection<EmailTemplate> EmailTemplates { get; } = new List<EmailTemplate>();
        //
    }
}
// ---------------------------------------------------------------------------
