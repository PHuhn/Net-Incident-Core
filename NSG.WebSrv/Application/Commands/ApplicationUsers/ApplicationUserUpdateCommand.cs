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
	public class ApplicationUserUpdateCommand : IRequest<int>
	{
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int CompanyId { get; set; }
        public string FirstName { get; set; }
        public string FullName { get; set; }
        public string LastName { get; set; }
        public string UserNicName { get; set; }
        //
        public string[] SelectedRoles { get; set; }
        public int[] SelectedServers { get; set; }
        //
    }
    //
    /// <summary>
    /// 'ApplicationUser' update command handler.
    /// </summary>
    public class ApplicationUserUpdateCommandHandler : IRequestHandler<ApplicationUserUpdateCommand, int>
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
        public ApplicationUserUpdateCommandHandler(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMediator mediator)
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
        public async Task<int> Handle(ApplicationUserUpdateCommand request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new ApplicationUserUpdateCommandValidationException(_results.FluentValidationErrors());
			}
            ApplicationUser _entity = await _context.Users
                .Include(u => u.Company)
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Include(u => u.UserServers).ThenInclude(us => us.Server).ThenInclude(s => s.Company)
                .FirstOrDefaultAsync(r => r.UserName == request.UserName);
            if (_entity == null)
			{
                await Mediator.Send(new LogCreateCommand() {
                    Level = LoggingLevel.Warning, Method = MethodBase.GetCurrentMethod(),
                    Message = $"User: {request.UserName} not found.",
                    Exception = null });
                throw new ApplicationUserUpdateCommandKeyNotFoundException(request.UserName);
			}
            // Move from update command class to entity class.
            await MoveRequestToEntity(_context, request, _entity);
            // Add/delete values from the two selection lists.
            await AddDeleteRoles(_context, _entity, request, cancellationToken);
            await AddDeleteServers(_context, _entity, request, cancellationToken);
            // Save the complete changes
            // var _updateResults = await _userManager.UpdateAsync(_entity);
            await _context.SaveChangesAsync(cancellationToken);
            // Return the row count.
            return 1;
            //if (_updateResults.Succeeded)
            //{
            //    return 1;
            //}
            //return 0;
		}
        //
        private async Task MoveRequestToEntity(IDb_Context context, 
            ApplicationUserUpdateCommand request, ApplicationUser entity)
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
                if (entity.CompanyId != request.CompanyId)
                {
                    entity.CompanyId = request.CompanyId;
                    entity.Company = context.Companies.SingleOrDefault(c => c.CompanyId == request.CompanyId);
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
                await Mediator.Send(new LogCreateCommand() {
                    Level = LoggingLevel.Warning, Method = MethodBase.GetCurrentMethod(),
                    Message = "Requested roles: " + string.Join(", ", request.SelectedRoles),
                    Exception = _ex });
                throw new ApplicationUserUpdateCommandException(
                    $"User: {request.UserName}, message: {_ex.GetBaseException()}");
            }
        }
        //
        /// <summary>
        /// Add and delete roles
        /// </summary>
        /// <param name="userManager">UserManager of ApplicationUser</param>
        /// <param name="user">An ApplicationUser</param>
        /// <param name="selectedRole">Collection of selected roles</param>
        private async Task AddDeleteRoles(ApplicationDbContext context, 
            ApplicationUser user, ApplicationUserUpdateCommand request,
            CancellationToken cancellationToken)
        {
            //
            request.SelectedRoles = request.SelectedRoles ?? new string[] { };
            //
            try
            {
                //
                List<string> _userRoles = await (from userRole in context.UserRoles
                            join role in context.Roles on userRole.RoleId equals role.Id
                            where userRole.UserId.Equals(request.Id)
                            select role.Id).ToListAsync(cancellationToken);
                List<string> _addingRoles = request.SelectedRoles.Except(_userRoles).ToList();
                List<string> _deletingRoles = _userRoles.Except(request.SelectedRoles).ToList();
                if( _deletingRoles.Count > 0 )
                {
                    // Remove servers
                    foreach (ApplicationUserRole _ur in user.UserRoles)
                    {
                        if (_deletingRoles.Contains(_ur.RoleId))
                        {
                            _context.UserRoles.Remove(_ur);
                        }
                    }
                }
                if (_addingRoles.Count > 0)
                {
                    foreach (string _roleId in _addingRoles)
                    {
                        _context.UserRoles.Add(new ApplicationUserRole() { RoleId = _roleId, UserId = request.Id });
                    }
                }
            }
            catch (Exception _ex)
            {
                await Mediator.Send(new LogCreateCommand() {
                    Level = LoggingLevel.Warning, Method = MethodBase.GetCurrentMethod(),
                    Message = request.UserName + ", requested roles: " + string.Join(", ", request.SelectedRoles),
                    Exception = _ex });
                throw new ApplicationUserUpdateCommandException(
                    $"User: {request.UserName},  role update, message: {_ex.GetBaseException()}");
            }
        }
        //
        /// <summary>
        /// Add and delete roles
        /// </summary>
        /// <param name="context">Database IDb_Context</param>
        /// <param name="user">An ApplicationUser</param>
        /// <param name="request">full request including selected servers</param>
        /// <param name="cancellationToken">cancellationToken</param>
        private async Task AddDeleteServers(IDb_Context context, ApplicationUser user,
            ApplicationUserUpdateCommand request, CancellationToken cancellationToken)
        {
            //
            bool _changed = false;
            request.SelectedServers = request.SelectedServers ?? new int[] { };
            //
            try
            {
                List<int> _userServers = user.UserServers.Select(us => us.ServerId).ToList();
                List<int> _addingServers = request.SelectedServers.Except(_userServers).ToList();
                List<int> _deletingServers = _userServers.Except(request.SelectedServers).ToList();
                // Remove servers
                foreach (ApplicationUserServer _as in user.UserServers)
                {
                    if (_deletingServers.Contains(_as.ServerId))
                    {
                        _context.UserServers.Remove(_as);
                        _changed = true;
                    }
                }
                // Adding servers to this user
                foreach (int _serverId in _addingServers)
                {
                    _context.UserServers.Add(new ApplicationUserServer() { Id = user.Id, ServerId = _serverId });
                    _changed = true;
                }
                if(_changed == true)
                {
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }
            catch ( Exception _ex)
            {
                await Mediator.Send(new LogCreateCommand() {
                    Level = LoggingLevel.Warning, Method = MethodBase.GetCurrentMethod(),
                    Message = request.UserName + ", requested servers: " + string.Join(", ", request.SelectedServers),
                    Exception = _ex });
                throw new ApplicationUserUpdateCommandException(
                    $"User: {request.UserName},  role Server, message: {_ex.GetBaseException()}");
            }
        }
        //
        /// <summary>
        /// FluentValidation of the 'ApplicationUserUpdateCommand' class.
        /// </summary>
        public class Validator : AbstractValidator<ApplicationUserUpdateCommand>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'ApplicationUserUpdateCommand' validator.
			/// </summary>
			public Validator()
			{
				//
				RuleFor(x => x.Id).NotEmpty().MaximumLength(128);
				RuleFor(x => x.UserName).MaximumLength(256);
				RuleFor(x => x.Email).MaximumLength(256);
				RuleFor(x => x.PhoneNumber).MaximumLength(1073741823);
				RuleFor(x => x.CompanyId).NotNull();
				RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
				RuleFor(x => x.FullName).NotEmpty().MaximumLength(100);
				RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
				RuleFor(x => x.UserNicName).NotEmpty().MaximumLength(16);
                // RuleFor(x => x.SelectedRoles).NotNull();
                // RuleFor(x => x.SelectedServers).NotNull();
                //
            }
			//
		}
		//
	}
	//
	/// <summary>
	/// Custom ApplicationUserUpdateCommand record not found exception.
	/// </summary>
	public class ApplicationUserUpdateCommandKeyNotFoundException: KeyNotFoundException
	{
		//
		/// <summary>
		/// Implementation of ApplicationUserUpdateCommand record not found exception.
		/// </summary>
		/// <param name="id">The key for the record.</param>
		public ApplicationUserUpdateCommandKeyNotFoundException(string id)
			: base($"ApplicationUserUpdateCommand key not found exception: id: {id}")
		{
		}
	}
	//
	/// <summary>
	/// Custom ApplicationUserUpdateCommand validation exception.
	/// </summary>
	public class ApplicationUserUpdateCommandValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of ApplicationUserUpdateCommand validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public ApplicationUserUpdateCommandValidationException(string errorMessage)
			: base($"ApplicationUserUpdateCommand validation exception: errors: {errorMessage}")
		{
		}
	}
    //
    /// <summary>
    /// Custom ApplicationUserUpdateCommand exception.
    /// </summary>
    public class ApplicationUserUpdateCommandException : Exception
    {
        //
        /// <summary>
        /// Implementation of ApplicationUserUpdateCommand validation exception.
        /// </summary>
        /// <param name="errorMessage">The validation error messages.</param>
        public ApplicationUserUpdateCommandException(string errorMessage)
            : base($"ApplicationUserUpdateCommand exception: errors: {errorMessage}")
        {
        }
    }
    //
}
// ---------------------------------------------------------------------------

