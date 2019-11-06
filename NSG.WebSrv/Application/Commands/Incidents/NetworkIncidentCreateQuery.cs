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
//
namespace NSG.WebSrv.Application.Commands.Incidents
{
	//
	/// <summary>
	/// 'Incident' detail query handler.
	/// </summary>
	public class NetworkIncidentCreateQueryHandler : IRequestHandler<NetworkIncidentCreateQueryHandler.DetailQuery, NetworkIncidentDetailQuery>
	{
		private readonly IDb_Context _context;
        private IApplication _application;
        //
        /// <summary>
        ///  The constructor for the inner handler class, to detail the Incident entity.
        /// </summary>
        /// <param name="context">The database interface context.</param>
        public NetworkIncidentCreateQueryHandler(IDb_Context context)
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
				throw new NetworkIncidentCreateQueryValidationException(_results.FluentValidationErrors());
			}
			Server _server = await Extensions.GetServerByKey(_context, request.ServerId);
			if (_server == null)
			{
				throw new NetworkIncidentCreateQueryKeyNotFoundException(request.ServerId);
			}
            int _companyId = _server.CompanyId;
            //
            // Return the detail query model.
            return CreateEmpty(_server);
		}
        //
        /// <summary>
        /// Create an empty unit-of-work NetworkIncidentDetailQuery,
        /// with EmailTemplates, NICs, IncidentTypes, NoteTypes populated.
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="serverId"></param>
        /// <returns></returns>
        NetworkIncidentDetailQuery CreateEmpty(Server server)
        {
            NetworkIncidentDetailQuery _detail = new NetworkIncidentDetailQuery();
            _detail.IncidentId = 0;
            _detail.ServerId = server.ServerId;
            _detail.IPAddress = "";
            _detail.NIC_Id = "";
            _detail.NetworkName = "";
            _detail.AbuseEmailAddress = "";
            _detail.ISPTicketNumber = "";
            _detail.Mailed = false;
            _detail.Closed = false;
            _detail.Special = false;
            _detail.Notes = "";
            //
            _detail.Message = "";
            //
            _detail.NetworkLogs = new List<NetworkLogData>();
            _detail.DeletedLogs = new List<NetworkLogData>();
            //
            _detail.IncidentNotes = new List<IncidentNoteData>();
            _detail.DeletedNotes = new List<IncidentNoteData>();
            //
            _detail.TypeEmailTemplates = Extensions.GetCompanyIncidentType(_context, server.CompanyId);
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
			public int ServerId { get; set; }
		}
		//
		/// <summary>
		/// FluentValidation of the 'NetworkIncidentCreateQuery' class.
		/// </summary>
		public class Validator : AbstractValidator<DetailQuery>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'NetworkIncidentCreateQuery' validator.
			/// </summary>
			public Validator()
			{
				//
				RuleFor(x => x.ServerId).NotNull();
				//
			}
			//
		}
		//
	}
	//
	/// <summary>
	/// Custom NetworkIncidentCreateQuery record not found exception.
	/// </summary>
	public class NetworkIncidentCreateQueryKeyNotFoundException : KeyNotFoundException
	{
		//
		/// <summary>
		/// Implementation of NetworkIncidentCreateQuery record not found exception.
		/// </summary>
		/// <param name="id">The key for the record.</param>
		public NetworkIncidentCreateQueryKeyNotFoundException(long incidentId)
			: base($"NetworkIncidentCreateQuery key not found exception: Id: {incidentId}")
		{
		}
	}
	//
	/// <summary>
	/// Custom NetworkIncidentCreateQuery validation exception.
	/// </summary>
	public class NetworkIncidentCreateQueryValidationException : Exception
	{
		//
		/// <summary>
		/// Implementation of NetworkIncidentCreateQuery validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public NetworkIncidentCreateQueryValidationException(string errorMessage)
			: base($"NetworkIncidentCreateQuery validation exception: errors: {errorMessage}")
		{
		}
	}
	//
}
// ---------------------------------------------------------------------------
