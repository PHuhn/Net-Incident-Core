
//
// ---------------------------------------------------------------------------
// NetworkLog list query.
//
using System;
using System.Linq;
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
	/// 'NetworkLog' list query, handler and handle.
	/// </summary>
	public class NetworkLogListQuery
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
	/// 'NetworkLog' list query handler.
	/// </summary>
	public class NetworkLogListQueryHandler : IRequestHandler<NetworkLogListQueryHandler.ListQuery, NetworkLogListQueryHandler.ViewModel>
	{
		private readonly IDb_Context _context;
		//
		/// <summary>
		///  The constructor for the inner handler class, to list the NetworkLog entity.
		/// </summary>
		/// <param name="context">The database interface context.</param>
		public NetworkLogListQueryHandler(IDb_Context context)
		{
			_context = context;
		}
		//
		/// <summary>
		/// 'NetworkLog' query handle method, passing two interfaces.
		/// </summary>
		/// <param name="queryRequest">This list query request.</param>
		/// <param name="cancellationToken">Cancel token.</param>
		/// <returns>Returns a list of NetworkLogListQuery.</returns>
		public async Task<ViewModel> Handle(ListQuery queryRequest, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(queryRequest);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new ListQueryValidationException(_results.FluentValidationErrors());
			}
			//
			return new ViewModel()
			{
				NetworkLogsList = await _context.NetworkLogs.ToAsyncEnumerable()
					.Select(cnt => cnt.ToNetworkLogListQuery()).ToList()
			};
		}
		//
		/// <summary>
		/// The NetworkLog list query class view class.
		/// </summary>
		public class ViewModel
		{
			public IList<NetworkLogListQuery> NetworkLogsList { get; set; }
		}
		//
		/// <summary>
		/// Get NetworkLog list query class (the primary key).
		/// </summary>
		public class ListQuery : IRequest<ViewModel>
		{
		}
		//
		/// <summary>
		/// FluentValidation of the 'NetworkLogListQuery' class.
		/// </summary>
		public class Validator : AbstractValidator<ListQuery>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'NetworkLogListQuery' validator.
			/// </summary>
			public Validator()
			{
				//
			}
			//
		}
		//
	}
	//
	/// <summary>
	/// Custom NetworkLogListQuery validation exception.
	/// </summary>
	public class ListQueryValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of NetworkLogListQuery validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public ListQueryValidationException(string errorMessage)
			: base($"NetworkLogListQuery validation exception: errors: {errorMessage}")
		{
		}
	}
    //
}
// ---------------------------------------------------------------------------
