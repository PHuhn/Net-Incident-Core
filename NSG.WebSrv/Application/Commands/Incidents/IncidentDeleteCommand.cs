//
// ---------------------------------------------------------------------------
// Incident delete command.
//
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
//
using Microsoft.EntityFrameworkCore;
using MediatR;
using FluentValidation;
using FluentValidation.Results;
//
using NSG.WebSrv.Domain.Entities;
using NSG.WebSrv.Application.Commands.IncidentNotes;
using NSG.WebSrv.Application.Commands.Logs;
using NSG.WebSrv.Infrastructure.Common;
//
namespace NSG.WebSrv.Application.Commands.Incidents
{
	//
	/// <summary>
	/// 'Incident' delete command, handler and handle.
	/// </summary>
	public class IncidentDeleteCommand : IRequest<int>
	{
		public long IncidentId { get; set; }
	}
	//
	/// <summary>
	/// 'Incident' delete command handler.
	/// </summary>
	public class IncidentDeleteCommandHandler : IRequestHandler<IncidentDeleteCommand, int>
	{
		private readonly IDb_Context _context;
        protected IMediator Mediator;
        private IApplication _application;
        //
        /// <summary>
        ///  The constructor for the inner handler class, to delete the Incident entity.
        /// </summary>
        /// <param name="context">The database interface context.</param>
        public IncidentDeleteCommandHandler(IDb_Context context, IMediator mediator, IApplication application)
		{
			_context = context;
            Mediator = mediator;
            _application = application;
        }
        //
        /// <summary>
        /// 'Incident' command handle method, passing two interfaces.
        /// </summary>
        /// <param name="request">This update command request.</param>
        /// <param name="cancellationToken">Cancel token.</param>
        /// <returns>Returns the row count.</returns>
        public async Task<int> Handle(IncidentDeleteCommand request, CancellationToken cancellationToken)
		{
            if (_application.IsCompanyAdmin() == false)
            {
                throw new IncidentDeleteCommandPermissionsException("user not company/admin group.");
            }
            int _return = 0;
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new IncidentDeleteCommandValidationException(_results.FluentValidationErrors());
			}
			var _entity = await _context.Incidents
                .Include(i => i.IncidentIncidentNotes)
				.SingleOrDefaultAsync(r => r.IncidentId == request.IncidentId, cancellationToken);
			if (_entity == null)
			{
				throw new DeleteCommandKeyNotFoundException(request.IncidentId);
			}
            //
            List<NetworkLog> _logs = _context.NetworkLogs.Where(_r => _r.IncidentId == request.IncidentId).ToList();
            foreach (NetworkLog _log in _logs)
            {
                _log.IncidentId = null;
                _return++;
            }
            List<IncidentIncidentNote> _notes = _entity.IncidentIncidentNotes.ToList();
            foreach (IncidentIncidentNote _note in _notes)
            {
                int _count = await Mediator.Send(new IncidentNoteDeleteCommand() { IncidentNoteId = _note.IncidentNoteId });
                if(_count == 0)
                {
                    // multiply linked
                    _context.IncidentNotes.Remove(_note.IncidentNote);
                    _context.IncidentIncidentNotes.Remove(_note);
                }
                _return++;
            }
            _return++;      // one row updated
            _context.Incidents.Remove(_entity);
            await _context.SaveChangesAsync(cancellationToken);
            await Mediator.Send(new LogCreateCommand(
                LoggingLevel.Warning, MethodBase.GetCurrentMethod(),
                "Deleted Incident: " + _entity.IncidentToString(), null ));
            // Return the row count affected.
            return _return;
		}
		//
		/// <summary>
		/// FluentValidation of the 'IncidentDeleteCommand' class.
		/// </summary>
		public class Validator : AbstractValidator<IncidentDeleteCommand>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'IncidentDeleteCommand' validator.
			/// </summary>
			public Validator()
			{
				//
				RuleFor(x => x.IncidentId).NotNull();
				//
			}
			//
		}
		//
	}
	//
	/// <summary>
	/// Custom IncidentDeleteCommand record not found exception.
	/// </summary>
	public class DeleteCommandKeyNotFoundException: KeyNotFoundException
	{
		//
		/// <summary>
		/// Implementation of IncidentDeleteCommand record not found exception.
		/// </summary>
		/// <param name="id">The key for the record.</param>
		public DeleteCommandKeyNotFoundException(long incidentId)
			: base($"IncidentDeleteCommand key not found exception: id: {incidentId}")
		{
		}
	}
	//
	/// <summary>
	/// Custom IncidentDeleteCommand validation exception.
	/// </summary>
	public class IncidentDeleteCommandValidationException : Exception
	{
		//
		/// <summary>
		/// Implementation of IncidentDeleteCommand validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public IncidentDeleteCommandValidationException(string errorMessage)
			: base($"IncidentDeleteCommand validation exception: errors: {errorMessage}")
		{
		}
	}
    //
    /// <summary>
    /// Custom IncidentDeleteCommand permissions exception.
    /// </summary>
    public class IncidentDeleteCommandPermissionsException : Exception
    {
        //
        /// <summary>
        /// Implementation of IncidentDeleteCommand permissions exception.
        /// </summary>
        /// <param name="errorMessage">The permissions error messages.</param>
        public IncidentDeleteCommandPermissionsException(string errorMessage)
            : base($"IncidentDeleteCommand permissions exception: {errorMessage}")
        {
        }
    }
    //
}
// ---------------------------------------------------------------------------
