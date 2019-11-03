
//
// ---------------------------------------------------------------------------
// IncidentNote detail query.
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
namespace NSG.WebSrv.Application.Commands.IncidentNotes
{
	//
	/// <summary>
	/// 'IncidentNote' detail query, handler and handle.
	/// </summary>
	public class IncidentNoteDetailQuery
	{
		[System.ComponentModel.DataAnnotations.Key]
		public long IncidentNoteId { get; set; }
		public int NoteTypeId { get; set; }
		public string Note { get; set; }
		public DateTime CreatedDate { get; set; }
	}
	//
	/// <summary>
	/// 'IncidentNote' detail query handler.
	/// </summary>
	public class IncidentNoteDetailQueryHandler : IRequestHandler<IncidentNoteDetailQueryHandler.DetailQuery, IncidentNoteDetailQuery>
	{
		private readonly IDb_Context _context;
		//
		/// <summary>
		///  The constructor for the inner handler class, to detail the IncidentNote entity.
		/// </summary>
		/// <param name="context">The database interface context.</param>
		public IncidentNoteDetailQueryHandler(IDb_Context context)
		{
			_context = context;
		}
		//
		/// <summary>
		/// 'IncidentNote' query handle method, passing two interfaces.
		/// </summary>
		/// <param name="request">This detail query request.</param>
		/// <param name="cancellationToken">Cancel token.</param>
		/// <returns>Returns the row count.</returns>
		public async Task<IncidentNoteDetailQuery> Handle(DetailQuery request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new DetailQueryValidationException(_results.FluentValidationErrors());
			}
			var _entity = await GetEntityByKey(request.IncidentNoteId);
			if (_entity == null)
			{
				throw new DetailQueryKeyNotFoundException(request.IncidentNoteId);
			}
			//
			// Return the detail query model.
			return _entity.ToIncidentNoteDetailQuery();
		}
		//
		/// <summary>
		/// Get an entity record via the primary key.
		/// </summary>
		/// <param name="incidentNoteId">long key</param>
		/// <returns>Returns a IncidentNote entity record.</returns>
		private Task<IncidentNote> GetEntityByKey(long incidentNoteId)
		{
			return _context.IncidentNotes.SingleOrDefaultAsync(r => r.IncidentNoteId == incidentNoteId);
		}
		//
		/// <summary>
		/// Get IncidentNote detail query class (the primary key).
		/// </summary>
		public class DetailQuery : IRequest<IncidentNoteDetailQuery>
		{
			public long IncidentNoteId { get; set; }
		}
		//
		/// <summary>
		/// FluentValidation of the 'IncidentNoteDetailQuery' class.
		/// </summary>
		public class Validator : AbstractValidator<DetailQuery>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'IncidentNoteDetailQuery' validator.
			/// </summary>
			public Validator()
			{
				//
				RuleFor(x => x.IncidentNoteId).NotNull();
				//
			}
			//
		}
		//
	}
	//
	/// <summary>
	/// Custom IncidentNoteDetailQuery record not found exception.
	/// </summary>
	public class DetailQueryKeyNotFoundException: KeyNotFoundException
	{
		//
		/// <summary>
		/// Implementation of IncidentNoteDetailQuery record not found exception.
		/// </summary>
		/// <param name="id">The key for the record.</param>
		public DetailQueryKeyNotFoundException(long incidentNoteId)
			: base($"IncidentNoteDetailQuery key not found exception: Id: {incidentNoteId}")
		{
		}
	}
	//
	/// <summary>
	/// Custom IncidentNoteDetailQuery validation exception.
	/// </summary>
	public class DetailQueryValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of IncidentNoteDetailQuery validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public DetailQueryValidationException(string errorMessage)
			: base($"IncidentNoteDetailQuery validation exception: errors: {errorMessage}")
		{
		}
	}
	//
}
// ---------------------------------------------------------------------------

