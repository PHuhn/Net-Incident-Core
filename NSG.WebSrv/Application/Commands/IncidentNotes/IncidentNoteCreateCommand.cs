//
// ---------------------------------------------------------------------------
// IncidentNote create command.
//
using System;
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
	/// 'IncidentNote' create command, handler and handle.
	/// </summary>
	public class IncidentNoteCreateCommand : IRequest<IncidentNote>
	{
		public int NoteTypeId { get; set; }
		public string Note { get; set; }
	}
	//
	/// <summary>
	/// 'IncidentNote' create command handler.
	/// </summary>
	public class IncidentNoteCreateCommandHandler : IRequestHandler<IncidentNoteCreateCommand, IncidentNote>
	{
		private readonly IDb_Context _context;
		//
		//
		/// <summary>
		///  The constructor for the inner handler class, to create the IncidentNote entity.
		/// </summary>
		/// <param name="context">The database interface context.</param>
		public IncidentNoteCreateCommandHandler(IDb_Context context)
		{
			_context = context;
		}
		//
		/// <summary>
		/// 'IncidentNote' command handle method, passing two interfaces.
		/// </summary>
		/// <param name="request">This create command request.</param>
		/// <param name="cancellationToken">Cancel token.</param>
		/// <returns>The IncidentNote entity class.</returns>
		public async Task<IncidentNote> Handle(IncidentNoteCreateCommand request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new CreateCommandValidationException(_results.FluentValidationErrors());
			}
			// Move from create command class to entity class.
			var _entity = new IncidentNote
			{
				NoteTypeId = request.NoteTypeId,
				Note = request.Note,
				CreatedDate = DateTime.Now
			};
			_context.IncidentNotes.Add(_entity);
			await _context.SaveChangesAsync(cancellationToken);
			// Return the entity class.
			return _entity;
		}
		//
		/// <summary>
		/// FluentValidation of the 'IncidentNoteCreateCommand' class.
		/// </summary>
		public class Validator : AbstractValidator<IncidentNoteCreateCommand>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'IncidentNoteCreateCommand' validator.
			/// </summary>
			public Validator()
			{
				//
				RuleFor(x => x.NoteTypeId).NotNull();
				RuleFor(x => x.Note).NotEmpty().MaximumLength(1073741823);
				//
			}
			//
		}
		//
	}
	//
	/// <summary>
	/// Custom IncidentNoteCreateCommand validation exception.
	/// </summary>
	public class CreateCommandValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of IncidentNoteCreateCommand validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public CreateCommandValidationException(string errorMessage)
			: base($"IncidentNoteCreateCommand validation exception: errors: {errorMessage}")
		{
		}
	}
	//
}
// ---------------------------------------------------------------------------

