//
// ---------------------------------------------------------------------------
// EmailTemplates delete command.
//
using System;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
//
using Microsoft.EntityFrameworkCore;
using MediatR;
using FluentValidation;
using FluentValidation.Results;
using NSG.WebSrv.Domain.Entities;
using NSG.WebSrv.Application.Commands.Logs;
//
namespace NSG.WebSrv.Application.Commands.CompanyEmailTemplates
{
	//
	/// <summary>
	/// 'EmailTemplate' delete command, handler and handle.
	/// </summary>
	public class CompanyEmailTemplateDeleteCommand : IRequest<int>
	{
		public int CompanyId { get; set; }
		public int IncidentTypeId { get; set; }
	}
	//
	/// <summary>
	/// 'EmailTemplate' delete command handler.
	/// </summary>
	public class CompanyEmailTemplateDeleteCommandHandler : IRequestHandler<CompanyEmailTemplateDeleteCommand, int>
	{
		private readonly IDb_Context _context;
        protected IMediator Mediator;
        //
        /// <summary>
        ///  The constructor for the inner handler class, to delete the EmailTemplate entity.
        /// </summary>
        /// <param name="context">The database interface context.</param>
        public CompanyEmailTemplateDeleteCommandHandler(IDb_Context context, IMediator mediator)
		{
			_context = context;
            Mediator = mediator;
        }
        //
        /// <summary>
        /// 'EmailTemplate' command handle method, passing two interfaces.
        /// </summary>
        /// <param name="request">This update command request.</param>
        /// <param name="cancellationToken">Cancel token.</param>
        /// <returns>Returns the row count.</returns>
        public async Task<int> Handle(CompanyEmailTemplateDeleteCommand request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new DeleteCommandValidationException(_results.FluentValidationErrors());
			}
			var _entity = await _context.EmailTemplates
				.SingleOrDefaultAsync(r => r.CompanyId == request.CompanyId && r.IncidentTypeId == request.IncidentTypeId, cancellationToken);
			if (_entity == null)
			{
				throw new DeleteCommandKeyNotFoundException(request.CompanyId, request.IncidentTypeId);
			}
			//
			_context.EmailTemplates.Remove(_entity);
			await _context.SaveChangesAsync(cancellationToken);
            // Log what was deleted
            await Mediator.Send(new LogCreateCommand() {
                Level = LoggingLevel.Warning, Method = MethodBase.GetCurrentMethod(),
                Message = "Deleted CompanyEmailTemplate: " + _entity.EmailTemplateToString(),
                Exception = null });
            // Return the row count affected.
            return 1;
		}
		//
		/// <summary>
		/// FluentValidation of the 'EmailTemplateDeleteCommand' class.
		/// </summary>
		public class Validator : AbstractValidator<CompanyEmailTemplateDeleteCommand>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'EmailTemplateDeleteCommand' validator.
			/// </summary>
			public Validator()
			{
				//
				RuleFor(x => x.CompanyId).NotNull();
				RuleFor(x => x.IncidentTypeId).NotNull();
				//
			}
			//
		}
		//
	}
	//
	/// <summary>
	/// Custom EmailTemplateDeleteCommand record not found exception.
	/// </summary>
	public class DeleteCommandKeyNotFoundException: KeyNotFoundException
	{
		//
		/// <summary>
		/// Implementation of EmailTemplateDeleteCommand record not found exception.
		/// </summary>
		/// <param name="id">The key for the record.</param>
		public DeleteCommandKeyNotFoundException(int companyId, int incidentTypeId)
			: base($"EmailTemplateDeleteCommand key not found exception: id: {companyId}-{incidentTypeId}")
		{
		}
	}
	//
	/// <summary>
	/// Custom EmailTemplateDeleteCommand validation exception.
	/// </summary>
	public class DeleteCommandValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of EmailTemplateDeleteCommand validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public DeleteCommandValidationException(string errorMessage)
			: base($"EmailTemplateDeleteCommand validation exception: errors: {errorMessage}")
		{
		}
	}
    //
}
// ---------------------------------------------------------------------------
