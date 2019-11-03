//
// ---------------------------------------------------------------------------
// ApplicationUsers get valid/permissible companies for logged in user.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
//
using MediatR;
using FluentValidation;
using FluentValidation.Results;
//
using NSG.WebSrv.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using NSG.WebSrv.Infrastructure.Common;
//
namespace NSG.WebSrv.Application.Infrastructure
{
    //
    /// <summary>
    /// 'ApplicationUser' list query handler.
    /// </summary>
    public class GetUserCompanyListQueryHandler : IRequestHandler<GetUserCompanyListQueryHandler.ListQuery, GetUserCompanyListQueryHandler.ViewModel>
    {
        private readonly IDb_Context _context;
        private UserManager<ApplicationUser> _userManager;
        private IApplication _application;
        //
        /// <summary>
        ///  The constructor for the inner handler class, to list the ApplicationUser entity.
        /// </summary>
        /// <param name="context">The database interface context.</param>
        public GetUserCompanyListQueryHandler(IDb_Context context, UserManager<ApplicationUser> userManager, IApplication application)
        {
            _context = context;
            _userManager = userManager;
            _application = application;
        }
        //
        /// <summary>
        /// 'ApplicationUser' query handle method, passing two interfaces.
        /// </summary>
        /// <param name="queryRequest">This list query request.</param>
        /// <param name="cancellationToken">Cancel token.</param>
        /// <returns>Returns a list of GetUserCompanyListQuery.</returns>
        public async Task<ViewModel> Handle(ListQuery queryRequest, CancellationToken cancellationToken)
        {
            Validator _validator = new Validator();
            ValidationResult _results = _validator.Validate(queryRequest);
            if (!_results.IsValid)
            {
                // Call the FluentValidationErrors extension method.
                throw new ListQueryValidationException(_results.FluentValidationErrors());
            }
            string _userName = _application.GetUserAccount();
            ApplicationUser _entity = await _userManager.Users
                .Include(u => u.Company)
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Include(u => u.UserServers).ThenInclude(us => us.Server)
                .FirstOrDefaultAsync(r => r.UserName == _userName);
            if (_entity == null)
            {
                throw new GetUserCompanyListQueryUserException($"User not found: {_userName}");
            }
            ViewModel _companiesList = new ViewModel();
            string _adminRole = Helpers.GetUserCompanyRoleQuery(_entity);
            if (_adminRole == "adm")
            {
                _companiesList.CompanyList = _context.Companies.Select(c => c.CompanyId).ToList();
            }
            else
            {
                if (_adminRole == "cadm")
                {
                    foreach (ApplicationUserServer _userServer in _entity.UserServers)
                    {
                        // only add a specific company once
                        if (!_companiesList.CompanyList.Contains(_userServer.Server.CompanyId))
                        {
                            _companiesList.CompanyList.Add(_userServer.Server.CompanyId);
                        }
                    }
                }
            }
            //
            return _companiesList;
        }
        //
        /// <summary>
        /// The ApplicationUser list query class view class.
        /// </summary>
        public class ViewModel
        {
            public IList<int> CompanyList { get; set; }
            public ViewModel()
            {
                CompanyList = new List<int>();
            }
        }
        //
        /// <summary>
        /// Get ApplicationUser list query class (the primary key).
        /// </summary>
        public class ListQuery : IRequest<ViewModel>
        {
        }
        //
        /// <summary>
        /// FluentValidation of the 'GetUserCompanyListQuery' class.
        /// </summary>
        public class Validator : AbstractValidator<ListQuery>
        {
            //
            /// <summary>
            /// Constructor that will invoke the 'GetUserCompanyListQuery' validator.
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
    /// Custom GetUserCompanyListQuery validation exception.
    /// </summary>
    public class ListQueryValidationException : Exception
    {
        //
        /// <summary>
        /// Implementation of GetUserCompanyListQuery validation exception.
        /// </summary>
        /// <param name="errorMessage">The validation error messages.</param>
        public ListQueryValidationException(string errorMessage)
            : base($"GetUserCompanyListQuery validation exception: errors: {errorMessage}")
        {
        }
    }
    //
    /// <summary>
    /// Custom GetUserCompanyListQuery validation exception.
    /// </summary>
    public class GetUserCompanyListQueryUserException : Exception
    {
        //
        /// <summary>
        /// Implementation of GetUserCompanyListQuery validation exception.
        /// </summary>
        /// <param name="errorMessage">The validation error messages.</param>
        public GetUserCompanyListQueryUserException(string errorMessage)
            : base($"GetUserCompanyListQuery user validation exception: errors: {errorMessage}")
        {
        }
    }
}
// ---------------------------------------------------------------------------
