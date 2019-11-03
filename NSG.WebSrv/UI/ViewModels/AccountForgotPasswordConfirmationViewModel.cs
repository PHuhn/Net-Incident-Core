//
using System;
using System.ComponentModel.DataAnnotations;
//
namespace NSG.WebSrv.UI.ViewModels
{
    //
    public class AccountForgotPasswordConfirmationViewModel
    {
        //
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        //
    }
    //
}
//

