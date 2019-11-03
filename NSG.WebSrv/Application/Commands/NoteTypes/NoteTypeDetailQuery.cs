//
// ---------------------------------------------------------------------------
// NoteType detail query.
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
using System.ComponentModel.DataAnnotations;
//
namespace NSG.WebSrv.Application.Commands.NoteTypes
{
	//
	/// <summary>
	/// 'NoteType' detail query, handler and handle.
	/// </summary>
	public class NoteTypeDetailQuery
	{
		[Key]
		public int NoteTypeId { get; set; }
		public string NoteTypeShortDesc { get; set; }
		public string NoteTypeDesc { get; set; }
        public string NoteTypeClientScript { get; set; }
    }
    //
    /// <summary>
    /// 'NoteType' detail query handler.
    /// </summary>
    public class NoteTypeDetailQueryHandler : IRequestHandler<NoteTypeDetailQueryHandler.DetailQuery, NoteTypeDetailQuery>
	{
		private readonly IDb_Context _context;
		//
		/// <summary>
		///  The constructor for the inner handler class, to detail the NoteType entity.
		/// </summary>
		/// <param name="context">The database interface context.</param>
		public NoteTypeDetailQueryHandler(IDb_Context context)
		{
			_context = context;
		}
		//
		/// <summary>
		/// 'NoteType' query handle method, passing two interfaces.
		/// </summary>
		/// <param name="request">This detail query request.</param>
		/// <param name="cancellationToken">Cancel token.</param>
		/// <returns>Returns the row count.</returns>
		public async Task<NoteTypeDetailQuery> Handle(DetailQuery request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
            FluentValidation.Results.ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new DetailQueryValidationException(_results.FluentValidationErrors());
			}
			var _entity = await GetEntityByKey(request.NoteTypeId);
			if (_entity == null)
			{
				throw new DetailQueryKeyNotFoundException(request.NoteTypeId);
			}
			//
			// Return the detail query model.
			return _entity.ToNoteTypeDetailQuery();
		}
		//
		/// <summary>
		/// Get an entity record via the primary key.
		/// </summary>
		/// <param name="noteTypeId">int key</param>
		/// <returns>Returns a NoteType entity record.</returns>
		private Task<NoteType> GetEntityByKey(int noteTypeId)
		{
			return _context.NoteTypes.SingleOrDefaultAsync(r => r.NoteTypeId == noteTypeId);
		}
		//
		/// <summary>
		/// Get NoteType detail query class (the primary key).
		/// </summary>
		public class DetailQuery : IRequest<NoteTypeDetailQuery>
		{
			public int NoteTypeId { get; set; }
		}
		//
		/// <summary>
		/// FluentValidation of the 'NoteTypeDetailQuery' class.
		/// </summary>
		public class Validator : AbstractValidator<DetailQuery>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'NoteTypeDetailQuery' validator.
			/// </summary>
			public Validator()
			{
				//
				RuleFor(x => x.NoteTypeId).NotNull();
				//
			}
			//
		}
		//
	}
	//
	/// <summary>
	/// Custom NoteTypeDetailQuery record not found exception.
	/// </summary>
	public class DetailQueryKeyNotFoundException: KeyNotFoundException
	{
		//
		/// <summary>
		/// Implementation of NoteTypeDetailQuery record not found exception.
		/// </summary>
		/// <param name="id">The key for the record.</param>
		public DetailQueryKeyNotFoundException(int noteTypeId)
			: base($"NoteTypeDetailQuery key not found exception: Id: {noteTypeId}")
		{
		}
	}
	//
	/// <summary>
	/// Custom NoteTypeDetailQuery validation exception.
	/// </summary>
	public class DetailQueryValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of NoteTypeDetailQuery validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public DetailQueryValidationException(string errorMessage)
			: base($"NoteTypeDetailQuery validation exception: errors: {errorMessage}")
		{
		}
	}
	//
}
// ---------------------------------------------------------------------------

