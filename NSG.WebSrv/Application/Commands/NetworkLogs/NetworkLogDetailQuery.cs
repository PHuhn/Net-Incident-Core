
//
// ---------------------------------------------------------------------------
// NetworkLog detail query.
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
	/// 'NetworkLog' detail query, handler and handle.
	/// </summary>
	public class NetworkLogDetailQuery
	{
		[System.ComponentModel.DataAnnotations.Key]
		public long NetworkLogId { get; set; }
		public int ServerId { get; set; }
		public long? IncidentId { get; set; }
		public string IPAddress { get; set; }
		public DateTime NetworkLogDate { get; set; }
		public string Log { get; set; }
		public int IncidentTypeId { get; set; }
	}
	//
	/// <summary>
	/// 'NetworkLog' detail query handler.
	/// </summary>
	public class NetworkLogDetailQueryHandler : IRequestHandler<NetworkLogDetailQueryHandler.DetailQuery, NetworkLogDetailQuery>
	{
		private readonly IDb_Context _context;
		//
		/// <summary>
		///  The constructor for the inner handler class, to detail the NetworkLog entity.
		/// </summary>
		/// <param name="context">The database interface context.</param>
		public NetworkLogDetailQueryHandler(IDb_Context context)
		{
			_context = context;
		}
		//
		/// <summary>
		/// 'NetworkLog' query handle method, passing two interfaces.
		/// </summary>
		/// <param name="request">This detail query request.</param>
		/// <param name="cancellationToken">Cancel token.</param>
		/// <returns>Returns the row count.</returns>
		public async Task<NetworkLogDetailQuery> Handle(DetailQuery request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new DetailQueryValidationException(_results.FluentValidationErrors());
			}
			var _entity = await GetEntityByKey(request.NetworkLogId);
			if (_entity == null)
			{
				throw new DetailQueryKeyNotFoundException(request.NetworkLogId);
			}
			//
			// Return the detail query model.
			return _entity.ToNetworkLogDetailQuery();
		}
		//
		/// <summary>
		/// Get an entity record via the primary key.
		/// </summary>
		/// <param name="networkLogId">long key</param>
		/// <returns>Returns a NetworkLog entity record.</returns>
		private Task<NetworkLog> GetEntityByKey(long networkLogId)
		{
			return _context.NetworkLogs.SingleOrDefaultAsync(r => r.NetworkLogId == networkLogId);
		}
		//
		/// <summary>
		/// Get NetworkLog detail query class (the primary key).
		/// </summary>
		public class DetailQuery : IRequest<NetworkLogDetailQuery>
		{
			public long NetworkLogId { get; set; }
		}
		//
		/// <summary>
		/// FluentValidation of the 'NetworkLogDetailQuery' class.
		/// </summary>
		public class Validator : AbstractValidator<DetailQuery>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'NetworkLogDetailQuery' validator.
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
	/// Custom NetworkLogDetailQuery record not found exception.
	/// </summary>
	public class DetailQueryKeyNotFoundException: KeyNotFoundException
	{
		//
		/// <summary>
		/// Implementation of NetworkLogDetailQuery record not found exception.
		/// </summary>
		/// <param name="id">The key for the record.</param>
		public DetailQueryKeyNotFoundException(long networkLogId)
			: base($"NetworkLogDetailQuery key not found exception: Id: {networkLogId}")
		{
		}
	}
	//
	/// <summary>
	/// Custom NetworkLogDetailQuery validation exception.
	/// </summary>
	public class DetailQueryValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of NetworkLogDetailQuery validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public DetailQueryValidationException(string errorMessage)
			: base($"NetworkLogDetailQuery validation exception: errors: {errorMessage}")
		{
		}
	}
}
// ---------------------------------------------------------------------------
