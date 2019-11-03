
//
// ---------------------------------------------------------------------------
// IncidentType update command.
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
namespace NSG.WebSrv.Application.Commands.IncidentTypes
{
	//
	/// <summary>
	/// 'IncidentType' update command, handler and handle.
	/// </summary>
	public class IncidentTypeUpdateCommand : IRequest<int>
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
	/// 'IncidentType' update command handler.
	/// </summary>
	public class IncidentTypeUpdateCommandHandler : IRequestHandler<IncidentTypeUpdateCommand, int>
	{
		private readonly IDb_Context _context;
		//
		/// <summary>
		///  The constructor for the inner handler class, to update the IncidentType entity.
		/// </summary>
		/// <param name="context">The database interface context.</param>
		public IncidentTypeUpdateCommandHandler(IDb_Context context)
		{
			_context = context;
		}
		//
		/// <summary>
		/// 'IncidentType' command handle method, passing two interfaces.
		/// </summary>
		/// <param name="request">This update command request.</param>
		/// <param name="cancellationToken">Cancel token.</param>
		/// <returns>Returns the row count.</returns>
		public async Task<int> Handle(IncidentTypeUpdateCommand request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new UpdateCommandValidationException(_results.FluentValidationErrors());
			}
			var _entity = await _context.IncidentTypes
				.SingleOrDefaultAsync(r => r.IncidentTypeId == request.IncidentTypeId, cancellationToken);
			if (_entity == null)
			{
				throw new UpdateCommandKeyNotFoundException(request.IncidentTypeId);
			}
			// Move from update command class to entity class.
			_entity.IncidentTypeShortDesc = request.IncidentTypeShortDesc;
			_entity.IncidentTypeDesc = request.IncidentTypeDesc;
			_entity.IncidentTypeFromServer = request.IncidentTypeFromServer;
			_entity.IncidentTypeSubjectLine = request.IncidentTypeSubjectLine;
			_entity.IncidentTypeEmailTemplate = request.IncidentTypeEmailTemplate;
			_entity.IncidentTypeTimeTemplate = request.IncidentTypeTimeTemplate;
			_entity.IncidentTypeThanksTemplate = request.IncidentTypeThanksTemplate;
			_entity.IncidentTypeLogTemplate = request.IncidentTypeLogTemplate;
			_entity.IncidentTypeTemplate = request.IncidentTypeTemplate;
			//
			await _context.SaveChangesAsync(cancellationToken);
			// Return the row count.
			return 1;
		}
		//
		/// <summary>
		/// FluentValidation of the 'IncidentTypeUpdateCommand' class.
		/// </summary>
		public class Validator : AbstractValidator<IncidentTypeUpdateCommand>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'IncidentTypeUpdateCommand' validator.
			/// </summary>
			public Validator()
			{
				//
				RuleFor(x => x.IncidentTypeId).NotNull();
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
	/// Custom IncidentTypeUpdateCommand record not found exception.
	/// </summary>
	public class UpdateCommandKeyNotFoundException: KeyNotFoundException
	{
		//
		/// <summary>
		/// Implementation of IncidentTypeUpdateCommand record not found exception.
		/// </summary>
		/// <param name="id">The key for the record.</param>
		public UpdateCommandKeyNotFoundException(int incidentTypeId)
			: base($"IncidentTypeUpdateCommand key not found exception: id: {incidentTypeId}")
		{
		}
	}
	//
	/// <summary>
	/// Custom IncidentTypeUpdateCommand validation exception.
	/// </summary>
	public class UpdateCommandValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of IncidentTypeUpdateCommand validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public UpdateCommandValidationException(string errorMessage)
			: base($"IncidentTypeUpdateCommand validation exception: errors: {errorMessage}")
		{
		}
	}
}
// ---------------------------------------------------------------------------

