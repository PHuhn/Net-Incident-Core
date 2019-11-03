//
// ---------------------------------------------------------------------------
// EmailTemplates update command.
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
namespace NSG.WebSrv.Application.Commands.CompanyEmailTemplates
{
	//
	/// <summary>
	/// 'EmailTemplate' update command, handler and handle.
	/// </summary>
	public class CompanyEmailTemplateUpdateCommand : IRequest<int>
	{
		public int CompanyId { get; set; }
		public int IncidentTypeId { get; set; }
		public string SubjectLine { get; set; }
		public string EmailBody { get; set; }
		public string TimeTemplate { get; set; }
		public string ThanksTemplate { get; set; }
		public string LogTemplate { get; set; }
		public string Template { get; set; }
		public bool FromServer { get; set; }
	}
	//
	/// <summary>
	/// 'EmailTemplate' update command handler.
	/// </summary>
	public class CompanyEmailTemplateUpdateCommandHandler : IRequestHandler<CompanyEmailTemplateUpdateCommand, int>
	{
		private readonly IDb_Context _context;
		//
		/// <summary>
		///  The constructor for the inner handler class, to update the EmailTemplate entity.
		/// </summary>
		/// <param name="context">The database interface context.</param>
		public CompanyEmailTemplateUpdateCommandHandler(IDb_Context context)
		{
			_context = context;
		}
		//
		/// <summary>
		/// 'EmailTemplate' command handle method, passing two interfaces.
		/// </summary>
		/// <param name="request">This update command request.</param>
		/// <param name="cancellationToken">Cancel token.</param>
		/// <returns>Returns the row count.</returns>
		public async Task<int> Handle(CompanyEmailTemplateUpdateCommand request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new UpdateCommandValidationException(_results.FluentValidationErrors());
			}
			var _entity = await _context.EmailTemplates
				.SingleOrDefaultAsync(r => r.CompanyId == request.CompanyId && r.IncidentTypeId == request.IncidentTypeId, cancellationToken);
			if (_entity == null)
			{
				throw new UpdateCommandKeyNotFoundException(request.CompanyId, request.IncidentTypeId);
			}
			// Move from update command class to entity class.
			_entity.SubjectLine = request.SubjectLine;
			_entity.EmailBody = request.EmailBody;
			_entity.TimeTemplate = request.TimeTemplate;
			_entity.ThanksTemplate = request.ThanksTemplate;
			_entity.LogTemplate = request.LogTemplate;
			_entity.Template = request.Template;
			_entity.FromServer = request.FromServer;
			//
			await _context.SaveChangesAsync(cancellationToken);
			// Return the row count.
			return 1;
		}
		//
		/// <summary>
		/// FluentValidation of the 'EmailTemplateUpdateCommand' class.
		/// </summary>
		public class Validator : AbstractValidator<CompanyEmailTemplateUpdateCommand>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'EmailTemplateUpdateCommand' validator.
			/// </summary>
			public Validator()
			{
				//
				RuleFor(x => x.CompanyId).NotNull();
				RuleFor(x => x.IncidentTypeId).NotNull();
				RuleFor(x => x.SubjectLine).NotEmpty().MaximumLength(1073741823);
				RuleFor(x => x.EmailBody).NotEmpty().MaximumLength(1073741823);
				RuleFor(x => x.TimeTemplate).NotEmpty().MaximumLength(1073741823);
				RuleFor(x => x.ThanksTemplate).NotEmpty().MaximumLength(1073741823);
				RuleFor(x => x.LogTemplate).NotEmpty().MaximumLength(1073741823);
				RuleFor(x => x.Template).NotEmpty().MaximumLength(1073741823);
				RuleFor(x => x.FromServer).NotNull();
				//
			}
			//
		}
		//
	}
	//
	/// <summary>
	/// Custom EmailTemplateUpdateCommand record not found exception.
	/// </summary>
	public class UpdateCommandKeyNotFoundException: KeyNotFoundException
	{
		//
		/// <summary>
		/// Implementation of EmailTemplateUpdateCommand record not found exception.
		/// </summary>
		/// <param name="id">The key for the record.</param>
		public UpdateCommandKeyNotFoundException(int companyId, int incidentTypeId)
			: base($"EmailTemplateUpdateCommand key not found exception: id: {companyId}-{incidentTypeId}")
		{
		}
	}
	//
	/// <summary>
	/// Custom EmailTemplateUpdateCommand validation exception.
	/// </summary>
	public class UpdateCommandValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of EmailTemplateUpdateCommand validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public UpdateCommandValidationException(string errorMessage)
			: base($"EmailTemplateUpdateCommand validation exception: errors: {errorMessage}")
		{
		}
        //
	}
    //
}
// ---------------------------------------------------------------------------
