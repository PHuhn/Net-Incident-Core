
//
// ---------------------------------------------------------------------------
// IncidentType create command.
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
namespace NSG.WebSrv.Application.Commands.IncidentTypes
{
	//
	/// <summary>
	/// 'IncidentType' create command, handler and handle.
	/// </summary>
	public class IncidentTypeCreateCommand : IRequest<IncidentType>
	{
		public int IncidentTypeId { get; set; }
		public string IncidentTypeShortDesc { get; set; }
		public string IncidentTypeDesc { get; set; }
		public bool IncidentTypeFromServer { get; set; }
		public string IncidentTypeSubjectLine { get; set; }
		public string IncidentTypeEmailTemplate { get; set; }
		public string IncidentTypeTimeTemplate { get; set; }
		public string IncidentTypeThanksTemplate { get; set; }
		public string IncidentTypeLogTemplate { get; set; }
		public string IncidentTypeTemplate { get; set; }
	}
	//
	/// <summary>
	/// 'IncidentType' create command handler.
	/// </summary>
	public class IncidentTypeCreateCommandHandler : IRequestHandler<IncidentTypeCreateCommand, IncidentType>
	{
		private readonly IDb_Context _context;
		//
		//
		/// <summary>
		///  The constructor for the inner handler class, to create the IncidentType entity.
		/// </summary>
		/// <param name="context">The database interface context.</param>
		public IncidentTypeCreateCommandHandler(IDb_Context context)
		{
			_context = context;
		}
		//
		/// <summary>
		/// 'IncidentType' command handle method, passing two interfaces.
		/// </summary>
		/// <param name="request">This create command request.</param>
		/// <param name="cancellationToken">Cancel token.</param>
		/// <returns>The IncidentType entity class.</returns>
		public async Task<IncidentType> Handle(IncidentTypeCreateCommand request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new CreateCommandValidationException(_results.FluentValidationErrors());
			}
			// Move from create command class to entity class.
			var _entity = new IncidentType
			{
				IncidentTypeId = request.IncidentTypeId,
				IncidentTypeShortDesc = request.IncidentTypeShortDesc,
				IncidentTypeDesc = request.IncidentTypeDesc,
				IncidentTypeFromServer = request.IncidentTypeFromServer,
				IncidentTypeSubjectLine = request.IncidentTypeSubjectLine,
				IncidentTypeEmailTemplate = request.IncidentTypeEmailTemplate,
				IncidentTypeTimeTemplate = request.IncidentTypeTimeTemplate,
				IncidentTypeThanksTemplate = request.IncidentTypeThanksTemplate,
				IncidentTypeLogTemplate = request.IncidentTypeLogTemplate,
				IncidentTypeTemplate = request.IncidentTypeTemplate,
			};
			_context.IncidentTypes.Add(_entity);
			await _context.SaveChangesAsync(cancellationToken);
			// Return the entity class.
			return _entity;
		}
		//
		/// <summary>
		/// FluentValidation of the 'IncidentTypeCreateCommand' class.
		/// </summary>
		public class Validator : AbstractValidator<IncidentTypeCreateCommand>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'IncidentTypeCreateCommand' validator.
			/// </summary>
			public Validator()
			{
				//
				RuleFor(x => x.IncidentTypeShortDesc).NotEmpty().MaximumLength(8);
				RuleFor(x => x.IncidentTypeDesc).NotEmpty().MaximumLength(50);
				RuleFor(x => x.IncidentTypeFromServer).NotNull();
				RuleFor(x => x.IncidentTypeSubjectLine).NotEmpty().MaximumLength(1073741823);
				RuleFor(x => x.IncidentTypeEmailTemplate).NotEmpty().MaximumLength(1073741823);
				RuleFor(x => x.IncidentTypeTimeTemplate).NotEmpty().MaximumLength(1073741823);
				RuleFor(x => x.IncidentTypeThanksTemplate).NotEmpty().MaximumLength(1073741823);
				RuleFor(x => x.IncidentTypeLogTemplate).NotEmpty().MaximumLength(1073741823);
				RuleFor(x => x.IncidentTypeTemplate).NotEmpty().MaximumLength(1073741823);
				//
			}
			//
		}
		//
	}
	//
	/// <summary>
	/// Custom IncidentTypeCreateCommand validation exception.
	/// </summary>
	public class CreateCommandValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of IncidentTypeCreateCommand validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public CreateCommandValidationException(string errorMessage)
			: base($"IncidentTypeCreateCommand validation exception: errors: {errorMessage}")
		{
		}
	}
	//
}
// ---------------------------------------------------------------------------

