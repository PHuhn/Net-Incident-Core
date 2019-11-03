//
// ---------------------------------------------------------------------------
// ApplicationRoles list query.
//
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
//
using MediatR;
using FluentValidation;
using FluentValidation.Results;
using NSG.WebSrv.Domain.Entities;
//
namespace NSG.WebSrv.Application.Commands.ApplicationRoles
{
	//
	/// <summary>
	/// 'ApplicationRole' list query, handler and handle.
	/// </summary>
	public class ApplicationRoleListQuery
	{
		[System.ComponentModel.DataAnnotations.Key]
		public string Id { get; set; }
		public string Name { get; set; }
	}
	//
	/// <summary>
	/// 'ApplicationRole' list query handler.
	/// </summary>
	public class ApplicationRoleListQueryHandler : IRequestHandler<ApplicationRoleListQueryHandler.ListQuery, ApplicationRoleListQueryHandler.ViewModel>
	{
        private RoleManager<ApplicationRole> _roleManager;
		//
		/// <summary>
		///  The constructor for the inner handler class, to list the ApplicationRole entity.
		/// </summary>
		/// <param name="context">The database interface context.</param>
		public ApplicationRoleListQueryHandler(RoleManager<ApplicationRole> roleManager)
		{
            _roleManager = roleManager;
		}
		//
		/// <summary>
		/// 'ApplicationRole' query handle method, passing two interfaces.
		/// </summary>
		/// <param name="queryRequest">This list query request.</param>
		/// <param name="cancellationToken">Cancel token.</param>
		/// <returns>Returns a list of ApplicationRoleListQuery.</returns>
		public async Task<ViewModel> Handle(ListQuery queryRequest, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(queryRequest);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new ListQueryValidationException(_results.FluentValidationErrors());
			}
            return new ViewModel()
			{
				ApplicationRolesList = await _roleManager.Roles.ToAsyncEnumerable()
					.Select(cnt => cnt.ToApplicationRoleListQuery()).ToList()
			};
		}
		//
		/// <summary>
		/// The ApplicationRole list query class view class.
		/// </summary>
		public class ViewModel
		{
			public IList<ApplicationRoleListQuery> ApplicationRolesList { get; set; }
		}
		//
		/// <summary>
		/// Get ApplicationRole list query class (the primary key).
		/// </summary>
		public class ListQuery : IRequest<ViewModel>
		{
		}
		//
		/// <summary>
		/// FluentValidation of the 'ApplicationRoleListQuery' class.
		/// </summary>
		public class Validator : AbstractValidator<ListQuery>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'ApplicationRoleListQuery' validator.
			/// </summary>
			public Validator()
			{
				//
			}
			//
		}
		//
	}
	//
	/// <summary>
	/// Custom ApplicationRoleListQuery validation exception.
	/// </summary>
	public class ListQueryValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of ApplicationRoleListQuery validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public ListQueryValidationException(string errorMessage)
			: base($"ApplicationRoleListQuery validation exception: errors: {errorMessage}")
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
		/// Extension method that translates from ApplicationRole to ApplicationRoleListQuery.
		/// </summary>
		/// <param name="entity">The ApplicationRole entity class.</param>
		/// <returns>'ApplicationRoleListQuery' or ApplicationRole list query.</returns>
		public static ApplicationRoleListQuery ToApplicationRoleListQuery(this ApplicationRole entity)
		{
			return new ApplicationRoleListQuery
			{
				Id = entity.Id,
				Name = entity.Name,
			};
		}
	}
}
// ---------------------------------------------------------------------------

