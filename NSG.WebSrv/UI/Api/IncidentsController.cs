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
//
namespace NSG.WebSrv.UI.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncidentsController : BaseApiController
    {
        //
        /// <summary>
        /// Incidents controller parameterless constructor
        /// All parameters are handled by IMediator from the base BaseApiController;
        /// </summary>
        public IncidentsController()
        {
        }
        //
        //  GetIncidents([FromQuery(Name = "lazyLoadEvent")]string lazyLoadEvent)
        //
        #region"Incident list"
        //
        /// <summary>
        /// GET: api/Incidents
        /// Example:
        /// /api/incidents?lazyLoadEvent={"first":0,"rows":3,"filters":{"ServerId":{"value":1,"matchMode":"equals"},"Mailed":{"value":false,"matchMode":"equals"}}}
        /// </summary>
        /// <param name="lazyLoadEvent"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IncidentListQueryHandler.ViewModel> GetIncidents([FromQuery(Name = "lazyLoadEvent")]string lazyLoadEvent)
        {
            IncidentListQueryHandler.ViewModel _incidentViewModel =
                await Mediator.Send(new IncidentListQueryHandler.ListQuery() { JsonString = lazyLoadEvent });
            return _incidentViewModel;
        }
        //
        #endregion // Incident list
        //
        //  DeleteIncident(long id)
        //
        #region "Incident Delete"
        //
        /// <summary>
        /// DELETE: api/Incidents/5
        /// 
        /// Delete incident and notes and drop link to network-log.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Count of deleted (can be more than 1)</returns>
        [Authorize]
        [Authorize(Policy = "CompanyAdminRole")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<int>> DeleteIncident(long id)
        {
            int _count = 0;
            try
            {
                _count = await Mediator.Send(new IncidentDeleteCommand() { IncidentId = id });
                return RedirectToAction("Index");
            }
            catch (Exception _ex)
            {
                await Mediator.Send(new LogCreateCommand(
                    LoggingLevel.Error, MethodBase.GetCurrentMethod(),
                    _ex.Message, _ex ));
            }
            return _count;
        }
        //
        #endregion // Delete
        //
    }
}
