//
// ---------------------------------------------------------------------------
// ApplicationUsers delete command.
//
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
//
using Microsoft.EntityFrameworkCore;
using MediatR;
using FluentValidation;
using FluentValidation.Results;
//
using NSG.WebSrv.Domain.Entities;
using NSG.WebSrv.Application.Commands.Logs;
using Microsoft.AspNetCore.Identity;
//
namespace NSG.WebSrv.Application.Commands.ApplicationUsers
{
	//
	/// <summary>
	/// 'ApplicationUser' delete command, handler and handle.
	/// </summary>
	public class ApplicationUserDeleteCommand : IRequest<int>
	{
		public string UserName { get; set; }
	}
	//
	/// <summary>
	/// 'ApplicationUser' delete command handler.
	/// </summary>
	public class ApplicationUserDeleteCommandHandler : IRequestHandler<ApplicationUserDeleteCommand, int>
	{
        private UserManager<ApplicationUser> _userManager;
        private IDb_Context _context;
        private IMediator Mediator;
        //
        /// <summary>
        ///  The constructor for the inner handler class, to delete the ApplicationUser entity.
        /// </summary>
        /// <param name="context">The database interface context.</param>
        public ApplicationUserDeleteCommandHandler(IDb_Context context, UserManager<ApplicationUser> userManager, IMediator mediator)
		{
            _userManager = userManager;
            _context = context;
            Mediator = mediator;
        }
        //
        /// <summary>
        /// 'ApplicationUser' command handle method, passing two interfaces.
        /// </summary>
        /// <param name="request">This update command request.</param>
        /// <param name="cancellationToken">Cancel token.</param>
        /// <returns>Returns the row count.</returns>
        public async Task<int> Handle(ApplicationUserDeleteCommand request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new DeleteCommandValidationException(_results.FluentValidationErrors());
			}
            ApplicationUser _entity = await _userManager.Users
                .Include(u => u.Company)
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Include(u => u.UserServers).ThenInclude(us => us.Server).ThenInclude(s => s.Company)
                .FirstOrDefaultAsync(r => r.UserName == request.UserName);
            if (_entity == null)
			{
				throw new DeleteCommandKeyNotFoundException(request.UserName);
			}
            foreach(ApplicationUserServer _userServer in _entity.UserServers )
            {
                _context.UserServers.Remove(_userServer);
            }
			//
			await _userManager.DeleteAsync(_entity);
            await Mediator.Send(new LogCreateCommand(
                LoggingLevel.Warning, MethodBase.GetCurrentMethod(),
                "Deleted User: " + _entity.ApplicationUserToString(), null));
            // Return the row count affected.
            return 1;
		}
		//
		/// <summary>
		/// FluentValidation of the 'ApplicationUserDeleteCommand' class.
		/// </summary>
		public class Validator : AbstractValidator<ApplicationUserDeleteCommand>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'ApplicationUserDeleteCommand' validator.
			/// </summary>
			public Validator()
			{
				//
				RuleFor(x => x.UserName).NotEmpty().MaximumLength(128);
				//
			}
			//
		}
		//
	}
	//
	/// <summary>
	/// Custom ApplicationUserDeleteCommand record not found exception.
	/// </summary>
	public class DeleteCommandKeyNotFoundException: KeyNotFoundException
	{
		//
		/// <summary>
		/// Implementation of ApplicationUserDeleteCommand record not found exception.
		/// </summary>
		/// <param name="id">The key for the record.</param>
		public DeleteCommandKeyNotFoundException(string id)
			: base($"ApplicationUserDeleteCommand key not found exception: id: {id}")
		{
		}
	}
	//
	/// <summary>
	/// Custom ApplicationUserDeleteCommand validation exception.
	/// </summary>
	public class DeleteCommandValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of ApplicationUserDeleteCommand validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public DeleteCommandValidationException(string errorMessage)
			: base($"ApplicationUserDeleteCommand validation exception: errors: {errorMessage}")
		{
		}
	}
	//
}
// ---------------------------------------------------------------------------

