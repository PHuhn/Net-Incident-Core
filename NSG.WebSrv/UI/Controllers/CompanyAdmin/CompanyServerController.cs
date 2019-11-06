using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
//
using MediatR;
//
using NSG.WebSrv.Application.Commands.Companies;
using NSG.WebSrv.Application.Commands.Servers;
using NSG.WebSrv.Application.Commands.CompanyServers;
using NSG.WebSrv.Application.Commands.Logs;
using NSG.WebSrv.Domain.Entities;
using NSG.WebSrv.UI.Controllers;
//
namespace NSG.WebSrv.UI.Controllers.CompanyAdmin
{
    [Authorize]
    [Authorize(Policy = "CompanyAdminRole")]
    public class CompanyServerController : BaseController
    {
        //
        // GET: CompanyServer
        public async Task<ActionResult> Index()
        {
            CompanyServerListQueryHandler.ViewModel _results = await Mediator.Send(new CompanyServerListQueryHandler.ListQuery());
            return View(_results.CompaniesList);
        }
        //
        // GET: CompanyServer/Details/5
        public async Task<ActionResult> Details(int id)
        {
            CompanyServerDetailQuery _results =
                await Mediator.Send(new CompanyServerDetailQueryHandler.DetailQuery() { CompanyId = id });
            return View(_results);
        }
        //
        // -------------------------------------------------------------------
        // Create()
        // CompanyCreate([FromBody]CompanyCreateCommand model)
        // ServerCreate([FromBody]ServerCreateCommand model)
        //
        #region "Create section"
        //
        /// <summary>
        /// GET: CompanyServer/Create
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View(new CompanyCreateCommand());
        }
        //
        /// <summary>
        /// POST: CompanyServer/CompanyCreate
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CompanyCreate([FromForm]CompanyCreateCommand model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Company _company = await Mediator.Send(model);
                    return RedirectToAction("Edit", new { id = _company.CompanyId });
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
        /// <summary>
        /// POST: CompanyServer/ServerCreate
        /// </summary>
        /// <param name="model">Of type ServerCreateCommand</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> ServerCreate([FromForm]ServerCreateCommand model)
        {
            try
            {
                int _companyId = model.CompanyId;
                if (ModelState.IsValid)
                {
                    if (model.DST && model.DST_Start != null && model.DST_End != null)
                    {
                        Server _server = await Mediator.Send(model);
                    }
                    else
                        Error("DTS requires start/end dates.");
                }
                else
                    Base_AddErrors(ModelState);
                return RedirectToAction("Edit", new { id = _companyId });
            }
            catch (Exception _ex)
            {
                await Mediator.Send(new LogCreateCommand(
                    LoggingLevel.Error, MethodBase.GetCurrentMethod(),
                    _ex.Message, _ex));
                Base_AddErrors(_ex);
            }
            return RedirectToAction("Index");
        }
        //
        #endregion // Create section
        //
        // -------------------------------------------------------------------
        // Edit(int id)
        // CompanyEdit([FromBody]CompanyUpdateCommand model)
        // ServerEdit([FromBody]ServerUpdateCommand model)
        //
        #region "Edit section"
        //
        /// <summary>
        /// GET: Server/Edit/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> Edit(int id)
        {
            CompanyServerDetailQuery _results =
                await Mediator.Send(new CompanyServerDetailQueryHandler.DetailQuery() { CompanyId = id });
            return View(_results);
        }
        //
        /// <summary>
        /// POST: Server/Edit/5
        /// </summary>
        /// <param name="id"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CompanyEdit([FromForm]CompanyUpdateCommand model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int ret = await Mediator.Send(model);
                }
                else
                    Base_AddErrors(ModelState);
                return RedirectToAction("Edit", new { id = model.CompanyId });
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
        /// <summary>
        /// POST: CompanyServer/ServerEdit/5
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ServerEdit([FromForm]ServerUpdateCommand model)
        {
            try
            {
                int _companyId = model.CompanyId;
                if (ModelState.IsValid)
                {
                    if (model.DST && model.DST_Start != null && model.DST_End != null)
                    {
                        int ret = await Mediator.Send(model);
                    }
                    else
                        Error("DTS requires start/end dates.");
                }
                else
                    Base_AddErrors(ModelState);
                return RedirectToAction("Edit", new { id = _companyId });
            }
            catch (Exception _ex)
            {
                await Mediator.Send(new LogCreateCommand(
                    LoggingLevel.Error, MethodBase.GetCurrentMethod(),
                    _ex.Message, _ex));
                Base_AddErrors(_ex);
            }
            return RedirectToAction("Index");
        }
        //
        #endregion // Edit
        //
        // -------------------------------------------------------------------
        // Delete(int id)
        // CompanyDelete(int id, CompanyServerDetailQuery model)
        // ServerDelete(int id, ServerDetailQuery model)
        //
        #region "Delete section"
        // GET: CompanyServer/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            CompanyServerDetailQuery _results =
                await Mediator.Send(new CompanyServerDetailQueryHandler.DetailQuery() { CompanyId = id });
            return View(_results);
        }
        //
        /// <summary>
        /// POST: Server/CompanyDelete/5
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CompanyDelete(int id, CompanyServerDetailQuery model)
        {
            try
            {
                int _count = await Mediator.Send(new CompanyDeleteCommand() { CompanyId = id });
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
        /// <summary>
        /// POST: Server/ServerDelete/5
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ServerDelete(int id, ServerDetailQuery model)
        {
            int _companyId = model.CompanyId;
            try
            {
                int _count = await Mediator.Send(new ServerDeleteCommand() { ServerId = id });
                return RedirectToAction("Delete", new { id = _companyId });
            }
            catch (Exception _ex)
            {
                await Mediator.Send(new LogCreateCommand(
                    LoggingLevel.Error, MethodBase.GetCurrentMethod(),
                    _ex.Message, _ex));
                Base_AddErrors(_ex);
            }
            return RedirectToAction("Delete", new { id = _companyId });
        }
        //
        #endregion // Delete section
        //
    }
}