//
// ---------------------------------------------------------------------------
// Company & Server list query.
//
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
//
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using MediatR;
using FluentValidation;
using FluentValidation.Results;
using NSG.WebSrv.Domain.Entities;
using NSG.WebSrv.Application.Infrastructure;
//
namespace NSG.WebSrv.Application.Commands.Companies
{
    //
    /// <summary>
    /// 'Company' list query handler.
    /// </summary>
    public class UserCompanySelectionListQueryHandler : IRequestHandler<UserCompanySelectionListQueryHandler.ListQuery, UserCompanySelectionListQueryHandler.ViewModel>
    {
        private readonly IDb_Context _context;
        UserManager<ApplicationUser> _userManager;
        private IMediator Mediator;
        //
        /// <summary>
        ///  The constructor for the inner handler class, to list the Company entity.
        /// </summary>
        /// <param name="context">The database interface context.</param>
        /// <param name="userManager">UserManager<ApplicationUser>.</param>
        public UserCompanySelectionListQueryHandler(UserManager<ApplicationUser> userManager, IDb_Context context, IMediator mediator)
        {
            _userManager = userManager;
            _context = context;
            Mediator = mediator;
        }
        //
        /// <summary>
        /// 'Company' query handle method, passing two interfaces.
        /// </summary>
        /// <param name="queryRequest">This list query request.</param>
        /// <param name="cancellationToken">Cancel token.</param>
        /// <returns>Returns a list of CompanyListQuery.</returns>
        public async Task<ViewModel> Handle(ListQuery queryRequest, CancellationToken cancellationToken)
        {
            Validator _validator = new Validator();
            ValidationResult _results = _validator.Validate(queryRequest);
            if (!_results.IsValid)
            {
                // Call the FluentValidationErrors extension method.
                throw new UserCompanySelectionListQueryValidationException(_results.FluentValidationErrors());
            }
            GetUserCompanyListQueryHandler.ViewModel _companiesViewModel =
                await Mediator.Send(new GetUserCompanyListQueryHandler.ListQuery());
            ApplicationUser _user = await _userManager.FindByNameAsync(queryRequest.UserName);
            if (_user == null)
            {
                throw new UserCompanySelectionListQueryUserNotFoundException(queryRequest.UserName);
            }
            ViewModel _viewModel = new ViewModel();
            _viewModel.CompanyList = await _context.Companies
                .Where(_cmp => _companiesViewModel.CompanyList.Contains(_cmp.CompanyId))
                .Select(_c => _c.ToUserCompanySelectListItem(_user.CompanyId)).ToListAsync();
            if (_viewModel.CompanyList.Count == 0 )
            {
                _viewModel.CompanyList.Add(new SelectListItem("- no companies -", ""));
            }
            //
            return _viewModel;
        }
        //
        /// <summary>
        /// The Company list query class view class.
        /// </summary>
        public class ViewModel
        {
            public List<SelectListItem> CompanyList { get; set; }
        }
        //
        /// <summary>
        /// Get Company list query class (the primary key).
        /// </summary>
        public class ListQuery : IRequest<ViewModel>
        {
            public string UserName;
        }
        //
        /// <summary>
        /// FluentValidation of the 'CompanyListQuery' class.
        /// </summary>
        public class Validator : AbstractValidator<ListQuery>
        {
            //
            /// <summary>
            /// Constructor that will invoke the 'CompanyListQuery' validator.
            /// </summary>
            public Validator()
            {
                RuleFor(x => x.UserName).NotEmpty().MaximumLength(255);
            }
            //
        }
        //
    }
    //
    /// <summary>
    /// Custom UserCompanySelectionListQuery record not found exception.
    /// </summary>
    public class UserCompanySelectionListQueryUserNotFoundException : KeyNotFoundException
    {
        //
        /// <summary>
        /// Implementation of UserCompanySelectionListQuery record not found exception.
        /// </summary>
        /// <param name="user">User name for the record.</param>
        public UserCompanySelectionListQueryUserNotFoundException(string user)
            : base($"UserCompanySelectionListQuery key not found exception: Id: {user}")
        {
        }
    }
    //
    /// <summary>
    /// Custom CompanyListQuery validation exception.
    /// </summary>
    public class UserCompanySelectionListQueryValidationException : Exception
    {
        //
        /// <summary>
        /// Implementation of CompanyListQuery validation exception.
        /// </summary>
        /// <param name="errorMessage">The validation error messages.</param>
        public UserCompanySelectionListQueryValidationException(string errorMessage)
            : base($"CompanyListQuery validation exception: errors: {errorMessage}")
        {
        }
    }
    //
}
// ---------------------------------------------------------------------------
