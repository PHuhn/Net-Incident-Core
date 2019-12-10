//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
//
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//
using NSG.WebSrv.Application.Commands.Incidents;
using NSG.WebSrv.Application.Commands.Logs;
//
using NSG.WebSrv.Domain.Entities;
//
namespace NSG.WebSrv.UI.Api
{
    [Authorize]
    [Authorize(Policy = "AnyUserRole")]
    [Route("api/[controller]")]
    [ApiController]
    public class NetworkIncidentsController : BaseApiController
    {
        //
        /// <summary>
        /// NetworkIncidents controller parameterless constructor
        /// All parameters are handled by IMediator from the base BaseApiController;
        /// </summary>
        public NetworkIncidentsController( )
        {
        }
        //
        //  GetIncident(long? id)
        //
        #region "Network Incident get"
        //
        /// <summary>
        /// GET: api/Incidents/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<NetworkIncidentDetailQuery>> GetIncident(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            NetworkIncidentDetailQuery _results =
                await Mediator.Send(new NetworkIncidentDetailQueryHandler.DetailQuery() { IncidentId = id.Value });
            return _results;
        }
        //
        #endregion // Network Incident get
        //
        //  PutIncident(NetworkIncidentUpdateCommand model)
        //
        #region "Network Incident update"
        //
        /// <summary>
        /// PUT: api/Incidents/
        /// </summary>
        /// <param name="model">update model</param>
        /// <returns>same incident</returns>
        [HttpPut("{id}")]
        public async Task<NetworkIncidentDetailQuery> PutIncident(NetworkIncidentUpdateCommand model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    NetworkIncidentDetailQuery _ret = await Mediator.Send(model);
                    return _ret;
                }
                else
                {
                    NetworkIncidentDetailQuery _results =
                        await Mediator.Send(new NetworkIncidentDetailQueryHandler.DetailQuery() { IncidentId = model.IncidentId });
                    _results.Message = string.Join(", ", ModelState.ToArray());
                    return _results;
                }
            }
            catch (Exception _ex)
            {
                await Mediator.Send(new LogCreateCommand(
                    LoggingLevel.Error, MethodBase.GetCurrentMethod(),
                    _ex.Message, _ex));
                NetworkIncidentDetailQuery _results =
                    await Mediator.Send(new NetworkIncidentDetailQueryHandler.DetailQuery() { IncidentId = model.IncidentId });
                _results.Message = _ex.GetBaseException().Message;
                return _results;
            }
        }
        //
        #endregion // Network Incident update
        //
        //  EmptyIncident(int id) // server id
        //  PostIncident(NetworkIncidentCreateCommand model)
        //
        #region "Network Incident Create"
        //
        /// <summary>
        /// GET: api/Incidents/GetEmpty/1
        /// </summary>
        /// <param name="id">server id</param>
        /// <returns></returns>
        [HttpGet("GetEmpty/{id}")]
        public async Task<ActionResult<NetworkIncidentDetailQuery>> EmptyIncident(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            NetworkIncidentDetailQuery _results =
                await Mediator.Send(new NetworkIncidentCreateQueryHandler.DetailQuery() { ServerId = id.Value });
            return _results;
        }
        // POST: api/Incidents
        [HttpPost]
        public async Task<ActionResult<NetworkIncidentDetailQuery>> PostIncident(NetworkIncidentCreateCommand model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    NetworkIncidentDetailQuery _detailIncident = await Mediator.Send(model);
                    return _detailIncident;
                }
            }
            catch (Exception _ex)
            {
                await Mediator.Send(new LogCreateCommand(
                    LoggingLevel.Error, MethodBase.GetCurrentMethod(),
                    _ex.Message, _ex));
            }
            NetworkIncidentDetailQuery _results =
                await Mediator.Send(new NetworkIncidentCreateQueryHandler.DetailQuery() { ServerId = model.ServerId });
            return _results;
        }
        //
        #endregion // Network Incident Create
        //
    }
}
