//
// ---------------------------------------------------------------------------
// IncidentType delete command.
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
namespace NSG.WebSrv.Application.Commands.IncidentTypes
{
	//
	/// <summary>
	/// 'IncidentType' delete command, handler and handle.
	/// </summary>
	public class IncidentTypeDeleteCommand : IRequest<int>
	{
		public int IncidentTypeId { get; set; }
	}
	//
	/// <summary>
	/// 'IncidentType' delete command handler.
	/// </summary>
	public class IncidentTypeDeleteCommandHandler : IRequestHandler<IncidentTypeDeleteCommand, int>
	{
		private readonly IDb_Context _context;
        protected IMediator Mediator;
        //
        /// <summary>
        ///  The constructor for the inner handler class, to delete the IncidentType entity.
        /// </summary>
        /// <param name="context">The database interface context.</param>
        public IncidentTypeDeleteCommandHandler(IDb_Context context, IMediator mediator)
		{
			_context = context;
            Mediator = mediator;
        }
		//
		/// <summary>
		/// 'IncidentType' command handle method, passing two interfaces.
		/// </summary>
		/// <param name="request">This update command request.</param>
		/// <param name="cancellationToken">Cancel token.</param>
		/// <returns>Returns the row count.</returns>
		public async Task<int> Handle(IncidentTypeDeleteCommand request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new DeleteCommandValidationException(_results.FluentValidationErrors());
			}
			var _entity = await _context.IncidentTypes
                .Include(_it => _it.NetworkLogs)
				.SingleOrDefaultAsync(r => r.IncidentTypeId == request.IncidentTypeId, cancellationToken);
			if (_entity == null)
			{
				throw new DeleteCommandKeyNotFoundException(request.IncidentTypeId);
			}
            // require user to delete all servers before deleting company.
            if (_entity.NetworkLogs.Count > 0)
            {
                throw new IncidentTypeDeleteCommandActiveNetworkLogsException(
                    string.Format("NetworkLogs count: {0}", _entity.NetworkLogs.Count));
            }
            //
            _context.IncidentTypes.Remove(_entity);
			await _context.SaveChangesAsync(cancellationToken);
            // Log what was deleted
            await Mediator.Send(new LogCreateCommand(
                LoggingLevel.Warning, MethodBase.GetCurrentMethod(),
                "Deleted IncidentType: " + _entity.IncidentTypeToString(), null));
            // Return the row count affected.
            return 1;
		}
		//
		/// <summary>
		/// FluentValidation of the 'IncidentTypeDeleteCommand' class.
		/// </summary>
		public class Validator : AbstractValidator<IncidentTypeDeleteCommand>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'IncidentTypeDeleteCommand' validator.
			/// </summary>
			public Validator()
			{
				//
				RuleFor(x => x.IncidentTypeId).NotNull();
				//
			}
			//
		}
		//
	}
	//
	/// <summary>
	/// Custom IncidentTypeDeleteCommand record not found exception.
	/// </summary>
	public class DeleteCommandKeyNotFoundException: KeyNotFoundException
	{
		//
		/// <summary>
		/// Implementation of IncidentTypeDeleteCommand record not found exception.
		/// </summary>
		/// <param name="id">The key for the record.</param>
		public DeleteCommandKeyNotFoundException(int incidentTypeId)
			: base($"IncidentTypeDeleteCommand key not found exception: id: {incidentTypeId}")
		{
		}
	}
	//
	/// <summary>
	/// Custom IncidentTypeDeleteCommand validation exception.
	/// </summary>
	public class DeleteCommandValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of IncidentTypeDeleteCommand validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public DeleteCommandValidationException(string errorMessage)
			: base($"IncidentTypeDeleteCommand validation exception: errors: {errorMessage}")
		{
		}
	}
    //
    /// <summary>
    /// Custom IncidentTypeDeleteCommand validation exception.
    /// </summary>
    public class IncidentTypeDeleteCommandActiveNetworkLogsException : Exception
    {
        //
        /// <summary>
        /// Implementation of IncidentTypeDeleteCommand validation exception.
        /// </summary>
        /// <param name="errorMessage">The validation error messages.</param>
        public IncidentTypeDeleteCommandActiveNetworkLogsException(string errorMessage)
            : base($"IncidentTypeDeleteCommand contains active note types on NetworkLogs validation exception: errors: {errorMessage}")
        {
        }
    }
    //
}
// ---------------------------------------------------------------------------
