using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//
using NSG.WebSrv.Application.Commands.ApplicationUsers;
//
namespace NSG.WebSrv.UI.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseApiController
    {
        //
        public UserController()
        {
        }
        //
        /// <summary>
        /// GET api/<controller>/id=5?serverShortName=nsg
        /// </summary>
        /// <param name="id">users UserName</param>
        /// <param name="serverShortName">server's short-name</param>
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
//
