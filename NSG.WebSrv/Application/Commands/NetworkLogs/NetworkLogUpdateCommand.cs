
//
// ---------------------------------------------------------------------------
// NetworkLog update command.
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
//
namespace NSG.WebSrv.Application.Commands.NetworkLogs
{
	//
	/// <summary>
	/// 'NetworkLog' update command, handler and handle.
	/// </summary>
	public class NetworkLogUpdateCommand : IRequest<int>
	{
		public long NetworkLogId { get; set; }
		public int ServerId { get; set; }
		public long IncidentId { get; set; }
		public string IPAddress { get; set; }
		public DateTime NetworkLogDate { get; set; }
		public string Log { get; set; }
		public int IncidentTypeId { get; set; }
	}
	//
	/// <summary>
	/// 'NetworkLog' update command handler.
	/// </summary>
	public class NetworkLogUpdateCommandHandler : IRequestHandler<NetworkLogUpdateCommand, int>
	{
		private readonly IDb_Context _context;
		//
		/// <summary>
		///  The constructor for the inner handler class, to update the NetworkLog entity.
		/// </summary>
		/// <param name="context">The database interface context.</param>
		public NetworkLogUpdateCommandHandler(IDb_Context context)
		{
			_context = context;
		}
		//
		/// <summary>
		/// 'NetworkLog' command handle method, passing two interfaces.
		/// </summary>
		/// <param name="request">This update command request.</param>
		/// <param name="cancellationToken">Cancel token.</param>
		/// <returns>Returns the row count.</returns>
		public async Task<int> Handle(NetworkLogUpdateCommand request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new UpdateCommandValidationException(_results.FluentValidationErrors());
			}
			var _entity = await _context.NetworkLogs
				.SingleOrDefaultAsync(r => r.NetworkLogId == request.NetworkLogId, cancellationToken);
			if (_entity == null)
			{
				throw new UpdateCommandKeyNotFoundException(request.NetworkLogId);
			}
			// Move from update command class to entity class.
			_entity.ServerId = request.ServerId;
			_entity.IncidentId = request.IncidentId;
			_entity.IPAddress = request.IPAddress;
			_entity.NetworkLogDate = request.NetworkLogDate;
			_entity.Log = request.Log;
			_entity.IncidentTypeId = request.IncidentTypeId;
			//
			await _context.SaveChangesAsync(cancellationToken);
			// Return the row count.
			return 1;
		}
		//
		/// <summary>
		/// FluentValidation of the 'NetworkLogUpdateCommand' class.
		/// </summary>
		public class Validator : AbstractValidator<NetworkLogUpdateCommand>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'NetworkLogUpdateCommand' validator.
			/// </summary>
			public Validator()
			{
				//
				RuleFor(x => x.NetworkLogId).NotNull();
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
	/// Custom NetworkLogUpdateCommand record not found exception.
	/// </summary>
	public class UpdateCommandKeyNotFoundException: KeyNotFoundException
	{
		//
		/// <summary>
		/// Implementation of NetworkLogUpdateCommand record not found exception.
		/// </summary>
		/// <param name="id">The key for the record.</param>
		public UpdateCommandKeyNotFoundException(long networkLogId)
			: base($"NetworkLogUpdateCommand key not found exception: id: {networkLogId}")
		{
		}
	}
	//
	/// <summary>
	/// Custom NetworkLogUpdateCommand validation exception.
	/// </summary>
	public class UpdateCommandValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of NetworkLogUpdateCommand validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public UpdateCommandValidationException(string errorMessage)
			: base($"NetworkLogUpdateCommand validation exception: errors: {errorMessage}")
		{
		}
	}
}
// ---------------------------------------------------------------------------

