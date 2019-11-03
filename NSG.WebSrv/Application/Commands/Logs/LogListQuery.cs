//
// ---------------------------------------------------------------------------
// Log list query.
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
namespace NSG.WebSrv.Application.Commands.Logs
{
	//
	/// <summary>
	/// 'Log' list query, handler and handle.
	/// </summary>
	public class LogListQuery
	{
		public DateTime Date { get; set; }
		public string Application { get; set; }
		public string Method { get; set; }
		public string Level { get; set; }
		public string Message { get; set; }
		public string Exception { get; set; }
	}
	//
	/// <summary>
	/// 'Log' list query handler.
	/// </summary>
	public class LogListQueryHandler : IRequestHandler<LogListQueryHandler.ListQuery, LogListQueryHandler.ViewModel>
	{
		private readonly IDb_Context _context;
		//
		/// <summary>
		///  The constructor for the inner handler class, to list the Log entity.
		/// </summary>
		/// <param name="context">The database interface context.</param>
		public LogListQueryHandler(IDb_Context context)
		{
			_context = context;
		}
		//
		/// <summary>
		/// 'Log' query handle method, passing two interfaces.
		/// </summary>
		/// <param name="queryRequest">This list query request.</param>
		/// <param name="cancellationToken">Cancel token.</param>
		/// <returns>Returns a list of LogListQuery.</returns>
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
				LogsList = await _context.Logs
                    .Where( lg => lg.UserAccount == queryRequest.UserAccount )
                    .OrderByDescending( ob => ob.Date )
                    .Take( 25 )
                    .ToAsyncEnumerable()
					.Select(cnt => cnt.ToLogListQuery()).ToList()
			};
		}
		//
		/// <summary>
		/// The Log list query class view class.
		/// </summary>
		public class ViewModel
		{
			public IList<LogListQuery> LogsList { get; set; }
		}
		//
		/// <summary>
		/// Get Log list query class (the primary key).
		/// </summary>
		public class ListQuery : IRequest<ViewModel>
		{
            public string UserAccount { get; set; }
        }
		//
		/// <summary>
		/// FluentValidation of the 'LogListQuery' class.
		/// </summary>
		public class Validator : AbstractValidator<ListQuery>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'LogListQuery' validator.
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
	/// Custom LogListQuery validation exception.
	/// </summary>
	public class ListQueryValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of LogListQuery validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public ListQueryValidationException(string errorMessage)
			: base($"LogListQuery validation exception: errors: {errorMessage}")
		{
		}
	}
	//
}
// ---------------------------------------------------------------------------
