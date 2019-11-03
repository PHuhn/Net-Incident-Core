//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
//
namespace NSG.WebSrv.UI.ViewModels
{
    public class RoleViewModel
    {
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Id")]
        public string Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Role Name")]
        public string Name { get; set; }
    }
}
//
