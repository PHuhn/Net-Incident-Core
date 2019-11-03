//
// ---------------------------------------------------------------------------
// NoteType list query.
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
namespace NSG.WebSrv.Application.Commands.NoteTypes
{
	//
	/// <summary>
	/// 'NoteType' list query, handler and handle.
	/// </summary>
	public class NoteTypeListQuery
	{
		[System.ComponentModel.DataAnnotations.Key]
		public int NoteTypeId { get; set; }
		public string NoteTypeShortDesc { get; set; }
		public string NoteTypeDesc { get; set; }
        public string NoteTypeClientScript { get; set; }
    }
    //
    /// <summary>
    /// 'NoteType' list query handler.
    /// </summary>
    public class NoteTypeListQueryHandler : IRequestHandler<NoteTypeListQueryHandler.ListQuery, NoteTypeListQueryHandler.ViewModel>
	{
		private readonly IDb_Context _context;
		//
		/// <summary>
		///  The constructor for the inner handler class, to list the NoteType entity.
		/// </summary>
		/// <param name="context">The database interface context.</param>
		public NoteTypeListQueryHandler(IDb_Context context)
		{
			_context = context;
		}
		//
		/// <summary>
		/// 'NoteType' query handle method, passing two interfaces.
		/// </summary>
		/// <param name="queryRequest">This list query request.</param>
		/// <param name="cancellationToken">Cancel token.</param>
		/// <returns>Returns a list of NoteTypeListQuery.</returns>
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
				NoteTypesList = await _context.NoteTypes.ToAsyncEnumerable()
					.Select(cnt => cnt.ToNoteTypeListQuery()).ToList()
			};
		}
		//
		/// <summary>
		/// The NoteType list query class view class.
		/// </summary>
		public class ViewModel
		{
			public IList<NoteTypeListQuery> NoteTypesList { get; set; }
		}
		//
		/// <summary>
		/// Get NoteType list query class (the primary key).
		/// </summary>
		public class ListQuery : IRequest<ViewModel>
		{
		}
		//
		/// <summary>
		/// FluentValidation of the 'NoteTypeListQuery' class.
		/// </summary>
		public class Validator : AbstractValidator<ListQuery>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'NoteTypeListQuery' validator.
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
	/// Custom NoteTypeListQuery validation exception.
	/// </summary>
	public class ListQueryValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of NoteTypeListQuery validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public ListQueryValidationException(string errorMessage)
			: base($"NoteTypeListQuery validation exception: errors: {errorMessage}")
		{
		}
	}
	//
}
// ---------------------------------------------------------------------------

