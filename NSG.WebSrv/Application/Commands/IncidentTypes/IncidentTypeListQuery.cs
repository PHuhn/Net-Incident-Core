//
// ---------------------------------------------------------------------------
// IncidentType list query.
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
namespace NSG.WebSrv.Application.Commands.IncidentTypes
{
	//
	/// <summary>
	/// 'IncidentType' list query, handler and handle.
	/// </summary>
	public class IncidentTypeListQuery
	{
		[System.ComponentModel.DataAnnotations.Key]
		public int IncidentTypeId { get; set; }
		public string IncidentTypeShortDesc { get; set; }
		public string IncidentTypeDesc { get; set; }
		public bool IncidentTypeFromServer { get; set; }
		public string IncidentTypeSubjectLine { get; set; }
		public string IncidentTypeEmailTemplate { get; set; }
		public string IncidentTypeTimeTemplate { get; set; }
		public string IncidentTypeThanksTemplate { get; set; }
		public string IncidentTypeLogTemplate { get; set; }
		public string IncidentTypeTemplate { get; set; }
	}
	//
	/// <summary>
	/// 'IncidentType' list query handler.
	/// </summary>
	public class IncidentTypeListQueryHandler : IRequestHandler<IncidentTypeListQueryHandler.ListQuery, IncidentTypeListQueryHandler.ViewModel>
	{
		private readonly IDb_Context _context;
		//
		/// <summary>
		///  The constructor for the inner handler class, to list the IncidentType entity.
		/// </summary>
		/// <param name="context">The database interface context.</param>
		public IncidentTypeListQueryHandler(IDb_Context context)
		{
			_context = context;
		}
		//
		/// <summary>
		/// 'IncidentType' query handle method, passing two interfaces.
		/// </summary>
		/// <param name="queryRequest">This list query request.</param>
		/// <param name="cancellationToken">Cancel token.</param>
		/// <returns>Returns a list of IncidentTypeListQuery.</returns>
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
				IncidentTypesList = await _context.IncidentTypes.ToAsyncEnumerable()
					.Select(cnt => cnt.ToIncidentTypeListQuery()).ToList()
			};
		}
		//
		/// <summary>
		/// The IncidentType list query class view class.
		/// </summary>
		public class ViewModel
		{
			public IList<IncidentTypeListQuery> IncidentTypesList { get; set; }
		}
		//
		/// <summary>
		/// Get IncidentType list query class (the primary key).
		/// </summary>
		public class ListQuery : IRequest<ViewModel>
		{
		}
		//
		/// <summary>
		/// FluentValidation of the 'IncidentTypeListQuery' class.
		/// </summary>
		public class Validator : AbstractValidator<ListQuery>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'IncidentTypeListQuery' validator.
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
	/// Custom IncidentTypeListQuery validation exception.
	/// </summary>
	public class ListQueryValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of IncidentTypeListQuery validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public ListQueryValidationException(string errorMessage)
			: base($"IncidentTypeListQuery validation exception: errors: {errorMessage}")
		{
		}
	}
	//
}
// ---------------------------------------------------------------------------

