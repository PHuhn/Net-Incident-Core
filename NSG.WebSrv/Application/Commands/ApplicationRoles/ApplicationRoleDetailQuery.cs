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
    public class ApplicationRoleDetailQuery
    {
        [System.ComponentModel.DataAnnotations.Key]
        public string Id { get; set; }
        public string Name { get; set; }
    }
    //
    /// <summary>
    /// 'ApplicationRole' detail query handler.
    /// </summary>
    public class ApplicationRoleDetailQueryHandler : IRequestHandler<ApplicationRoleDetailQueryHandler.DetailQuery, ApplicationRoleDetailQuery>
    {
        private RoleManager<ApplicationRole> _roleManager;
        //
        /// <summary>
        ///  The constructor for the inner handler class, to detail the ApplicationRole entity.
        /// </summary>
        /// <param name="context">The database interface context.</param>
        public ApplicationRoleDetailQueryHandler(RoleManager<ApplicationRole> roleManager)
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
        public async Task<ApplicationRoleDetailQuery> Handle(DetailQuery request, CancellationToken cancellationToken)
        {
            Validator _validator = new Validator();
            ValidationResult _results = _validator.Validate(request);
            if (!_results.IsValid)
            {
                // Call the FluentValidationErrors extension method.
                throw new ApplicationRoleDetailQueryValidationException(_results.FluentValidationErrors());
            }
            // ApplicationDbContext _db = ApplicationDbContext.Create();
            var _entity = await _roleManager.FindByIdAsync(request.Id);
            // .Include(u => u.UserRoles).ThenInclude(ur => ur.Role).ToList();
            if (_entity == null)
            {
                throw new ApplicationRoleDetailQueryKeyNotFoundException(request.Id);
            }
            // Return the detail query model.
            return _entity.ToApplicationRoleDetailQuery();
        }
        //
        /// <summary>
        /// Get ApplicationRole detail query class (the primary key).
        /// </summary>
        public class DetailQuery : IRequest<ApplicationRoleDetailQuery>
        {
            public string Id { get; set; }
        }
        //
        /// <summary>
        /// FluentValidation of the 'ApplicationRoleDetailQuery' class.
        /// </summary>
        public class Validator : AbstractValidator<DetailQuery>
        {
            //
            /// <summary>
            /// Constructor that will invoke the 'ApplicationRoleDetailQuery' validator.
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
    /// Custom ApplicationRoleDetailQuery record not found exception.
    /// </summary>
    public class ApplicationRoleDetailQueryKeyNotFoundException : KeyNotFoundException
    {
        //
        /// <summary>
        /// Implementation of ApplicationRoleDetailQuery record not found exception.
        /// </summary>
        /// <param name="id">The key for the record.</param>
        public ApplicationRoleDetailQueryKeyNotFoundException(string id)
            : base($"ApplicationRoleDetailQuery key not found exception: Id: {id}")
        {
        }
    }
    //
    /// <summary>
    /// Custom ApplicationRoleDetailQuery validation exception.
    /// </summary>
    public class ApplicationRoleDetailQueryValidationException : Exception
    {
        //
        /// <summary>
        /// Implementation of ApplicationRoleDetailQuery validation exception.
        /// </summary>
        /// <param name="errorMessage">The validation error messages.</param>
        public ApplicationRoleDetailQueryValidationException(string errorMessage)
            : base($"ApplicationRoleDetailQuery validation exception: errors: {errorMessage}")
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
        /// Extension method that translates from ApplicationRole to ApplicationRoleDetailQuery.
        /// </summary>
        /// <param name="entity">The ApplicationRole entity class.</param>
        /// <returns>'ApplicationRoleDetailQuery' or ApplicationRole detail query.</returns>
        public static ApplicationRoleDetailQuery ToApplicationRoleDetailQuery(this ApplicationRole entity)
        {
            return new ApplicationRoleDetailQuery
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }
    }
}
// ---------------------------------------------------------------------------
