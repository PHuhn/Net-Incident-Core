using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//
using NSG.WebSrv.Application.Commands.Logs;
//
namespace NSG.WebSrv.UI.Controllers
{
    public class LogController : BaseController
    {
        // 
        /// <summary>
        /// GET: Log
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Index()
        {
            string _user = Base_GetUserAccount();
            ViewBag.UserAccount = _user;
            LogListQueryHandler.ViewModel _results = await Mediator.Send(new LogListQueryHandler.ListQuery() { UserAccount = _user });
            return View(_results.LogsList);
        }
    }
}