//
// ---------------------------------------------------------------------------
// ApplicationRoles create command.
//
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
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
	/// 'ApplicationRole' create command, handler and handle.
	/// </summary>
	public class ApplicationRoleCreateCommand : IRequest<ApplicationRole>
	{
		public string Id { get; set; }
		public string Name { get; set; }
	}
	//
	/// <summary>
	/// 'ApplicationRole' create command handler.
	/// </summary>
	public class ApplicationRoleCreateCommandHandler : IRequestHandler<ApplicationRoleCreateCommand, ApplicationRole>
	{
        private RoleManager<ApplicationRole> _roleManager;
        protected IMediator Mediator;
        //
        /// <summary>
        ///  The constructor for the inner handler class, to create the ApplicationRole entity.
        /// </summary>
        /// <param name="context">The database interface context.</param>
        public ApplicationRoleCreateCommandHandler(RoleManager<ApplicationRole> roleManager, IMediator mediator)
        {
            _roleManager = roleManager;
            Mediator = mediator;
        }
        //
        /// <summary>
        /// 'ApplicationRole' command handle method, passing two interfaces.
        /// </summary>
        /// <param name="request">This create command request.</param>
        /// <param name="cancellationToken">Cancel token.</param>
        /// <returns>The ApplicationRole entity class.</returns>
        public async Task<ApplicationRole> Handle(ApplicationRoleCreateCommand request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new ApplicationRoleCreateCommandValidationException(_results.FluentValidationErrors());
			}
            var _duplicate = await _roleManager.FindByIdAsync(request.Id);
			if (_duplicate != null)
			{
				throw new ApplicationRoleCreateCommandDuplicateException(request.Id);
			}
            // Move from create command class to entity class.
            var _entity = new ApplicationRole
			{
				Id = request.Id,
				Name = request.Name
			};
            var _roleresult = await _roleManager.CreateAsync(_entity);
            if (_roleresult.Succeeded)
            {
                await Mediator.Send(new LogCreateCommand(
                    LoggingLevel.Warning, MethodBase.GetCurrentMethod(),
                    "Created Role: " + _entity.ApplicationRoleToString(), null));
                // Return the entity class.
                return _entity;
            }
            else
            {
                string _errMsg = "";
                foreach(IdentityError _err in _roleresult.Errors)
                {
                    _errMsg += _err.Description + ", ";
                }
                throw new ApplicationRoleCreateCommandRoleManagerException(_errMsg);
            }
        }
		//
		/// <summary>
		/// FluentValidation of the 'ApplicationRoleCreateCommand' class.
		/// </summary>
		public class Validator : AbstractValidator<ApplicationRoleCreateCommand>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'ApplicationRoleCreateCommand' validator.
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
	/// Custom ApplicationRoleCreateCommand duplicate exception.
	/// </summary>
	public class ApplicationRoleCreateCommandDuplicateException: Exception
	{
		//
		/// <summary>
		/// Implementation of ApplicationRoleCreateCommand duplicate exception.
		/// </summary>
		/// <param name="id">The key for the record.</param>
		public ApplicationRoleCreateCommandDuplicateException(string id)
			: base($"ApplicationRoleCreateCommand duplicate id exception: Id: {id}")
		{
		}
	}
	//
	/// <summary>
	/// Custom ApplicationRoleCreateCommand validation exception.
	/// </summary>
	public class ApplicationRoleCreateCommandValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of ApplicationRoleCreateCommand validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public ApplicationRoleCreateCommandValidationException(string errorMessage)
			: base($"ApplicationRoleCreateCommand validation exception: errors: {errorMessage}")
		{
		}
	}
    //
    /// <summary>
    /// Custom ApplicationRoleCreateCommand RoleManager exception.
    /// </summary>
    public class ApplicationRoleCreateCommandRoleManagerException : Exception
    {
        //
        /// <summary>
        /// Implementation of ApplicationRoleCreateCommand validation exception.
        /// </summary>
        /// <param name="errorMessage">The validation error messages.</param>
        public ApplicationRoleCreateCommandRoleManagerException(string errorMessage)
            : base($"ApplicationRoleCreateCommand RoleManager exception: errors: {errorMessage}")
        {
        }
    }
    //
}
// ---------------------------------------------------------------------------
