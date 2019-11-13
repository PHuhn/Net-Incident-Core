using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//
using NSG.WebSrv.Domain.Entities;
using NSG.WebSrv.Application.Commands.Incidents;
using NSG.WebSrv.Application.Commands.Logs;
using System.Reflection;
using NSG.WebSrv.Application.Commands.ApplicationUsers;
//
namespace NSG.WebSrv.UI.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseApiController
    {
        //
        // private readonly ApplicationDbContext _context;
        //
        public UserController()
        {
            // ApplicationDbContext context
            // _context = context;
        }

        // GET: api/Incidents
        /// <summary>
        /// 
        //
        /// <summary>
        /// GET api/<controller>/id=5?serverShortName=nsg
        /// </summary>
        /// <param name="id"></param>
        /// <param name="serverShortName"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApplicationUserServerDetailQuery> GetAsync(string id, string serverShortName)
        {
            ApplicationUserServerDetailQuery _results =
                await Mediator.Send(new ApplicationUserServerDetailQueryHandler.DetailQuery()
                    {  UserName = id, ServerShortName = serverShortName });
            return _results;
        }
        //
    }
}
