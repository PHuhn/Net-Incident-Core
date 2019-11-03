//
using System;
using System.ComponentModel.DataAnnotations;
//
namespace NSG.WebSrv.UI.ViewModels
{
    //
    public class AccountForgotPasswordViewModel
    {
        //
        [Required]
        [MaxLength(256)]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        //
    }
    //
}
//

