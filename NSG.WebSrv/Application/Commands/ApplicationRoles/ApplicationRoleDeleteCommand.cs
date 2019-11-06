//
// ---------------------------------------------------------------------------
// ApplicationRoles delete command.
//
using System;
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
using System.Reflection;
//
namespace NSG.WebSrv.Application.Commands.ApplicationRoles
{
	//
	/// <summary>
	/// 'ApplicationRole' delete command, handler and handle.
	/// </summary>
	public class ApplicationRoleDeleteCommand : IRequest<int>
	{
		public string Id { get; set; }
	}
	//
	/// <summary>
	/// 'ApplicationRole' delete command handler.
	/// </summary>
	public class ApplicationRoleDeleteCommandHandler : IRequestHandler<ApplicationRoleDeleteCommand, int>
	{
        private RoleManager<ApplicationRole> _roleManager;
        protected IMediator Mediator;
        //
        /// <summary>
        ///  The constructor for the inner handler class, to delete the ApplicationRole entity.
        /// </summary>
        /// <param name="context">The database interface context.</param>
        public ApplicationRoleDeleteCommandHandler(RoleManager<ApplicationRole> roleManager, IMediator mediator)
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
        public async Task<int> Handle(ApplicationRoleDeleteCommand request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new DeleteCommandValidationException(_results.FluentValidationErrors());
			}
            ApplicationRole _entity = await _roleManager.Roles
                .Include(u => u.UserRoles).ThenInclude(ur => ur.User)
                .FirstOrDefaultAsync(r => r.Id == request.Id);
            if (_entity == null)
			{
				throw new ApplicationRoleDeleteCommandKeyNotFoundException(request.Id);
			}
            // require user to delete all servers before deleting company.
            if (_entity.UserRoles.Count > 0)
            {
                throw new ApplicationRoleDeleteCommandActiveUsersException(
                    string.Format("ApplicationUser count: {0}", _entity.UserRoles.Count));
            }
            //
            var result = await _roleManager.DeleteAsync(_entity);
            await Mediator.Send(new LogCreateCommand(
                LoggingLevel.Warning, MethodBase.GetCurrentMethod(),
                "Deleted Role: " + _entity.ApplicationRoleToString(), null));
            // Return the row count affected.
            return 1;
		}
		//
		/// <summary>
		/// FluentValidation of the 'ApplicationRoleDeleteCommand' class.
		/// </summary>
		public class Validator : AbstractValidator<ApplicationRoleDeleteCommand>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'ApplicationRoleDeleteCommand' validator.
			/// </summary>
			public Validator()
			{
				//
				RuleFor(x => x.Id).NotEmpty().MaximumLength(128);
				//
			}
			//
		}
		//
	}
	//
	/// <summary>
	/// Custom ApplicationRoleDeleteCommand record not found exception.
	/// </summary>
	public class ApplicationRoleDeleteCommandKeyNotFoundException : KeyNotFoundException
	{
		//
		/// <summary>
		/// Implementation of ApplicationRoleDeleteCommand record not found exception.
		/// </summary>
		/// <param name="id">The key for the record.</param>
		public ApplicationRoleDeleteCommandKeyNotFoundException(string id)
			: base($"ApplicationRoleDeleteCommand key not found exception: id: {id}")
		{
		}
	}
    //
    /// <summary>
    /// Custom ApplicationRoleDeleteCommand record not found exception.
    /// </summary>
    public class ApplicationRoleDeleteCommandActiveUsersException : Exception
    {
        //
        /// <summary>
        /// Implementation of ApplicationRoleDeleteCommand record not found exception.
        /// </summary>
        /// <param name="id">The key for the record.</param>
        public ApplicationRoleDeleteCommandActiveUsersException(string message)
            : base($"ApplicationRoleDeleteCommand contains active users on ApplicationRole validation exception: {message}")
        {
        }
    }
    //
    /// <summary>
    /// Custom ApplicationRoleDeleteCommand validation exception.
    /// </summary>
    public class DeleteCommandValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of ApplicationRoleDeleteCommand validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public DeleteCommandValidationException(string errorMessage)
			: base($"ApplicationRoleDeleteCommand validation exception: errors: {errorMessage}")
		{
		}
	}
    //
}
// ---------------------------------------------------------------------------

