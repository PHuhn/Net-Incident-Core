using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//
using NSG.WebSrv.Application.Commands.IncidentTypes;
using NSG.WebSrv.Application.Commands.Logs;
using NSG.WebSrv.Domain.Entities;
using NSG.WebSrv.UI.Controllers;
//
namespace NSG.WebSrv.UI.Controllers.Admin
{
    [Authorize]
    [Authorize(Policy = "AdminRole")]
    public class IncidentTypesEmailTemplateController : BaseController
    {
        private readonly ApplicationDbContext _context;
        //
        public IncidentTypesEmailTemplateController(ApplicationDbContext context)
        {
            _context = context;
        }
        //
        // -------------------------------------------------------------------
        // Index()
        // Details(int? id)
        //
        #region "Query section"
        //
        // GET: IncidentTypesEmailTemplate
        public async Task<IActionResult> Index()
        {
            IncidentTypeListQueryHandler.ViewModel _results = await Mediator.Send(new IncidentTypeListQueryHandler.ListQuery());
            return View(_results.IncidentTypesList);
        }
        //
        // GET: IncidentTypesEmailTemplate/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            return View(await Mediator.Send(new IncidentTypeDetailQueryHandler.DetailQuery() { IncidentTypeId = id.Value }));
        }
        //
        #endregion // Query section
        //
        // -------------------------------------------------------------------
        // Create()
        // Create([FromForm]IncidentTypeCreateCommand model)
        //
        #region "Create section"
        //
        // GET: IncidentTypesEmailTemplate/Create
        public async Task<IActionResult> Create()
        {
            IncidentTypeDetailByShortDescQuery _incidentType =
                await Mediator.Send(new IncidentTypeDetailByShortDescQueryHandler.DetailQuery() { IncidentTypeShortDesc = "Multiple" });
            if (_incidentType == null)
            {
                return View();
            }
            var _create = new IncidentTypeCreateCommand()
            {
                IncidentTypeId = 0,
                IncidentTypeShortDesc = "",
                IncidentTypeDesc = "",
                IncidentTypeFromServer = _incidentType.IncidentTypeFromServer,
                IncidentTypeSubjectLine = _incidentType.IncidentTypeSubjectLine,
                IncidentTypeEmailTemplate = _incidentType.IncidentTypeEmailTemplate,
                IncidentTypeTimeTemplate = _incidentType.IncidentTypeTimeTemplate,
                IncidentTypeThanksTemplate = _incidentType.IncidentTypeThanksTemplate,
                IncidentTypeLogTemplate = _incidentType.IncidentTypeLogTemplate,
                IncidentTypeTemplate = _incidentType.IncidentTypeTemplate
            };
            return View(_create);
        }
        //
        // POST: IncidentTypesEmailTemplate/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm]IncidentTypeCreateCommand model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    IncidentType _incidentType = await Mediator.Send(model);
                    return RedirectToAction("Edit", new { id = _incidentType.IncidentTypeId });
                }
                else
                    Base_AddErrors(ModelState);
            }
            catch (Exception _ex)
            {
                await Mediator.Send(new LogCreateCommand(
                    LoggingLevel.Error, MethodBase.GetCurrentMethod(),
                    _ex.Message, _ex));
                Base_AddErrors(_ex);
            }
            return RedirectToAction("Create");
        }
        //
#endregion // Create section
        //
        // -------------------------------------------------------------------
        // Edit(int id)
        // Edit([FromForm]IncidentTypeUpdateCommand model)
        //
        #region "Edit section"
        //
        // GET: IncidentTypesEmailTemplate/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            return View(await Mediator.Send(new IncidentTypeDetailQueryHandler.DetailQuery() { IncidentTypeId = id.Value }));
        }

        // POST: IncidentTypesEmailTemplate/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm]IncidentTypeUpdateCommand model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int ret = await Mediator.Send(model);
                }
                else
                    Base_AddErrors(ModelState);
                return RedirectToAction("Edit", new { id = model.IncidentTypeId });
            }
            catch (Exception _ex)
            {
                await Mediator.Send(new LogCreateCommand(
                    LoggingLevel.Error, MethodBase.GetCurrentMethod(),
                    _ex.Message, _ex));
                Base_AddErrors(_ex);
            }
            return View();
        }
        //
        #endregion // Edit section
        //
        // -------------------------------------------------------------------
        // Delete(int? id)
        // DeleteConfirmed(int id)
        //
        #region "Delete section"
        //
        // GET: IncidentTypesEmailTemplate/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            return View(await Mediator.Send(new IncidentTypeDetailQueryHandler.DetailQuery() { IncidentTypeId = id.Value }));
        }

        // POST: IncidentTypesEmailTemplate/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                int _count = await Mediator.Send(new IncidentTypeDeleteCommand() { IncidentTypeId = id });
                return RedirectToAction("Index");
            }
            catch (Exception _ex)
            {
                await Mediator.Send(new LogCreateCommand(
                    LoggingLevel.Error, MethodBase.GetCurrentMethod(),
                    _ex.Message, _ex));
                Base_AddErrors(_ex);
            }
            return RedirectToAction("Delete", new { id = id });
        }
        //
        #endregion // Delete section
        //
    }
}
