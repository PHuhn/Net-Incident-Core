//
using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//
using MediatR;
//
using NSG.WebSrv.Application.Commands.ApplicationUsers;
using NSG.WebSrv.Application.Commands.Logs;
using NSG.WebSrv.Domain.Entities;
using NSG.WebSrv.UI.Controllers;
//
namespace NSG.WebSrv.UI.Controllers.CompanyAdmin
{
    [Authorize]
    [Authorize(Policy = "CompanyAdminRole")]
    public class UsersAdminController : BaseController
    {
        public UsersAdminController() : base()
        {
        }
        //
        // -------------------------------------------------------------------
        // Index()
        // Details(int? id)
        //
        #region "Query section"
        //
        /// <summary>
        /// GET: /CompanyUsers/
        /// or
        /// GET: /CompanyUsers/Index
        /// </summary>
        /// <returns>View of a list of permissible users</returns>
        public async Task<IActionResult> Index()
        {
            ApplicationUserListQueryHandler.ViewModel _usersResults =
                await Mediator.Send(new ApplicationUserListQueryHandler.ListQuery());
            return View(_usersResults.ApplicationUsersList);
        }
        //
        /// <summary>
        /// GET: /CompanyUsers/Details/5
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty( id ))
            {
                return NotFound();
            }
            ApplicationUserDetailQuery _results =
                await Mediator.Send(new ApplicationUserDetailQueryHandler.DetailQuery() { UserName = id });
            return View(_results);
        }
        //
        #endregion // Query section
        //
        // -------------------------------------------------------------------
        // Edit(string id)
        // Edit([FromBody]ApplicationUserUpdateCommand model)
        //
        #region "Edit section"
        //
        // GET: NoteTypes/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if ( string.IsNullOrEmpty(id) )
            {
                return NotFound();
            }
            ApplicationUserUpdateQuery _results =
                await Mediator.Send(new ApplicationUserUpdateQueryHandler.EditQuery() { UserName = id });
            return View(_results);
        }

        // POST: NoteTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm]ApplicationUserUpdateCommand model  )
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int ret = await Mediator.Send(model);
                }
                else
                    Base_AddErrors(ModelState);
                return RedirectToAction("Edit", new { id = model.UserName });
            }
            catch (Exception _ex)
            {
                await Mediator.Send(new LogCreateCommand() {
                    Level = LoggingLevel.Error, Method = MethodBase.GetCurrentMethod(),
                    Message = _ex.Message, Exception = _ex });
                Base_AddErrors(_ex);
            }
            return View();
        }
        //
        #endregion // Edit section
        //
        // -------------------------------------------------------------------
        // Delete(string id)
        // DeleteConfirmed(string id)
        //
        #region "Delete section"
        //
        // GET: ApplicationUsers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ApplicationUserDetailQuery _results =
                await Mediator.Send(new ApplicationUserDetailQueryHandler.DetailQuery() { UserName = id });
            return View(_results);
        }

        // POST: ApplicationUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                int _count = await Mediator.Send(new ApplicationUserDeleteCommand() { UserName = id });
                return RedirectToAction("Index");
            }
            catch (Exception _ex)
            {
                await Mediator.Send(new LogCreateCommand()
                {
                    Level = LoggingLevel.Error,
                    Method = MethodBase.GetCurrentMethod(),
                    Message = _ex.Message,
                    Exception = _ex
                });
                Base_AddErrors(_ex);
            }
            return RedirectToAction("Delete", new { id = id });
        }
        //
        #endregion // Delete section
        //
    }
}
//
