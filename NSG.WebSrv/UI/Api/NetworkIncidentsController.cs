using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//
using NSG.WebSrv.Domain.Entities;
//
namespace NSG.WebSrv.UI.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class NetworkIncidentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NetworkIncidentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Incidents
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Incident>>> GetIncidents()
        {
            return await _context.Incidents.ToListAsync();
        }

        // GET: api/Incidents/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Incident>> GetIncident(long id)
        {
            var incident = await _context.Incidents.FindAsync(id);

            if (incident == null)
            {
                return NotFound();
            }

            return incident;
        }

        // PUT: api/Incidents/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutIncident(long id, Incident incident)
        {
            if (id != incident.IncidentId)
            {
                return BadRequest();
            }

            _context.Entry(incident).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IncidentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Incidents
        [HttpPost]
        public async Task<ActionResult<Incident>> PostIncident(Incident incident)
        {
            _context.Incidents.Add(incident);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetIncident", new { id = incident.IncidentId }, incident);
        }

        // DELETE: api/Incidents/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Incident>> DeleteIncident(long id)
        {
            var incident = await _context.Incidents.FindAsync(id);
            if (incident == null)
            {
                return NotFound();
            }

            _context.Incidents.Remove(incident);
            await _context.SaveChangesAsync();

            return incident;
        }

        private bool IncidentExists(long id)
        {
            return _context.Incidents.Any(e => e.IncidentId == id);
        }
    }
}
