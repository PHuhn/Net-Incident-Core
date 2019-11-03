
//
// ---------------------------------------------------------------------------
// IncidentNote list query.
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
namespace NSG.WebSrv.Application.Commands.IncidentNotes
{
	//
	/// <summary>
	/// 'IncidentNote' list query, handler and handle.
	/// </summary>
	public class IncidentNoteListQuery
	{
		[System.ComponentModel.DataAnnotations.Key]
		public long IncidentNoteId { get; set; }
		public int NoteTypeId { get; set; }
		public string Note { get; set; }
		public DateTime CreatedDate { get; set; }
	}
	//
	/// <summary>
	/// 'IncidentNote' list query handler.
	/// </summary>
	public class IncidentNoteListQueryHandler : IRequestHandler<IncidentNoteListQueryHandler.ListQuery, IncidentNoteListQueryHandler.ViewModel>
	{
		private readonly IDb_Context _context;
		//
		/// <summary>
		///  The constructor for the inner handler class, to list the IncidentNote entity.
		/// </summary>
		/// <param name="context">The database interface context.</param>
		public IncidentNoteListQueryHandler(IDb_Context context)
		{
			_context = context;
		}
		//
		/// <summary>
		/// 'IncidentNote' query handle method, passing two interfaces.
		/// </summary>
		/// <param name="queryRequest">This list query request.</param>
		/// <param name="cancellationToken">Cancel token.</param>
		/// <returns>Returns a list of IncidentNoteListQuery.</returns>
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
				IncidentNotesList = await _context.IncidentNotes.ToAsyncEnumerable()
					.Select(cnt => cnt.ToIncidentNoteListQuery()).ToList()
			};
		}
		//
		/// <summary>
		/// The IncidentNote list query class view class.
		/// </summary>
		public class ViewModel
		{
			public IList<IncidentNoteListQuery> IncidentNotesList { get; set; }
		}
		//
		/// <summary>
		/// Get IncidentNote list query class (the primary key).
		/// </summary>
		public class ListQuery : IRequest<ViewModel>
		{
		}
		//
		/// <summary>
		/// FluentValidation of the 'IncidentNoteListQuery' class.
		/// </summary>
		public class Validator : AbstractValidator<ListQuery>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'IncidentNoteListQuery' validator.
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
	/// Custom IncidentNoteListQuery validation exception.
	/// </summary>
	public class ListQueryValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of IncidentNoteListQuery validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public ListQueryValidationException(string errorMessage)
			: base($"IncidentNoteListQuery validation exception: errors: {errorMessage}")
		{
		}
	}
}
// ---------------------------------------------------------------------------

