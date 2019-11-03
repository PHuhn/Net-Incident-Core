//
// ---------------------------------------------------------------------------
// ApplicationUsers update command.
//
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
//
using Microsoft.EntityFrameworkCore;
using MediatR;
using FluentValidation;
using FluentValidation.Results;
using NSG.WebSrv.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using NSG.WebSrv.Application.Commands.Logs;
using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//
namespace NSG.WebSrv.Application.Commands.ApplicationUsers
{
	//
	/// <summary>
	/// 'ApplicationUser' update command, handler and handle.
	/// </summary>
	public class ApplicationUserManageUpdateCommand : IRequest<int>
	{
        //
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string FullName { get; set; }
        public string LastName { get; set; }
        public string UserNicName { get; set; }
        //
    }
    //
    /// <summary>
    /// 'ApplicationUser' update command handler.
    /// </summary>
    public class ApplicationUserManageUpdateCommandHandler : IRequestHandler<ApplicationUserManageUpdateCommand, int>
	{
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private IMediator Mediator;
        //
        /// <summary>
        ///  The constructor for the inner handler class, to update the ApplicationUser entity.
        /// </summary>
        /// <param name="context">The database interface context.</param>
        /// <param name="userManager">The identity interface for users.</param>
        /// <param name="mediator">MediatR dependency injector.</param>
        public ApplicationUserManageUpdateCommandHandler(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMediator mediator)
        {
            _context = context;
            _userManager = userManager;
            Mediator = mediator;
        }
        //
        /// <summary>
        /// 'ApplicationUser' command handle method, passing two interfaces.
        /// </summary>
        /// <param name="request">This update command request.</param>
        /// <param name="cancellationToken">Cancel token.</param>
        /// <returns>Returns the row count.</returns>
        public async Task<int> Handle(ApplicationUserManageUpdateCommand request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new ApplicationUserManageUpdateCommandValidationException(_results.FluentValidationErrors());
			}
            ApplicationUser _entity = await _context.Users
                .FirstOrDefaultAsync(r => r.UserName == request.UserName);
            if (_entity == null)
			{
                await Mediator.Send(new LogCreateCommand() {
                    Level = LoggingLevel.Warning, Method = MethodBase.GetCurrentMethod(),
                    Message = $"User: {request.UserName} not found.",
                    Exception = null });
                throw new ApplicationUserManageUpdateCommandKeyNotFoundException(request.UserName);
			}
            // Move from update command class to entity class.
            MoveRequestToEntity(request, _entity);
            // Save the complete changes
            await _context.SaveChangesAsync(cancellationToken);
            // Return the row count.
            return 1;
		}
        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="entity"></param>
        private void MoveRequestToEntity(
            ApplicationUserManageUpdateCommand request, ApplicationUser entity)
        {
            try
            {
                if (entity.Email != request.Email)
                {
                    entity.Email = request.Email;
                    entity.NormalizedEmail = request.Email.ToUpper();
                    entity.EmailConfirmed = false;
                }
                if (entity.PhoneNumber != request.PhoneNumber)
                {
                    entity.PhoneNumber = request.PhoneNumber;
                    entity.PhoneNumberConfirmed = false;
                }
                if (entity.FirstName != request.FirstName)
                {
                    entity.FirstName = request.FirstName;
                }
                if (entity.FullName != request.FullName)
                {
                    entity.FullName = request.FullName;
                }
                if (entity.LastName != request.LastName)
                {
                    entity.LastName = request.LastName;
                }
                if (entity.UserNicName != request.UserNicName)
                {
                    entity.UserNicName = request.UserNicName;
                }
            }
            catch (Exception _ex)
            {
                throw new ApplicationUserManageUpdateCommandException(
                    $"User: {request.UserName}, message: {_ex.GetBaseException()}");
            }
        }
        //
        /// <summary>
        /// FluentValidation of the 'ApplicationUserManageUpdateCommand' class.
        /// </summary>
        public class Validator : AbstractValidator<ApplicationUserManageUpdateCommand>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'ApplicationUserManageUpdateCommand' validator.
			/// </summary>
			public Validator()
			{
				//
				RuleFor(x => x.UserName).MaximumLength(256);
				RuleFor(x => x.Email).MaximumLength(256);
				RuleFor(x => x.PhoneNumber).MaximumLength(1073741823);
				RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
				RuleFor(x => x.FullName).NotEmpty().MaximumLength(100);
				RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
				RuleFor(x => x.UserNicName).NotEmpty().MaximumLength(16);
                //
            }
			//
		}
		//
	}
	//
	/// <summary>
	/// Custom ApplicationUserManageUpdateCommand record not found exception.
	/// </summary>
	public class ApplicationUserManageUpdateCommandKeyNotFoundException: KeyNotFoundException
	{
		//
		/// <summary>
		/// Implementation of ApplicationUserManageUpdateCommand record not found exception.
		/// </summary>
		/// <param name="id">The key for the record.</param>
		public ApplicationUserManageUpdateCommandKeyNotFoundException(string id)
			: base($"ApplicationUserManageUpdateCommand key not found exception: id: {id}")
		{
		}
	}
	//
	/// <summary>
	/// Custom ApplicationUserManageUpdateCommand validation exception.
	/// </summary>
	public class ApplicationUserManageUpdateCommandValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of ApplicationUserManageUpdateCommand validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public ApplicationUserManageUpdateCommandValidationException(string errorMessage)
			: base($"ApplicationUserManageUpdateCommand validation exception: errors: {errorMessage}")
		{
		}
	}
    //
    /// <summary>
    /// Custom ApplicationUserManageUpdateCommand exception.
    /// </summary>
    public class ApplicationUserManageUpdateCommandException : Exception
    {
        //
        /// <summary>
        /// Implementation of ApplicationUserManageUpdateCommand validation exception.
        /// </summary>
        /// <param name="errorMessage">The validation error messages.</param>
        public ApplicationUserManageUpdateCommandException(string errorMessage)
            : base($"ApplicationUserManageUpdateCommand exception: errors: {errorMessage}")
        {
        }
    }
    //
}
// ---------------------------------------------------------------------------
