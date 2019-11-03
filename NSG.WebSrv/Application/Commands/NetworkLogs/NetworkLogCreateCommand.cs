
//
// ---------------------------------------------------------------------------
// NetworkLog create command.
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
//
namespace NSG.WebSrv.Application.Commands.NetworkLogs
{
	//
	/// <summary>
	/// 'NetworkLog' create command, handler and handle.
	/// </summary>
	public class NetworkLogCreateCommand : IRequest<NetworkLog>
	{
		public int ServerId { get; set; }
		public long? IncidentId { get; set; }
		public string IPAddress { get; set; }
		public DateTime NetworkLogDate { get; set; }
		public string Log { get; set; }
		public int IncidentTypeId { get; set; }
	}
	//
	/// <summary>
	/// 'NetworkLog' create command handler.
	/// </summary>
	public class NetworkLogCreateCommandHandler : IRequestHandler<NetworkLogCreateCommand, NetworkLog>
	{
		private readonly IDb_Context _context;
		//
		//
		/// <summary>
		///  The constructor for the inner handler class, to create the NetworkLog entity.
		/// </summary>
		/// <param name="context">The database interface context.</param>
		public NetworkLogCreateCommandHandler(IDb_Context context)
		{
			_context = context;
		}
		//
		/// <summary>
		/// 'NetworkLog' command handle method, passing two interfaces.
		/// </summary>
		/// <param name="request">This create command request.</param>
		/// <param name="cancellationToken">Cancel token.</param>
		/// <returns>The NetworkLog entity class.</returns>
		public async Task<NetworkLog> Handle(NetworkLogCreateCommand request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new CreateCommandValidationException(_results.FluentValidationErrors());
			}
			// Move from create command class to entity class.
			var _entity = new NetworkLog
			{
				ServerId = request.ServerId,
				IncidentId = request.IncidentId,
				IPAddress = request.IPAddress,
				NetworkLogDate = request.NetworkLogDate,
				Log = request.Log,
				IncidentTypeId = request.IncidentTypeId,
			};
			_context.NetworkLogs.Add(_entity);
			await _context.SaveChangesAsync(cancellationToken);
			// Return the entity class.
			return _entity;
		}
		//
		/// <summary>
		/// FluentValidation of the 'NetworkLogCreateCommand' class.
		/// </summary>
		public class Validator : AbstractValidator<NetworkLogCreateCommand>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'NetworkLogCreateCommand' validator.
			/// </summary>
			public Validator()
			{
				//
				RuleFor(x => x.ServerId).NotNull();
				RuleFor(x => x.IPAddress).NotEmpty().MaximumLength(50);
				RuleFor(x => x.NetworkLogDate).NotNull();
				RuleFor(x => x.Log).NotEmpty().MaximumLength(1073741823);
				RuleFor(x => x.IncidentTypeId).NotNull();
				//
			}
			//
		}
		//
	}
	//
	/// <summary>
	/// Custom NetworkLogCreateCommand validation exception.
	/// </summary>
	public class CreateCommandValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of NetworkLogCreateCommand validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public CreateCommandValidationException(string errorMessage)
			: base($"NetworkLogCreateCommand validation exception: errors: {errorMessage}")
		{
		}
	}
	//
}
// ---------------------------------------------------------------------------

