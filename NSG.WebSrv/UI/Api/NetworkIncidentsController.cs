//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly ApplicationDbContext _context;
        //
        public NetworkIncidentsController(ApplicationDbContext context)
        {
            _context = context;
        }
        //
        
        //
        // GET: api/Incidents/5
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

        // PUT: api/Incidents/5
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
        // GET: api/Incidents/GetEmpty/1
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
    }
}
