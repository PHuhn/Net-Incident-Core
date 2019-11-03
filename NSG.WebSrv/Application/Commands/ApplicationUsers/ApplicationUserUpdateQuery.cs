//
// ---------------------------------------------------------------------------
// ApplicationUsers detail query.
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
using NSG.WebSrv.Application.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
//
namespace NSG.WebSrv.Application.Commands.ApplicationUsers
{
	//
	/// <summary>
	/// 'ApplicationUser' detail query, handler and handle.
	/// </summary>
	public class ApplicationUserUpdateQuery
	{
		[System.ComponentModel.DataAnnotations.Key]
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
        public List<SelectListItem> RolesList { get; set; }
        public List<SelectListItem> ServersList { get; set; }
        public List<SelectListItem> CompaniesList { get; set; }
        //
    }
    //
    /// <summary>
    /// Users current servers
    /// </summary>
    public class ServerEditQuery
    {
        public int ServerId { get; set; }
    }
    //
    /// <summary>
    /// All available servers
    /// </summary>
    public class Servers
    {
        public int ServerId { get; set; }
        public string CompanyShortName { get; set; }
        public string ServerShortName { get; set; }
    }
    //
    /// <summary>
    /// 'ApplicationUser' detail query handler.
    /// </summary>
    public class ApplicationUserUpdateQueryHandler : IRequestHandler<ApplicationUserUpdateQueryHandler.EditQuery, ApplicationUserUpdateQuery>
	{
        //
        private IDb_Context _context;
        private UserManager<ApplicationUser> _userManager;
        private IMediator Mediator;
        //
        /// <summary>
        ///  The constructor for the inner handler class, to detail the ApplicationUser entity.
        /// </summary>
        /// <param name="context">The database interface context.</param>
        /// <param name="userManager">The identity interface for users.</param>
        /// <param name="mediator">MediatR dependency injector.</param>
        public ApplicationUserUpdateQueryHandler(IDb_Context context, UserManager<ApplicationUser> userManager, IMediator mediator)
        {
            _context = context;
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
        public async Task<ApplicationUserUpdateQuery> Handle(EditQuery request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new ApplicationUserUpdateQueryValidationException(_results.FluentValidationErrors());
			}
            //
            ApplicationUser _entity = await _userManager.Users
                .Include(u => u.Company)
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Include(u => u.UserServers).ThenInclude(us => us.Server).ThenInclude(s => s.Company)
                .FirstOrDefaultAsync(r => r.UserName == request.UserName);
			if (_entity == null)
			{
				throw new ApplicationUserUpdateQueryKeyNotFoundException(request.UserName);
			}
            ApplicationUserUpdateQuery _detail = _entity.ToApplicationUserUpdateQuery();
            await CreateRolesServerSelectionList(_detail, _entity);
            //
            // Return the detail query model.
            return _detail;
		}
        //
        /// <summary>
        /// Populate the 3 SelectItemList.
        /// </summary>
        /// <param name="detail"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        private async Task CreateRolesServerSelectionList(ApplicationUserUpdateQuery detail, ApplicationUser entity)
        {
            //
            GetUserCompanyListQueryHandler.ViewModel _companiesViewModel =
                await Mediator.Send(new GetUserCompanyListQueryHandler.ListQuery());
            // Get existing values
            IList<string> _userRoles = await _userManager.GetRolesAsync(entity);
            List<string> _serverList = new List<string>();
            // list of current servers
            foreach (ApplicationUserServer _userSrv in entity.UserServers)
            {
                _serverList.Add(_userSrv.Server.ServerShortName);
            }
            // Create the user's current roles SelectItemList.
            detail.RolesList = _context.Roles.ToList()
                .Select(x => new SelectListItem()
                {
                    Selected = _userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Id
                }).ToList();
            // Get the user's current servers SelectItemList.
            detail.ServersList = _context.Servers
                .Where(srv => _companiesViewModel.CompanyList.Contains(srv.CompanyId))
                .ToList().Select(x => new SelectListItem()
                {
                    Selected = _serverList.Contains(x.ServerShortName),
                    Text = x.ServerShortName,
                    Value = x.ServerId.ToString()
                }).ToList();
            // Create the available lists
            FixupCompanyAdminRoles(detail, entity);
            // Company dropdown SelectItemList
            CompaniesSelectionList(detail, _companiesViewModel.CompanyList);
        }
        //
        private void FixupCompanyAdminRoles(ApplicationUserUpdateQuery details, ApplicationUser entity)
        {
            string _adminRole = Helpers.GetUserCompanyRoleQuery(entity);
            // don't allow the company admins to set the admin role
            if (_adminRole == "cadm")
            {
                var _admItemToRemove = details.RolesList.SingleOrDefault(sl => sl.Value == "adm");
                if (_admItemToRemove != null)
                    details.RolesList.Remove(_admItemToRemove);
            }
        }
        //
        private void CompaniesSelectionList(ApplicationUserUpdateQuery details,
            IList<int> companiesAllowed)
        {
            details.CompaniesList = _context.Companies
                .Where(cmp => companiesAllowed.Contains(cmp.CompanyId))
                .ToList().Select(c => new SelectListItem()
                {
                    Text = $"{c.CompanyName} ({c.CompanyShortName})",
                    Value = c.CompanyId.ToString()
                }).ToList();
        }
        //
        /// <summary>
        /// Get ApplicationUser detail query class (the primary key).
        /// </summary>
        public class EditQuery : IRequest<ApplicationUserUpdateQuery>
		{
			public string UserName { get; set; }
		}
		//
		/// <summary>
		/// FluentValidation of the 'ApplicationUserUpdateQuery' class.
		/// </summary>
		public class Validator : AbstractValidator<EditQuery>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'ApplicationUserUpdateQuery' validator.
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
	/// Custom ApplicationUserUpdateQuery record not found exception.
	/// </summary>
	public class ApplicationUserUpdateQueryKeyNotFoundException: KeyNotFoundException
	{
		//
		/// <summary>
		/// Implementation of ApplicationUserUpdateQuery record not found exception.
		/// </summary>
		/// <param name="id">The key for the record.</param>
		public ApplicationUserUpdateQueryKeyNotFoundException(string id)
			: base($"ApplicationUserUpdateQuery key not found exception: Id: {id}")
		{
		}
	}
	//
	/// <summary>
	/// Custom ApplicationUserUpdateQuery validation exception.
	/// </summary>
	public class ApplicationUserUpdateQueryValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of ApplicationUserUpdateQuery validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public ApplicationUserUpdateQueryValidationException(string errorMessage)
			: base($"ApplicationUserUpdateQuery validation exception: errors: {errorMessage}")
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
		/// Extension method that translates from ApplicationUser to ApplicationUserUpdateQuery.
		/// </summary>
		/// <param name="entity">The ApplicationUser entity class.</param>
		/// <returns>'ApplicationUserUpdateQuery' or ApplicationUser detail query.</returns>
		public static ApplicationUserUpdateQuery ToApplicationUserUpdateQuery(this ApplicationUser entity)
		{
			return new ApplicationUserUpdateQuery
			{
				Id = entity.Id,
				UserName = entity.UserName,
				Email = entity.Email,
				PhoneNumber = entity.PhoneNumber,
				CompanyId = entity.CompanyId,
				FirstName = entity.FirstName,
				FullName = entity.FullName,
				LastName = entity.LastName,
				UserNicName = entity.UserNicName,
			};
		}
	}
}
// ---------------------------------------------------------------------------
