//
// ---------------------------------------------------------------------------
// ApplicationRoles update command.
//
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
//
using MediatR;
using FluentValidation;
using FluentValidation.Results;
//
using NSG.WebSrv.Domain.Entities;
using NSG.WebSrv.Application.Commands.Logs;
//
namespace NSG.WebSrv.Application.Commands.ApplicationRoles
{
	//
	/// <summary>
	/// 'ApplicationRole' update command, handler and handle.
	/// </summary>
	public class ApplicationRoleUpdateCommand : IRequest<int>
	{
		public string Id { get; set; }
		public string Name { get; set; }
	}
	//
	/// <summary>
	/// 'ApplicationRole' update command handler.
	/// </summary>
	public class ApplicationRoleUpdateCommandHandler : IRequestHandler<ApplicationRoleUpdateCommand, int>
	{
		//private readonly IDb_Context _context;
        private RoleManager<ApplicationRole> _roleManager;
        protected IMediator Mediator;
        //
        /// <summary>
        ///  The constructor for the inner handler class, to update the ApplicationRole entity.
        /// </summary>
        /// <param name="context">The database interface context.</param>
        public ApplicationRoleUpdateCommandHandler(RoleManager<ApplicationRole> roleManager, IMediator mediator)
        {
            _roleManager = roleManager;
            Mediator = mediator;
        }
		//
		/// <summary>
		/// 'ApplicationRole' command handle method, passing two interfaces.
		/// </summary>
		/// <param name="request">This update command request.</param>
		/// <param name="cancellationToken">Cancel token.</param>
		/// <returns>Returns the row count.</returns>
		public async Task<int> Handle(ApplicationRoleUpdateCommand request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new UpdateCommandValidationException(_results.FluentValidationErrors());
			}
			var _entity = await _roleManager.Roles
				.SingleOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
			if (_entity == null)
			{
				throw new UpdateCommandKeyNotFoundException(request.Id);
			}
            string _updateMessage = $"Name from: {_entity.Name}, to: {request.Name}";
			// Move from update command class to entity class.
			_entity.Name = request.Name;
            //
            await _roleManager.UpdateAsync(_entity);
            await Mediator.Send(new LogCreateCommand() {
                Level = LoggingLevel.Warning, Method = MethodBase.GetCurrentMethod(),
                Message = "Updated Role: " + _updateMessage, Exception = null });
            // Return the row count.
            return 1;
		}
		//
		/// <summary>
		/// FluentValidation of the 'ApplicationRoleUpdateCommand' class.
		/// </summary>
		public class Validator : AbstractValidator<ApplicationRoleUpdateCommand>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'ApplicationRoleUpdateCommand' validator.
			/// </summary>
			public Validator()
			{
				//
				RuleFor(x => x.Id).NotEmpty().MaximumLength(128);
				RuleFor(x => x.Name).MaximumLength(256);
				//
			}
			//
		}
		//
	}
	//
	/// <summary>
	/// Custom ApplicationRoleUpdateCommand record not found exception.
	/// </summary>
	public class UpdateCommandKeyNotFoundException: KeyNotFoundException
	{
		//
		/// <summary>
		/// Implementation of ApplicationRoleUpdateCommand record not found exception.
		/// </summary>
		/// <param name="id">The key for the record.</param>
		public UpdateCommandKeyNotFoundException(string id)
			: base($"ApplicationRoleUpdateCommand key not found exception: id: {id}")
		{
		}
	}
	//
	/// <summary>
	/// Custom ApplicationRoleUpdateCommand validation exception.
	/// </summary>
	public class UpdateCommandValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of ApplicationRoleUpdateCommand validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public UpdateCommandValidationException(string errorMessage)
			: base($"ApplicationRoleUpdateCommand validation exception: errors: {errorMessage}")
		{
		}
	}
}
// ---------------------------------------------------------------------------

