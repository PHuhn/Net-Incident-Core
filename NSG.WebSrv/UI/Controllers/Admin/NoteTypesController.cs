//
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
//
using MediatR;
//
using NSG.WebSrv.Application.Commands.NoteTypes;
using NSG.WebSrv.Application.Commands.Logs;
using NSG.WebSrv.Domain.Entities;
using NSG.WebSrv.UI.Controllers;
//
namespace NSG.WebSrv.UI.Controllers.Admin
{
    [Authorize]
    [Authorize(Policy = "AdminRole")]
    public class NoteTypesController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public NoteTypesController(ApplicationDbContext context)
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
        /// <summary>
        /// GET: NoteTypes
        /// </summary>
        /// <returns>action result view</returns>
        public async Task<ActionResult> Index()
        {
            NoteTypeListQueryHandler.ViewModel _results = await Mediator.Send(new NoteTypeListQueryHandler.ListQuery());
            return View(_results.NoteTypesList);
        }
        //
        /// <summary>
        /// GET: NoteTypes/Details/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            NoteTypeDetailQuery _results =
                await Mediator.Send(new NoteTypeDetailQueryHandler.DetailQuery() { NoteTypeId = id.Value });
            return View(_results);
        }
        //
        #endregion // Query section
        //
        // -------------------------------------------------------------------
        // Create()
        // Create([FromForm]NoteTypeCreateCommand model)
        //
        #region "Create section"
        //
        // GET: NoteTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: NoteTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm]NoteTypeCreateCommand model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    NoteType _noteType = await Mediator.Send(model);
                    return RedirectToAction("Edit", new { id = _noteType.NoteTypeId });
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
        // Edit([FromBody]CompanyUpdateCommand model)
        //
        #region "Edit section"
        //
        // GET: NoteTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            NoteTypeDetailQuery _results =
                await Mediator.Send(new NoteTypeDetailQueryHandler.DetailQuery() { NoteTypeId = id.Value });
            return View(_results);
        }

        // POST: NoteTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm]NoteTypeUpdateCommand model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int ret = await Mediator.Send(model);
                }
                else
                    Base_AddErrors(ModelState);
                return RedirectToAction("Edit", new { id = model.NoteTypeId });
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
        // GET: NoteTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            NoteTypeDetailQuery _results =
                await Mediator.Send(new NoteTypeDetailQueryHandler.DetailQuery() { NoteTypeId = id.Value });
            return View(_results);
        }

        // POST: NoteTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                int _count = await Mediator.Send(new NoteTypeDeleteCommand() { NoteTypeId = id });
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
