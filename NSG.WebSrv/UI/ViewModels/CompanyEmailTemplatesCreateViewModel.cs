//
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
//
using NSG.WebSrv.Application.Commands.CompanyEmailTemplates;
using NSG.WebSrv.Application.Commands.IncidentTypes;
//
namespace NSG.WebSrv.UI.ViewModels
{
    public class CompanyEmailTemplatesCreateViewModel
    {
        public List<SelectListItem> CompanySelect { get; set; }
        public List<SelectListItem> IncidentTypeSelect { get; set; }
        public List<IncidentTypeListQuery> IncidentTypes { get; set; }
    }
}