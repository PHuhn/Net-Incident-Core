using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
//
using MediatR;
//
using NSG.WebSrv.Domain.Entities;
using NSG.WebSrv.Application.Commands.CompanyEmailTemplates;
using NSG.WebSrv.Application.Commands.Logs;
using NSG.WebSrv.Application.Commands.Companies;
using NSG.WebSrv.UI.ViewModels;
using NSG.WebSrv.Application.Commands.IncidentTypes;
using NSG.WebSrv.UI.Controllers;
//
namespace NSG.WebSrv.UI.Controllers.CompanyAdmin
{
    [Authorize]
    [Authorize(Policy = "CompanyAdminRole")]
    public class CompanyEmailTemplatesController : BaseController
    {
        //
        public CompanyEmailTemplatesController()
        {
            //
        }
        //
        // -------------------------------------------------------------------
        // Index(int? companyId)
        // Details(int? companyId, int? incidentTypeId)
        //
        #region "Query section"
        //
        // GET: CompanyEmailTemplates
        public async Task<IActionResult> Index(int? companyId)
        {
            UserCompanySelectionListQueryHandler.ViewModel _companyResults =
                await Mediator.Send(new UserCompanySelectionListQueryHandler.ListQuery() { UserName = this.User.Identity.Name });
            if( companyId.HasValue == false)
            {
                string _companyIdValue = _companyResults.CompanyList.Where(_cmp => _cmp.Selected == true).FirstOrDefault().Value;
                if (!string.IsNullOrEmpty(_companyIdValue))
                    companyId = Convert.ToInt32(_companyIdValue);
                else
                    companyId = 0;
            }
            else
            {
                foreach( var _sl in _companyResults.CompanyList)
                {
                    _sl.Selected = (_sl.Value == companyId.ToString() ? true : false);
                }
            }
            CompanyEmailTemplateListQueryHandler.ViewModel _templateResults =
                await Mediator.Send(new CompanyEmailTemplateListQueryHandler.ListQuery() { CompanyId = companyId.Value });
            IncidentTypeListQueryHandler.ViewModel _results = await Mediator.Send(new IncidentTypeListQueryHandler.ListQuery());
            return View(new CompanyEmailTemplatesIndexViewModel()
            {
                CompanySelect = _companyResults.CompanyList,
                CompanyEmailTemplates = _templateResults.EmailTemplatesList,
                IncidentTypes = _results.IncidentTypesList.ToList()
            });
        }
        //
        // GET: CompanyEmailTemplates/Details/5
        public async Task<IActionResult> Details(int? companyId, int? incidentTypeId)
        {
            if (!companyId.HasValue || !incidentTypeId.HasValue)
            {
                return NotFound();
            }
            return View(await Mediator.Send(new CompanyEmailTemplateDetailQueryHandler.DetailQuery() { CompanyId = companyId.Value, IncidentTypeId = incidentTypeId.Value }));
        }
        //
        #endregion // Query section
        //
        // -------------------------------------------------------------------
        // Create(int? companyId)
        // Create([FromForm]CompanyEmailTemplateCreateCommand model)
        // CreateNewCreateViewModel(CompanyEmailTemplateCreateCommand model)
        //
        #region "Create section"
        //
        // GET: CompanyEmailTemplates/Create
        public async Task<IActionResult> Create(int? companyId)
        {
            if (companyId.HasValue == false)
            {
                return BadRequest();
            }
            //
            ViewBag.Support = await CreateNewCreateViewModel(companyId.Value);
            return View(
                new CompanyEmailTemplateCreateCommand() { CompanyId = companyId.Value });
            //
        }
        //
        // POST: CompanyEmailTemplates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm]CompanyEmailTemplateCreateCommand model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    EmailTemplate _emailTemplate = await Mediator.Send(model);
                    return RedirectToAction("Edit",
                        new { companyId = _emailTemplate.CompanyId, incidentTypeId = _emailTemplate.IncidentTypeId });
                }
                else
                    Base_AddErrors(ModelState);
            }
            catch (Exception _ex)
            {
                await Mediator.Send(new LogCreateCommand() {
                    Level = LoggingLevel.Error, Method = MethodBase.GetCurrentMethod(),
                    Message = _ex.Message, Exception = _ex });
                Base_AddErrors(_ex);
            }
            ViewBag.Support = await CreateNewCreateViewModel(model.CompanyId);
            return View(model);
        }
        //
        /// <summary>
        /// construct the 'create' view model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task<CompanyEmailTemplatesCreateViewModel> CreateNewCreateViewModel(int companyId)
        {
            UserCompanySelectionListQueryHandler.ViewModel _companyResults =
                await Mediator.Send(new UserCompanySelectionListQueryHandler.ListQuery() { UserName = this.User.Identity.Name });
            IncidentTypeSelectionListQueryHandler.ViewModel _incidentTypeResults =
                await Mediator.Send(new IncidentTypeSelectionListQueryHandler.ListQuery());
            CompanyEmailTemplateSelectionListQueryHandler.ViewModel _companyEmailTemplatesResults =
                await Mediator.Send(new CompanyEmailTemplateSelectionListQueryHandler.ListQuery() { CompanyId = companyId });
            if(_companyEmailTemplatesResults.EmailTemplatesList.Count > 0)
            {
                // remove already created company email templates.
                string[] _valToRemove = _companyEmailTemplatesResults
                    .EmailTemplatesList.Select(et => et.Value).ToArray();
                _incidentTypeResults.IncidentTypesList.RemoveAll(
                    itet => _valToRemove.Contains(itet.Value));
            }
            IncidentTypeListQueryHandler.ViewModel _results = await Mediator.Send(new IncidentTypeListQueryHandler.ListQuery());
            var _viewModel = new CompanyEmailTemplatesCreateViewModel()
            {
                CompanySelect = _companyResults.CompanyList,
                IncidentTypeSelect = _incidentTypeResults.IncidentTypesList,
                IncidentTypes = _results.IncidentTypesList.ToList(),
            };
            return _viewModel;
        }
        //
        #endregion // Create section
        //
        // -------------------------------------------------------------------
        // Edit(int? companyId, int? incidentTypeId)
        // Edit([FromForm]CompanyEmailTemplateUpdateCommand model)
        //
        #region "Edit section"
        //
        // GET: CompanyEmailTemplates/Edit/5
        public async Task<IActionResult> Edit(int? companyId, int? incidentTypeId)
        {
            if (!companyId.HasValue || !incidentTypeId.HasValue)
            {
                return NotFound();
            }
            CompanyEmailTemplateDetailQuery _model = await Mediator.Send(new CompanyEmailTemplateDetailQueryHandler.DetailQuery() { CompanyId = companyId.Value, IncidentTypeId = incidentTypeId.Value });
            ViewBag.DetailQuery = _model;
            return View( _model.ToCompanyEmailTemplateUpdateCommand() );
        }

        // POST: CompanyEmailTemplates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] CompanyEmailTemplateUpdateCommand model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int ret = await Mediator.Send(model);
                    return RedirectToAction("Index", new { companyId = model.CompanyId });
                }
                else
                    Base_AddErrors(ModelState);
            }
            catch (Exception _ex)
            {
                await Mediator.Send(new LogCreateCommand() {
                    Level = LoggingLevel.Error, Method = MethodBase.GetCurrentMethod(),
                    Message = _ex.Message, Exception = _ex });
                Base_AddErrors(_ex);
            }
            CompanyEmailTemplateDetailQuery _model = await Mediator.Send(new CompanyEmailTemplateDetailQueryHandler.DetailQuery() { CompanyId = model.CompanyId, IncidentTypeId = model.IncidentTypeId });
            ViewBag.DetailQuery = _model;
            return View(model);
        }
        //
        #endregion // Edit section
        //
        // GET: CompanyEmailTemplates/Delete?companyId=1&incidentTypeId=3
        public async Task<IActionResult> Delete(int? companyId, int? incidentTypeId)
        {
            if (!companyId.HasValue || !incidentTypeId.HasValue)
            {
                return NotFound();
            }
            CompanyEmailTemplateDetailQuery _results =
                await Mediator.Send(new CompanyEmailTemplateDetailQueryHandler.DetailQuery() { CompanyId = companyId.Value, IncidentTypeId = incidentTypeId.Value });
            return View(_results);
        }

        // POST: CompanyEmailTemplates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? companyId, int? incidentTypeId)
        {
            if (!companyId.HasValue || !incidentTypeId.HasValue)
            {
                return NotFound();
            }
            try
            {
                int _count = await Mediator.Send(new CompanyEmailTemplateDeleteCommand() { CompanyId = companyId.Value, IncidentTypeId = incidentTypeId.Value });
                return RedirectToAction("Index", new { companyId = companyId.Value });
            }
            catch (Exception _ex)
            {
                await Mediator.Send(new LogCreateCommand() {
                    Level = LoggingLevel.Error, Method = MethodBase.GetCurrentMethod(),
                    Message = _ex.Message, Exception = _ex });
                Base_AddErrors(_ex);
            }
            return RedirectToAction("Delete", new { companyId = companyId.Value, incidentTypeId = incidentTypeId.Value });
        }
        //
    }
}
