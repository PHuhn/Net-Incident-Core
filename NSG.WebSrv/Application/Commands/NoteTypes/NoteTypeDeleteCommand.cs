//
// ---------------------------------------------------------------------------
// NoteType delete command.
//
using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
//
using Microsoft.EntityFrameworkCore;
using MediatR;
using FluentValidation;
using FluentValidation.Results;
using NSG.WebSrv.Domain.Entities;
using NSG.WebSrv.Application.Commands.Logs;
//
namespace NSG.WebSrv.Application.Commands.NoteTypes
{
	//
	/// <summary>
	/// 'NoteType' delete command, handler and handle.
	/// </summary>
	public class NoteTypeDeleteCommand : IRequest<int>
	{
		public int NoteTypeId { get; set; }
	}
	//
	/// <summary>
	/// 'NoteType' delete command handler.
	/// </summary>
	public class NoteTypeDeleteCommandHandler : IRequestHandler<NoteTypeDeleteCommand, int>
	{
		private readonly IDb_Context _context;
        protected IMediator Mediator;
        //
        /// <summary>
        ///  The constructor for the inner handler class, to delete the NoteType entity.
        /// </summary>
        /// <param name="context">The database interface context.</param>
        public NoteTypeDeleteCommandHandler(IDb_Context context, IMediator mediator)
		{
			_context = context;
            Mediator = mediator;
        }
		//
		/// <summary>
		/// 'NoteType' command handle method, passing two interfaces.
		/// </summary>
		/// <param name="request">This update command request.</param>
		/// <param name="cancellationToken">Cancel token.</param>
		/// <returns>Returns the row count.</returns>
		public async Task<int> Handle(NoteTypeDeleteCommand request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new DeleteCommandValidationException(_results.FluentValidationErrors());
			}
			var _entity = await _context.NoteTypes
                .Include( inn => inn.IncidentNotes )
                .SingleOrDefaultAsync(r => r.NoteTypeId == request.NoteTypeId, cancellationToken);
			if (_entity == null)
			{
				throw new DeleteCommandKeyNotFoundException(request.NoteTypeId);
			}
            // require user to delete all servers before deleting company.
            if (_entity.IncidentNotes.Count > 0)
            {
                throw new NoteTypeDeleteCommandActiveIncidentNotesException(
                    string.Format("IncidentNotes count: {0}", _entity.IncidentNotes.Count));
            }
            //
            _context.NoteTypes.Remove(_entity);
			await _context.SaveChangesAsync(cancellationToken);
            await Mediator.Send(new LogCreateCommand(
                LoggingLevel.Warning, MethodBase.GetCurrentMethod(),
                "Deleted NoteType: " + _entity.NoteTypeToString(), null));
            // Return the row count affected.
            return 1;
		}
		//
		/// <summary>
		/// FluentValidation of the 'NoteTypeDeleteCommand' class.
		/// </summary>
		public class Validator : AbstractValidator<NoteTypeDeleteCommand>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'NoteTypeDeleteCommand' validator.
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
	/// Custom NoteTypeDeleteCommand record not found exception.
	/// </summary>
	public class DeleteCommandKeyNotFoundException: KeyNotFoundException
	{
		//
		/// <summary>
		/// Implementation of NoteTypeDeleteCommand record not found exception.
		/// </summary>
		/// <param name="id">The key for the record.</param>
		public DeleteCommandKeyNotFoundException(int noteTypeId)
			: base($"NoteTypeDeleteCommand key not found exception: id: {noteTypeId}")
		{
		}
	}
	//
	/// <summary>
	/// Custom NoteTypeDeleteCommand validation exception.
	/// </summary>
	public class DeleteCommandValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of NoteTypeDeleteCommand validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public DeleteCommandValidationException(string errorMessage)
			: base($"NoteTypeDeleteCommand validation exception: errors: {errorMessage}")
		{
		}
	}
    //
    /// <summary>
    /// Custom NoteTypeDeleteCommand validation exception.
    /// </summary>
    public class NoteTypeDeleteCommandActiveIncidentNotesException : Exception
    {
        //
        /// <summary>
        /// Implementation of ServerDeleteCommand validation exception.
        /// </summary>
        /// <param name="errorMessage">The validation error messages.</param>
        public NoteTypeDeleteCommandActiveIncidentNotesException(string errorMessage)
            : base($"NoteTypeDeleteCommand contains active note types on IncidentNotes validation exception: errors: {errorMessage}")
        {
        }
    }
    //
}
// ---------------------------------------------------------------------------
