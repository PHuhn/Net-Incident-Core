
//
// ---------------------------------------------------------------------------
// IncidentNote update command.
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
	/// 'IncidentNote' update command, handler and handle.
	/// </summary>
	public class IncidentNoteUpdateCommand : IRequest<int>
	{
		public long IncidentNoteId { get; set; }
		public int NoteTypeId { get; set; }
		public string Note { get; set; }
		public DateTime CreatedDate { get; set; }
	}
	//
	/// <summary>
	/// 'IncidentNote' update command handler.
	/// </summary>
	public class IncidentNoteUpdateCommandHandler : IRequestHandler<IncidentNoteUpdateCommand, int>
	{
		private readonly IDb_Context _context;
		//
		/// <summary>
		///  The constructor for the inner handler class, to update the IncidentNote entity.
		/// </summary>
		/// <param name="context">The database interface context.</param>
		public IncidentNoteUpdateCommandHandler(IDb_Context context)
		{
			_context = context;
		}
		//
		/// <summary>
		/// 'IncidentNote' command handle method, passing two interfaces.
		/// </summary>
		/// <param name="request">This update command request.</param>
		/// <param name="cancellationToken">Cancel token.</param>
		/// <returns>Returns the row count.</returns>
		public async Task<int> Handle(IncidentNoteUpdateCommand request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new UpdateCommandValidationException(_results.FluentValidationErrors());
			}
			var _entity = await _context.IncidentNotes
				.SingleOrDefaultAsync(r => r.IncidentNoteId == request.IncidentNoteId, cancellationToken);
			if (_entity == null)
			{
				throw new UpdateCommandKeyNotFoundException(request.IncidentNoteId);
			}
			// Move from update command class to entity class.
			_entity.NoteTypeId = request.NoteTypeId;
			_entity.Note = request.Note;
			_entity.CreatedDate = request.CreatedDate;
			//
			await _context.SaveChangesAsync(cancellationToken);
			// Return the row count.
			return 1;
		}
		//
		/// <summary>
		/// FluentValidation of the 'IncidentNoteUpdateCommand' class.
		/// </summary>
		public class Validator : AbstractValidator<IncidentNoteUpdateCommand>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'IncidentNoteUpdateCommand' validator.
			/// </summary>
			public Validator()
			{
				//
				RuleFor(x => x.IncidentNoteId).NotNull();
				RuleFor(x => x.NoteTypeId).NotNull();
				RuleFor(x => x.Note).NotEmpty().MaximumLength(1073741823);
				RuleFor(x => x.CreatedDate).NotNull();
				//
			}
			//
		}
		//
	}
	//
	/// <summary>
	/// Custom IncidentNoteUpdateCommand record not found exception.
	/// </summary>
	public class UpdateCommandKeyNotFoundException: KeyNotFoundException
	{
		//
		/// <summary>
		/// Implementation of IncidentNoteUpdateCommand record not found exception.
		/// </summary>
		/// <param name="id">The key for the record.</param>
		public UpdateCommandKeyNotFoundException(long incidentNoteId)
			: base($"IncidentNoteUpdateCommand key not found exception: id: {incidentNoteId}")
		{
		}
	}
	//
	/// <summary>
	/// Custom IncidentNoteUpdateCommand validation exception.
	/// </summary>
	public class UpdateCommandValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of IncidentNoteUpdateCommand validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public UpdateCommandValidationException(string errorMessage)
			: base($"IncidentNoteUpdateCommand validation exception: errors: {errorMessage}")
		{
		}
	}
}
// ---------------------------------------------------------------------------

