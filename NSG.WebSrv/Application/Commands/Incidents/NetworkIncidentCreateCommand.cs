//
// ---------------------------------------------------------------------------
// Incident create command.
//
using System;
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
	/// 'Incident' create command, handler and handle.
	/// </summary>
	public class NetworkIncidentCreateCommand : IRequest<Incident>
	{
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
	/// 'Incident' create command handler.
	/// </summary>
	public class NetworkIncidentCreateCommandHandler : IRequestHandler<NetworkIncidentCreateCommand, Incident>
	{
		private readonly IDb_Context _context;
        private IApplication _application;
        //
        //
        /// <summary>
        ///  The constructor for the inner handler class, to create the Incident entity.
        /// </summary>
        /// <param name="context">The database interface context.</param>
        public NetworkIncidentCreateCommandHandler(IDb_Context context)
		{
			_context = context;
		}
		//
		/// <summary>
		/// 'Incident' command handle method, passing two interfaces.
		/// </summary>
		/// <param name="request">This create command request.</param>
		/// <param name="cancellationToken">Cancel token.</param>
		/// <returns>The Incident entity class.</returns>
		public async Task<Incident> Handle(NetworkIncidentCreateCommand request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new NetworkIncidentCreateCommandValidationException(_results.FluentValidationErrors());
			}
			// Move from create command class to entity class.
			var _entity = new Incident
			{
				ServerId = request.ServerId,
				IPAddress = request.IPAddress,
				NIC_Id = request.NIC_Id,
				NetworkName = request.NetworkName,
				AbuseEmailAddress = request.AbuseEmailAddress,
				ISPTicketNumber = request.ISPTicketNumber,
				Mailed = request.Mailed,
				Closed = request.Closed,
				Special = request.Special,
				Notes = request.Notes,
				CreatedDate = request.CreatedDate,
			};
			_context.Incidents.Add(_entity);
			await _context.SaveChangesAsync(cancellationToken);
			// Return the entity class.
			return _entity;
		}
		//
		/// <summary>
		/// FluentValidation of the 'NetworkIncidentCreateCommand' class.
		/// </summary>
		public class Validator : AbstractValidator<NetworkIncidentCreateCommand>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'NetworkIncidentCreateCommand' validator.
			/// </summary>
			public Validator()
			{
				//
				RuleFor(x => x.ServerId).NotNull();
				RuleFor(x => x.IPAddress).NotEmpty().MinimumLength(7).MaximumLength(50)
                    .Must(Extensions.ValidateIPv4);
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
	/// Custom NetworkIncidentCreateCommand validation exception.
	/// </summary>
	public class NetworkIncidentCreateCommandValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of NetworkIncidentCreateCommand validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public NetworkIncidentCreateCommandValidationException(string errorMessage)
			: base($"NetworkIncidentCreateCommand validation exception: errors: {errorMessage}")
		{
		}
	}
	//
}
// ---------------------------------------------------------------------------
