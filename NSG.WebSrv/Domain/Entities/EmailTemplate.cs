//
// ---------------------------------------------------------------------------
// EmailTemplate insert duplicate validation.
//
using System;
using System.ComponentModel.DataAnnotations;
//
namespace NSG.WebSrv.Domain.Entities
{
    //
    /// <summary>
    /// Entity class of a company specific e-mail templates.
    /// </summary>
    public class EmailTemplate
    {
        //
        [Required(ErrorMessage = "CompanyId is required.")]
        public int CompanyId { get; set; }
        [Required(ErrorMessage = "IncidentTypeId is required.")]
        public int IncidentTypeId { get; set; }
        [Required(ErrorMessage = "SubjectLine is required."), MaxLength(1073741823, ErrorMessage = "'SubjectLine' must be 1073741823 or less characters.")]
        public string SubjectLine { get; set; }
        [Required(ErrorMessage = "EmailBody is required."), MaxLength(1073741823, ErrorMessage = "'EmailBody' must be 1073741823 or less characters.")]
        public string EmailBody { get; set; }
        [Required(ErrorMessage = "TimeTemplate is required."), MaxLength(1073741823, ErrorMessage = "'TimeTemplate' must be 1073741823 or less characters.")]
        public string TimeTemplate { get; set; }
        [Required(ErrorMessage = "ThanksTemplate is required."), MaxLength(1073741823, ErrorMessage = "'ThanksTemplate' must be 1073741823 or less characters.")]
        public string ThanksTemplate { get; set; }
        [Required(ErrorMessage = "LogTemplate is required."), MaxLength(1073741823, ErrorMessage = "'LogTemplate' must be 1073741823 or less characters.")]
        public string LogTemplate { get; set; }
        [Required(ErrorMessage = "Template is required."), MaxLength(1073741823, ErrorMessage = "'Template' must be 1073741823 or less characters.")]
        public string Template { get; set; }
        [Required(ErrorMessage = "FromServer is required.")]
        public bool FromServer { get; set; }
        //
        public Company Company { get; set; }
        public IncidentType IncidentType { get; set; }
        //
    }
}
// ---------------------------------------------------------------------------
