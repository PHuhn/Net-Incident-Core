//
// ---------------------------------------------------------------------------
// NIC delete command.
//
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
//
using MediatR;
using FluentValidation;
using FluentValidation.Results;
//
using NSG.WebSrv.Domain.Entities;
using NSG.WebSrv.Application.Commands.Logs;
using System.Reflection;
using System.Text;
//
namespace NSG.WebSrv.Application.Commands.NICs
{
	//
	/// <summary>
	/// 'NIC' delete command, handler and handle.
	/// </summary>
	public class NICDeleteCommand : IRequest<int>
	{
		public string NIC_Id { get; set; }
	}
	//
	/// <summary>
	/// 'NIC' delete command handler.
	/// </summary>
	public class NICDeleteCommandHandler : IRequestHandler<NICDeleteCommand, int>
	{
		private readonly IDb_Context _context;
        protected IMediator Mediator;
        //
        /// <summary>
        ///  The constructor for the inner handler class, to delete the NIC entity.
        /// </summary>
        /// <param name="context">The database interface context.</param>
        public NICDeleteCommandHandler(IDb_Context context, IMediator mediator)
		{
			_context = context;
            Mediator = mediator;
        }
        //
        /// <summary>
        /// 'NIC' command handle method, passing two interfaces.
        /// </summary>
        /// <param name="request">This update command request.</param>
        /// <param name="cancellationToken">Cancel token.</param>
        /// <returns>Returns the row count.</returns>
        public async Task<int> Handle(NICDeleteCommand request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new DeleteCommandValidationException(_results.FluentValidationErrors());
			}
			var _entity = await _context.NICs
                .Include( _n => _n.Incidents )
				.SingleOrDefaultAsync(r => r.NIC_Id == request.NIC_Id, cancellationToken);
			if (_entity == null)
			{
				throw new DeleteCommandKeyNotFoundException(request.NIC_Id);
			}
            // require user to delete all incidents before deleting company.
            if (_entity.Incidents.Count > 0)
            {

                throw new NICDeleteCommandActiveIncidentNotesException(
                    string.Format("Incident count: {0}", _entity.Incidents.Count));
            }
            //
            _context.NICs.Remove(_entity);
			await _context.SaveChangesAsync(cancellationToken);
            await Mediator.Send(new LogCreateCommand() {
                Level = LoggingLevel.Warning, Method = MethodBase.GetCurrentMethod(),
                Message = "Deleted Commpany : " + _entity.NICToString(), Exception = null });
            // Return the row count affected.
            return 1;
		}
		//
		/// <summary>
		/// FluentValidation of the 'NICDeleteCommand' class.
		/// </summary>
		public class Validator : AbstractValidator<NICDeleteCommand>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'NICDeleteCommand' validator.
			/// </summary>
			public Validator()
			{
				//
				RuleFor(x => x.NIC_Id).NotEmpty().MaximumLength(16);
				//
			}
			//
		}
		//
	}
	//
	/// <summary>
	/// Custom NICDeleteCommand record not found exception.
	/// </summary>
	public class DeleteCommandKeyNotFoundException: KeyNotFoundException
	{
		//
		/// <summary>
		/// Implementation of NICDeleteCommand record not found exception.
		/// </summary>
		/// <param name="id">The key for the record.</param>
		public DeleteCommandKeyNotFoundException(string nIC_Id)
			: base($"NICDeleteCommand key not found exception: id: {nIC_Id}")
		{
		}
	}
	//
	/// <summary>
	/// Custom NICDeleteCommand validation exception.
	/// </summary>
	public class DeleteCommandValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of NICDeleteCommand validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public DeleteCommandValidationException(string errorMessage)
			: base($"NICDeleteCommand validation exception: errors: {errorMessage}")
		{
		}
	}
    //
    /// <summary>
    /// Custom NICDeleteCommand validation exception.
    /// </summary>
    public class NICDeleteCommandActiveIncidentNotesException : Exception
    {
        //
        /// <summary>
        /// Implementation of ServerDeleteCommand validation exception.
        /// </summary>
        /// <param name="errorMessage">The validation error messages.</param>
        public NICDeleteCommandActiveIncidentNotesException(string errorMessage)
            : base($"NICDeleteCommand contains active incidents on NICs validation exception: errors: {errorMessage}")
        {
        }
    }
    //
}
// ---------------------------------------------------------------------------
