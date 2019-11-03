
//
// ---------------------------------------------------------------------------
// IncidentNote delete command.
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
using NSG.WebSrv.Application.Commands.Logs;
using System.Reflection;
//
namespace NSG.WebSrv.Application.Commands.IncidentNotes
{
	//
	/// <summary>
	/// 'IncidentNote' delete command, handler and handle.
	/// </summary>
	public class IncidentNoteDeleteCommand : IRequest<int>
	{
		public long IncidentNoteId { get; set; }
	}
	//
	/// <summary>
	/// 'IncidentNote' delete command handler.
	/// </summary>
	public class IncidentNoteDeleteCommandHandler : IRequestHandler<IncidentNoteDeleteCommand, int>
	{
        //
		private readonly IDb_Context _context;
        protected IMediator Mediator;
        //
        /// <summary>
        ///  The constructor for the inner handler class, to delete the IncidentNote entity.
        /// </summary>
        /// <param name="context">The database interface context.</param>
        public IncidentNoteDeleteCommandHandler(IDb_Context context, IMediator mediator)
        {
            _context = context;
            Mediator = mediator;
        }
        //
        /// <summary>
        /// 'IncidentNote' command handle method, passing two interfaces.
        /// </summary>
        /// <param name="request">This update command request.</param>
        /// <param name="cancellationToken">Cancel token.</param>
        /// <returns>Returns the row count.</returns>
        public async Task<int> Handle(IncidentNoteDeleteCommand request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new DeleteCommandValidationException(_results.FluentValidationErrors());
			}
			var _entity = await _context.IncidentNotes
                .Include(_r => _r.NoteType)
                .Include(_r => _r.IncidentIncidentNotes)
				.SingleOrDefaultAsync(r => r.IncidentNoteId == request.IncidentNoteId, cancellationToken);
			if (_entity == null)
			{
				throw new DeleteCommandKeyNotFoundException(request.IncidentNoteId);
			}
            //
            if(_entity.IncidentIncidentNotes.Count > 1)
            {
                throw new IncidentNoteDeleteCommandActiveTooManyIncidentsException(
                    string.Format("IncidentIncidentNotes count: {0}", _entity.IncidentIncidentNotes.Count));
            }
            //
            if (_entity.IncidentIncidentNotes.Count == 1)
            {
                foreach( IncidentIncidentNote _iin in _entity.IncidentIncidentNotes)
                {
                    _context.IncidentIncidentNotes.Remove(_iin);
                }
            }
            _context.IncidentNotes.Remove(_entity);
			await _context.SaveChangesAsync(cancellationToken);
            await Mediator.Send(new LogCreateCommand() {
                Level = LoggingLevel.Warning, Method = MethodBase.GetCurrentMethod(),
                Message = "Deleted IncidentNote: " + _entity.IncidentNoteToString(),
                Exception = null });
            // Return the row count affected.
            return 1;
		}
		//
		/// <summary>
		/// FluentValidation of the 'IncidentNoteDeleteCommand' class.
		/// </summary>
		public class Validator : AbstractValidator<IncidentNoteDeleteCommand>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'IncidentNoteDeleteCommand' validator.
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
	/// Custom IncidentNoteDeleteCommand record not found exception.
	/// </summary>
	public class DeleteCommandKeyNotFoundException: KeyNotFoundException
	{
		//
		/// <summary>
		/// Implementation of IncidentNoteDeleteCommand record not found exception.
		/// </summary>
		/// <param name="id">The key for the record.</param>
		public DeleteCommandKeyNotFoundException(long incidentNoteId)
			: base($"IncidentNoteDeleteCommand key not found exception: id: {incidentNoteId}")
		{
		}
	}
	//
	/// <summary>
	/// Custom IncidentNoteDeleteCommand validation exception.
	/// </summary>
	public class DeleteCommandValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of IncidentNoteDeleteCommand validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public DeleteCommandValidationException(string errorMessage)
			: base($"IncidentNoteDeleteCommand validation exception: errors: {errorMessage}")
		{
		}
	}
    //
    /// <summary>
    /// Custom IncidentNoteDeleteCommand validation exception.
    /// </summary>
    public class IncidentNoteDeleteCommandActiveTooManyIncidentsException : Exception
    {
        //
        /// <summary>
        /// Implementation of IncidentNoteDeleteCommand validation exception.
        /// </summary>
        /// <param name="errorMessage">The validation error messages.</param>
        public IncidentNoteDeleteCommandActiveTooManyIncidentsException(string errorMessage)
            : base($"IncidentNoteDeleteCommand contains active incidents on IncidentNotes exception: errors: {errorMessage}")
        {
        }
    }
    //
}
// ---------------------------------------------------------------------------
