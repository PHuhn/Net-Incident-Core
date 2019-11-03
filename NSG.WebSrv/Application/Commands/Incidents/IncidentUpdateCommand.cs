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
	public class IncidentUpdateCommand : IRequest<int>
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
		public DateTime CreatedDate { get; set; }
	}
	//
	/// <summary>
	/// 'Incident' update command handler.
	/// </summary>
	public class IncidentUpdateCommandHandler : IRequestHandler<IncidentUpdateCommand, int>
	{
		private readonly IDb_Context _context;
        private IApplication _application;
        //
        /// <summary>
        ///  The constructor for the inner handler class, to update the Incident entity.
        /// </summary>
        /// <param name="context">The database interface context.</param>
        public IncidentUpdateCommandHandler(IDb_Context context)
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
		public async Task<int> Handle(IncidentUpdateCommand request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new UpdateCommandValidationException(_results.FluentValidationErrors());
			}
			var _entity = await _context.Incidents
				.SingleOrDefaultAsync(r => r.IncidentId == request.IncidentId, cancellationToken);
			if (_entity == null)
			{
				throw new UpdateCommandKeyNotFoundException(request.IncidentId);
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
			_entity.CreatedDate = request.CreatedDate;
			//
			await _context.SaveChangesAsync(cancellationToken);
			// Return the row count.
			return 1;
		}
		//
		/// <summary>
		/// FluentValidation of the 'IncidentUpdateCommand' class.
		/// </summary>
		public class Validator : AbstractValidator<IncidentUpdateCommand>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'IncidentUpdateCommand' validator.
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
				RuleFor(x => x.CreatedDate).NotNull();
				//
			}
			//
		}
		//
	}
	//
	/// <summary>
	/// Custom IncidentUpdateCommand record not found exception.
	/// </summary>
	public class UpdateCommandKeyNotFoundException: KeyNotFoundException
	{
		//
		/// <summary>
		/// Implementation of IncidentUpdateCommand record not found exception.
		/// </summary>
		/// <param name="id">The key for the record.</param>
		public UpdateCommandKeyNotFoundException(long incidentId)
			: base($"IncidentUpdateCommand key not found exception: id: {incidentId}")
		{
		}
	}
	//
	/// <summary>
	/// Custom IncidentUpdateCommand validation exception.
	/// </summary>
	public class UpdateCommandValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of IncidentUpdateCommand validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public UpdateCommandValidationException(string errorMessage)
			: base($"IncidentUpdateCommand validation exception: errors: {errorMessage}")
		{
		}
	}
}
// ---------------------------------------------------------------------------

