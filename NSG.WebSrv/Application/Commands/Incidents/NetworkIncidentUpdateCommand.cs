//
// ---------------------------------------------------------------------------
// Incident update command.
//
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
//
using Microsoft.EntityFrameworkCore;
using MediatR;
using FluentValidation;
using FluentValidation.Results;
using NSG.WebSrv.Domain.Entities;
using NSG.WebSrv.Infrastructure.Common;
//
namespace NSG.WebSrv.Application.Commands.Incidents
{
	//
	/// <summary>
	/// 'Incident' update command, handler and handle.
	/// </summary>
	public class NetworkIncidentUpdateCommand : IRequest<int>
	{
		public long IncidentId { get; set; }
		public int ServerId { get; set; }
		public string IPAddress { get; set; }
		public string NIC_Id { get; set; }
		public string NetworkName { get; set; }
		public string AbuseEmailAddress { get; set; }
		public string ISPTicketNumber { get; set; }
		public bool Mailed { get; set; }
		public bool Closed { get; set; }
		public bool Special { get; set; }
		public string Notes { get; set; }
        //
        public string message;
        //
        public List<IncidentNoteData> incidentNotes;
        public List<IncidentNoteData> deletedNotes;
        //
        public List<NetworkLogData> networkLogs;
        public List<NetworkLogData> deletedLogs;
        //
        public UserServerData user;
        //
    }
    //
    /// <summary>
    /// 'Incident' update command handler.
    /// </summary>
    public class NetworkIncidentUpdateCommandHandler : IRequestHandler<NetworkIncidentUpdateCommand, int>
	{
		private readonly IDb_Context _context;
        private IApplication _application;
        //
        /// <summary>
        ///  The constructor for the inner handler class, to update the Incident entity.
        /// </summary>
        /// <param name="context">The database interface context.</param>
        public NetworkIncidentUpdateCommandHandler(IDb_Context context)
		{
			_context = context;
		}
		//
		/// <summary>
		/// 'Incident' command handle method, passing two interfaces.
		/// </summary>
		/// <param name="request">This update command request.</param>
		/// <param name="cancellationToken">Cancel token.</param>
		/// <returns>Returns the row count.</returns>
		public async Task<int> Handle(NetworkIncidentUpdateCommand request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new NetworkIncidentUpdateCommandValidationException(_results.FluentValidationErrors());
			}
			var _entity = await _context.Incidents
				.SingleOrDefaultAsync(r => r.IncidentId == request.IncidentId, cancellationToken);
			if (_entity == null)
			{
				throw new NetworkIncidentUpdateCommandKeyNotFoundException(request.IncidentId);
			}
			// Move from update command class to entity class.
			_entity.ServerId = request.ServerId;
			_entity.IPAddress = request.IPAddress;
			_entity.NIC_Id = request.NIC_Id;
			_entity.NetworkName = request.NetworkName;
			_entity.AbuseEmailAddress = request.AbuseEmailAddress;
			_entity.ISPTicketNumber = request.ISPTicketNumber;
			_entity.Mailed = request.Mailed;
			_entity.Closed = request.Closed;
			_entity.Special = request.Special;
			_entity.Notes = request.Notes;
			//
			await _context.SaveChangesAsync(cancellationToken);
			// Return the row count.
			return 1;
		}
		//
		/// <summary>
		/// FluentValidation of the 'NetworkIncidentUpdateCommand' class.
		/// </summary>
		public class Validator : AbstractValidator<NetworkIncidentUpdateCommand>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'NetworkIncidentUpdateCommand' validator.
			/// </summary>
			public Validator()
			{
				//
				RuleFor(x => x.IncidentId).NotNull();
				RuleFor(x => x.ServerId).NotNull();
				RuleFor(x => x.IPAddress).NotEmpty().MaximumLength(50);
				RuleFor(x => x.NIC_Id).NotEmpty().MaximumLength(16);
				RuleFor(x => x.NetworkName).MaximumLength(255);
				RuleFor(x => x.AbuseEmailAddress).MaximumLength(255);
				RuleFor(x => x.ISPTicketNumber).MaximumLength(50);
				RuleFor(x => x.Mailed).NotNull();
				RuleFor(x => x.Closed).NotNull();
				RuleFor(x => x.Special).NotNull();
				RuleFor(x => x.Notes).MaximumLength(1073741823);
				//
			}
			//
		}
		//
	}
	//
	/// <summary>
	/// Custom NetworkIncidentUpdateCommand record not found exception.
	/// </summary>
	public class NetworkIncidentUpdateCommandKeyNotFoundException : KeyNotFoundException
	{
		//
		/// <summary>
		/// Implementation of NetworkIncidentUpdateCommand record not found exception.
		/// </summary>
		/// <param name="id">The key for the record.</param>
		public NetworkIncidentUpdateCommandKeyNotFoundException(long incidentId)
			: base($"NetworkIncidentUpdateCommand key not found exception: id: {incidentId}")
		{
		}
	}
	//
	/// <summary>
	/// Custom NetworkIncidentUpdateCommand validation exception.
	/// </summary>
	public class NetworkIncidentUpdateCommandValidationException : Exception
	{
		//
		/// <summary>
		/// Implementation of NetworkIncidentUpdateCommand validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public NetworkIncidentUpdateCommandValidationException(string errorMessage)
			: base($"NetworkIncidentUpdateCommand validation exception: errors: {errorMessage}")
		{
		}
	}
}
// ---------------------------------------------------------------------------

