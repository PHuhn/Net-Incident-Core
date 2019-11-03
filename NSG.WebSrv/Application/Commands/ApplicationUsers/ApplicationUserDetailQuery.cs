//
// ---------------------------------------------------------------------------
// ApplicationUsers detail query.
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
using Microsoft.AspNetCore.Identity;
using NSG.WebSrv.Application.Infrastructure;
//
namespace NSG.WebSrv.Application.Commands.ApplicationUsers
{
	//
	/// <summary>
	/// 'ApplicationUser' detail query, handler and handle.
	/// </summary>
	public class ApplicationUserDetailQuery
	{
		[System.ComponentModel.DataAnnotations.Key]
		public string Id { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public bool EmailConfirmed { get; set; }
		public string PhoneNumber { get; set; }
		public bool PhoneNumberConfirmed { get; set; }
		public int CompanyId { get; set; }
        public string CompanyShortName { get; set; }
        public string CompanyName { get; set; }
        public DateTime CreateDate { get; set; }
		public string FirstName { get; set; }
		public string FullName { get; set; }
		public string LastName { get; set; }
		public string UserNicName { get; set; }
        //
        public List<string> RoleList { get; set; }
        public List<ServerListQuery> ServerList { get; set; }
        //
    }
    public class ServerListQuery
    {
        public string CompanyShortName { get; set; }
        public string ServerShortName { get; set; }
    }
    //
    /// <summary>
    /// 'ApplicationUser' detail query handler.
    /// </summary>
    public class ApplicationUserDetailQueryHandler : IRequestHandler<ApplicationUserDetailQueryHandler.DetailQuery, ApplicationUserDetailQuery>
	{
        private UserManager<ApplicationUser> _userManager;
        private IMediator Mediator;
        //
        /// <summary>
        ///  The constructor for the inner handler class, to detail the ApplicationUser entity.
        /// </summary>
        /// <param name="userManager">The identity interface for users.</param>
        /// <param name="mediator">MediatR dependency injector.</param>
        public ApplicationUserDetailQueryHandler(UserManager<ApplicationUser> userManager, IMediator mediator)
        {
            _userManager = userManager;
            Mediator = mediator;
        }
        //
        /// <summary>
        /// 'ApplicationUser' query handle method, passing two interfaces.
        /// </summary>
        /// <param name="request">This detail query request.</param>
        /// <param name="cancellationToken">Cancel token.</param>
        /// <returns>Returns the row count.</returns>
        public async Task<ApplicationUserDetailQuery> Handle(DetailQuery request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new ApplicationUserDetailQueryValidationException(_results.FluentValidationErrors());
			}
            //
            GetUserCompanyListQueryHandler.ViewModel _companiesViewModel =
                await Mediator.Send(new GetUserCompanyListQueryHandler.ListQuery(), cancellationToken);
            //
            ApplicationUser _entity = await _userManager.Users
                .Include(u => u.Company)
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Include(u => u.UserServers).ThenInclude(us => us.Server).ThenInclude(s => s.Company)
                .FirstOrDefaultAsync(r => r.UserName == request.UserName);
			if (_entity == null)
			{
				throw new ApplicationUserDetailQueryKeyNotFoundException(request.UserName);
			}
            if (!_companiesViewModel.CompanyList.Contains(_entity.CompanyId))
            {
                throw new ApplicationUserDetailQueryPermissionException($"User does not have permission for company: {_entity.CompanyId}");
            }
            //
            ApplicationUserDetailQuery _detail = _entity.ToApplicationUserDetailQuery();
            _detail.RoleList = new List<string>();
            _detail.ServerList = new List<ServerListQuery>();
            foreach (ApplicationUserRole _userRole in _entity.UserRoles)
            {
                _detail.RoleList.Add( _userRole.Role.Name );
            }
            foreach (ApplicationUserServer _userSrv in _entity.UserServers)
            {
                _detail.ServerList.Add(new ServerListQuery()
                    {
                        CompanyShortName = _userSrv.Server.Company.CompanyShortName,
                        ServerShortName = _userSrv.Server.ServerShortName
                    });
            }
            // Return the detail query model.
            return _detail;
		}
		//
		/// <summary>
		/// Get ApplicationUser detail query class (the primary key).
		/// </summary>
		public class DetailQuery : IRequest<ApplicationUserDetailQuery>
		{
			public string UserName { get; set; }
		}
		//
		/// <summary>
		/// FluentValidation of the 'ApplicationUserDetailQuery' class.
		/// </summary>
		public class Validator : AbstractValidator<DetailQuery>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'ApplicationUserDetailQuery' validator.
			/// </summary>
			public Validator()
			{
				//
				RuleFor(x => x.UserName).NotEmpty().MaximumLength(256);
				//
			}
			//
		}
		//
	}
	//
	/// <summary>
	/// Custom ApplicationUserDetailQuery record not found exception.
	/// </summary>
	public class ApplicationUserDetailQueryKeyNotFoundException: KeyNotFoundException
	{
		//
		/// <summary>
		/// Implementation of ApplicationUserDetailQuery record not found exception.
		/// </summary>
		/// <param name="id">The key for the record.</param>
		public ApplicationUserDetailQueryKeyNotFoundException(string id)
			: base($"ApplicationUserDetailQuery key not found exception: Id: {id}")
		{
		}
	}
	//
	/// <summary>
	/// Custom ApplicationUserDetailQuery validation exception.
	/// </summary>
	public class ApplicationUserDetailQueryValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of ApplicationUserDetailQuery validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public ApplicationUserDetailQueryValidationException(string errorMessage)
			: base($"ApplicationUserDetailQuery validation exception: errors: {errorMessage}")
		{
		}
	}
    //
    /// <summary>
    /// Custom ApplicationUserDetailQuery permission exception.
    /// </summary>
    public class ApplicationUserDetailQueryPermissionException : Exception
    {
        //
        /// <summary>
        /// Implementation of ApplicationUserDetailQuery permission exception.
        /// </summary>
        /// <param name="errorMessage">The permission error messages.</param>
        public ApplicationUserDetailQueryPermissionException(string errorMessage)
            : base($"ApplicationUserDetailQuery validation exception: errors: {errorMessage}")
        {
        }
    }
    //
    /// <summary>
    /// Extension method.
    /// </summary>
    public static partial class Extensions
	{
		//
		/// <summary>
		/// Extension method that translates from ApplicationUser to ApplicationUserDetailQuery.
		/// </summary>
		/// <param name="entity">The ApplicationUser entity class.</param>
		/// <returns>'ApplicationUserDetailQuery' or ApplicationUser detail query.</returns>
		public static ApplicationUserDetailQuery ToApplicationUserDetailQuery(this ApplicationUser entity)
		{
			return new ApplicationUserDetailQuery
			{
				Id = entity.Id,
				UserName = entity.UserName,
				Email = entity.Email,
				EmailConfirmed = entity.EmailConfirmed,
				PhoneNumber = entity.PhoneNumber,
				PhoneNumberConfirmed = entity.PhoneNumberConfirmed,
				CompanyId = entity.CompanyId,
                CompanyShortName = entity.Company.CompanyShortName,
                CompanyName = entity.Company.CompanyName,
                CreateDate = entity.CreateDate,
				FirstName = entity.FirstName,
				FullName = entity.FullName,
				LastName = entity.LastName,
				UserNicName = entity.UserNicName,
			};
		}
	}
}
// ---------------------------------------------------------------------------
