
//
// ---------------------------------------------------------------------------
// NetworkLog delete command.
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
	/// 'NetworkLog' delete command, handler and handle.
	/// </summary>
	public class NetworkLogDeleteCommand : IRequest<int>
	{
		public long NetworkLogId { get; set; }
	}
	//
	/// <summary>
	/// 'NetworkLog' delete command handler.
	/// </summary>
	public class NetworkLogDeleteCommandHandler : IRequestHandler<NetworkLogDeleteCommand, int>
	{
		private readonly IDb_Context _context;
		//
		/// <summary>
		///  The constructor for the inner handler class, to delete the NetworkLog entity.
		/// </summary>
		/// <param name="context">The database interface context.</param>
		public NetworkLogDeleteCommandHandler(IDb_Context context)
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
		public async Task<int> Handle(NetworkLogDeleteCommand request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new DeleteCommandValidationException(_results.FluentValidationErrors());
			}
			var _entity = await _context.NetworkLogs
				.SingleOrDefaultAsync(r => r.NetworkLogId == request.NetworkLogId, cancellationToken);
			if (_entity == null)
			{
				throw new DeleteCommandKeyNotFoundException(request.NetworkLogId);
			}
			//
			_context.NetworkLogs.Remove(_entity);
			await _context.SaveChangesAsync(cancellationToken);
			// Return the row count affected.
			return 1;
		}
		//
		/// <summary>
		/// FluentValidation of the 'NetworkLogDeleteCommand' class.
		/// </summary>
		public class Validator : AbstractValidator<NetworkLogDeleteCommand>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'NetworkLogDeleteCommand' validator.
			/// </summary>
			public Validator()
			{
				//
				RuleFor(x => x.NetworkLogId).NotNull();
				//
			}
			//
		}
		//
	}
	//
	/// <summary>
	/// Custom NetworkLogDeleteCommand record not found exception.
	/// </summary>
	public class DeleteCommandKeyNotFoundException: KeyNotFoundException
	{
		//
		/// <summary>
		/// Implementation of NetworkLogDeleteCommand record not found exception.
		/// </summary>
		/// <param name="id">The key for the record.</param>
		public DeleteCommandKeyNotFoundException(long networkLogId)
			: base($"NetworkLogDeleteCommand key not found exception: id: {networkLogId}")
		{
		}
	}
	//
	/// <summary>
	/// Custom NetworkLogDeleteCommand validation exception.
	/// </summary>
	public class DeleteCommandValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of NetworkLogDeleteCommand validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public DeleteCommandValidationException(string errorMessage)
			: base($"NetworkLogDeleteCommand validation exception: errors: {errorMessage}")
		{
		}
	}
	//
}
// ---------------------------------------------------------------------------

