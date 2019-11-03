//
// ---------------------------------------------------------------------------
// ApplicationRoles detail query.
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
using System.Linq;
//
namespace NSG.WebSrv.Application.Commands.ApplicationRoles
{
	//
	/// <summary>
	/// 'ApplicationRole' detail query, handler and handle.
	/// </summary>
	public class ApplicationRoleUserDetailQuery
	{
		[System.ComponentModel.DataAnnotations.Key]
		public string Id { get; set; }
		public string Name { get; set; }
        //
        public List<UserListQuery> UserList { get; set; }
        //
    }
    //
    /// <summary>
    /// 'ApplicationUser' list query, handler and handle.
    /// </summary>
    public class UserListQuery
    {
        public string UserName { get; set; }
    }
    //
    /// <summary>
    /// 'ApplicationRole' detail query handler.
    /// </summary>
    public class ApplicationRoleUserDetailQueryHandler : IRequestHandler<ApplicationRoleUserDetailQueryHandler.DetailQuery, ApplicationRoleUserDetailQuery>
	{
        private RoleManager<ApplicationRole> _roleManager;
        //
        /// <summary>
        ///  The constructor for the inner handler class, to detail the ApplicationRole entity.
        /// </summary>
        /// <param name="context">The database interface context.</param>
        public ApplicationRoleUserDetailQueryHandler(RoleManager<ApplicationRole> roleManager)
		{
            _roleManager = roleManager;
		}
		//
		/// <summary>
		/// 'ApplicationRole' query handle method, passing two interfaces.
		/// </summary>
		/// <param name="request">This detail query request.</param>
		/// <param name="cancellationToken">Cancel token.</param>
		/// <returns>Returns the row count.</returns>
		public async Task<ApplicationRoleUserDetailQuery> Handle(DetailQuery request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new DetailQueryValidationException(_results.FluentValidationErrors());
			}
            //
            ApplicationRole _entity = await _roleManager.Roles
                .Include(u => u.UserRoles).ThenInclude(ur => ur.User)
                .FirstOrDefaultAsync(r => r.Id == request.Id);
            if (_entity == null)
            {
                throw new DetailQueryKeyNotFoundException(request.Id);
            }
            ApplicationRoleUserDetailQuery _detail = _entity.ToApplicationRoleUserDetailQuery();
            //
            // Get the list of Users in this Role
            //
            _detail.UserList = new List<UserListQuery>();
            foreach (ApplicationUserRole _userRole in _entity.UserRoles.ToList())
            {
                _detail.UserList.Add(new UserListQuery()
                    { UserName = _userRole.User.UserName });
            }
            // Return the detail query model.
            return _detail;
        }
		//
		/// <summary>
		/// Get ApplicationRole detail query class (the primary key).
		/// </summary>
		public class DetailQuery : IRequest<ApplicationRoleUserDetailQuery>
		{
			public string Id { get; set; }
		}
		//
		/// <summary>
		/// FluentValidation of the 'ApplicationRoleUserDetailQuery' class.
		/// </summary>
		public class Validator : AbstractValidator<DetailQuery>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'ApplicationRoleUserDetailQuery' validator.
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
	/// Custom ApplicationRoleUserDetailQuery record not found exception.
	/// </summary>
	public class DetailQueryKeyNotFoundException: KeyNotFoundException
	{
		//
		/// <summary>
		/// Implementation of ApplicationRoleUserDetailQuery record not found exception.
		/// </summary>
		/// <param name="id">The key for the record.</param>
		public DetailQueryKeyNotFoundException(string id)
			: base($"ApplicationRoleUserDetailQuery key not found exception: Id: {id}")
		{
		}
	}
	//
	/// <summary>
	/// Custom ApplicationRoleUserDetailQuery validation exception.
	/// </summary>
	public class DetailQueryValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of ApplicationRoleUserDetailQuery validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public DetailQueryValidationException(string errorMessage)
			: base($"ApplicationRoleUserDetailQuery validation exception: errors: {errorMessage}")
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
		/// Extension method that translates from ApplicationRole to ApplicationRoleUserDetailQuery.
		/// </summary>
		/// <param name="entity">The ApplicationRole entity class.</param>
		/// <returns>'ApplicationRoleUserDetailQuery' or ApplicationRole detail query.</returns>
		public static ApplicationRoleUserDetailQuery ToApplicationRoleUserDetailQuery(this ApplicationRole entity)
		{
			return new ApplicationRoleUserDetailQuery
			{
				Id = entity.Id,
				Name = entity.Name
			};
		}
	}
}
// ---------------------------------------------------------------------------

