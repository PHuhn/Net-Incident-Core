//
// ---------------------------------------------------------------------------
// Incident detail query.
//
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
//
using MediatR;
using FluentValidation;
using FluentValidation.Results;
//
using NSG.WebSrv.Domain.Entities;
using NSG.WebSrv.Infrastructure.Common;
using System.Linq;
//
namespace NSG.WebSrv.Application.Commands.Incidents
{
    //
    /// <summary>
    /// 'Incident' detail query handler.
    /// </summary>
    public class NetworkIncidentDetailQueryHandler : IRequestHandler<NetworkIncidentDetailQueryHandler.DetailQuery, NetworkIncidentDetailQuery>
	{
		private readonly IDb_Context _context;
        private IApplication _application;
        //
        /// <summary>
        ///  The constructor for the inner handler class, to detail the Incident entity.
        /// </summary>
        /// <param name="context">The database interface context.</param>
        public NetworkIncidentDetailQueryHandler(IDb_Context context)
		{
			_context = context;
		}
		//
		/// <summary>
		/// 'Incident' query handle method, passing two interfaces.
		/// </summary>
		/// <param name="request">This detail query request.</param>
		/// <param name="cancellationToken">Cancel token.</param>
		/// <returns>Returns the row count.</returns>
		public async Task<NetworkIncidentDetailQuery> Handle(DetailQuery request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new NetworkIncidentDetailQueryValidationException(_results.FluentValidationErrors());
			}
			var _entity = await GetEntityByKey(request.IncidentId);
			if (_entity == null)
			{
				throw new NetworkIncidentDetailQueryKeyNotFoundException(request.IncidentId);
			}
			//
			// Return the detail query model.
			return CreateDetail(_entity);
		}
		//
		/// <summary>
		/// Get an entity record via the primary key.
		/// </summary>
		/// <param name="incidentId">long key</param>
		/// <returns>Returns a Incident entity record.</returns>
		private Task<Incident> GetEntityByKey(long incidentId)
		{
			return _context.Incidents
                .Include( _i => _i.Server )
                .Include( _i => _i.IncidentIncidentNotes )
                .SingleOrDefaultAsync(r => r.IncidentId == incidentId);
            //.ThenInclude(IncidentIncidentNotes => IncidentIncidentNotes.IncidentNote)
        }
        //
        NetworkIncidentDetailQuery CreateDetail(Incident incident)
        {
            NetworkIncidentDetailQuery _detail = incident.ToNetworkIncidentDetailQuery();
            //
            _detail.Message = "";
            //
            _detail.NetworkLogs = NetworkLogDataList(incident.ServerId, incident.IncidentId, incident.Mailed);
            _detail.DeletedLogs = new List<NetworkLogData>();
            //
            _detail.IncidentNotes = new List<IncidentNoteData>();
            foreach (IncidentIncidentNote _iin in incident.IncidentIncidentNotes)
            {
                _detail.IncidentNotes.Add( _iin.IncidentNote.ToIncidentNoteData() );
            }
            _detail.DeletedNotes= new List<IncidentNoteData>();
            //
            _detail.TypeEmailTemplates = Extensions.GetCompanyIncidentType(_context, incident.Server.CompanyId);
            //
            _detail.NICs = Extensions.GetNICs(_context);
            //
            _detail.IncidentTypes = Extensions.GetIncidentTypes(_context);
            //
            _detail.NoteTypes = Extensions.GetNoteTypes(_context);
            //
            // Return the detail query model.
            return _detail;
        }
        //
        /// <summary>
        /// Get Incident detail query class (the primary key).
        /// </summary>
        public class DetailQuery : IRequest<NetworkIncidentDetailQuery>
		{
			public long IncidentId { get; set; }
		}
        //
        /// <summary>
        /// Return an IQueryable of NetworkLog
        /// </summary>
        /// <returns></returns>
        private IQueryable<NetworkLogData> NetworkLogDataQueryable()
        {
            return
                from _r in _context.NetworkLogs
                select new NetworkLogData()
                {
                    NetworkLogId = _r.NetworkLogId,
                    ServerId = _r.ServerId,
                    IPAddress = _r.IPAddress,
                    NetworkLogDate = _r.NetworkLogDate,
                    Log = _r.Log,
                    IncidentTypeId = _r.IncidentTypeId,
                    IncidentTypeShortDesc = _r.IncidentType.IncidentTypeShortDesc,
                    IncidentId = (_r.IncidentId.HasValue ? _r.IncidentId.Value : 0),
                    Selected = (_r.IncidentId.HasValue ? (_r.IncidentId.Value > 0 ? true : false) : false),
                    IsChanged = false
                };
        }
        //
        /// <summary>
        /// Return a list with all rows of NetworkLog
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="incidentId"></param>
        /// <param name="closed"></param>
        /// <returns></returns>
        public List<NetworkLogData> NetworkLogDataList(int companyId, long incidentId, bool mailed)
        {
            long?[] incidents;
            if (mailed)
                incidents = new long?[] { incidentId };
            else
                incidents = new long?[] { incidentId, (long)0, null };
            List<NetworkLogData> _networkLogs = null;
            _networkLogs = NetworkLogDataQueryable()
                .OrderByDescending(_r => _r.Selected).ThenBy(_r2 => _r2.NetworkLogDate)
                .Where(_r => _r.ServerId == companyId
                   && incidents.Contains(_r.IncidentId)).ToList();
            return _networkLogs;
        }
        //
        /// <summary>
        /// FluentValidation of the 'NetworkIncidentDetailQuery' class.
        /// </summary>
        public class Validator : AbstractValidator<DetailQuery>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'NetworkIncidentDetailQuery' validator.
			/// </summary>
			public Validator()
			{
				//
				RuleFor(x => x.IncidentId).NotNull();
				//
			}
			//
		}
		//
	}
	//
	/// <summary>
	/// Custom NetworkIncidentDetailQuery record not found exception.
	/// </summary>
	public class NetworkIncidentDetailQueryKeyNotFoundException : KeyNotFoundException
	{
		//
		/// <summary>
		/// Implementation of NetworkIncidentDetailQuery record not found exception.
		/// </summary>
		/// <param name="id">The key for the record.</param>
		public NetworkIncidentDetailQueryKeyNotFoundException(long incidentId)
			: base($"NetworkIncidentDetailQuery key not found exception: Id: {incidentId}")
		{
		}
	}
	//
	/// <summary>
	/// Custom NetworkIncidentDetailQuery validation exception.
	/// </summary>
	public class NetworkIncidentDetailQueryValidationException : Exception
	{
		//
		/// <summary>
		/// Implementation of NetworkIncidentDetailQuery validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public NetworkIncidentDetailQueryValidationException(string errorMessage)
			: base($"NetworkIncidentDetailQuery validation exception: errors: {errorMessage}")
		{
		}
	}
	//
}
// ---------------------------------------------------------------------------
