//
using System;
using System.ComponentModel.DataAnnotations;
//
namespace NSG.WebSrv.UI.ViewModels
{
    public class AccountLoginViewModel
    {
        //
        [Required]
        [MaxLength(256)]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        //
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        //
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
        //
    }
}
//
