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
    public class CompanyEmailTemplatesIndexViewModel
    {
        public List<SelectListItem> CompanySelect { get; set; }
        public List<CompanyEmailTemplateListQuery> CompanyEmailTemplates { get; set; }
        public List<IncidentTypeListQuery> IncidentTypes { get; set; }
    }
}