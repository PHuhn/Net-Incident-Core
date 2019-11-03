//
// ---------------------------------------------------------------------------
// EmailTemplates list query.
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
using NSG.WebSrv.Application.Infrastructure;
//
namespace NSG.WebSrv.Application.Commands.CompanyEmailTemplates
{
	//
	/// <summary>
	/// 'EmailTemplate' list query, handler and handle.
	/// </summary>
	public class CompanyEmailTemplateListQuery
	{
		[System.ComponentModel.DataAnnotations.Key]
		public int CompanyId { get; set; }
		[System.ComponentModel.DataAnnotations.Key]
		public int IncidentTypeId { get; set; }
        //
        /// <summary>
        /// For column IncidentTypeShortDesc
        /// Foreign key description for IncidentTypeId
        /// </summary>
        public string IncidentTypeShortDesc { get; set; }
        /// <summary>
        /// For column IncidentTypeDesc
        /// Foreign key description for IncidentTypeId
        /// </summary>
        public string IncidentTypeDesc { get; set; }
        public string SubjectLine { get; set; }
		public string EmailBody { get; set; }
		//public string TimeTemplate { get; set; }
		//public string ThanksTemplate { get; set; }
		//public string LogTemplate { get; set; }
		//public string Template { get; set; }
		public bool FromServer { get; set; }
	}
	//
	/// <summary>
	/// 'EmailTemplate' list query handler.
	/// </summary>
	public class CompanyEmailTemplateListQueryHandler : IRequestHandler<CompanyEmailTemplateListQueryHandler.ListQuery, CompanyEmailTemplateListQueryHandler.ViewModel>
	{
		private readonly IDb_Context _context;
        private IMediator Mediator;
        //
        /// <summary>
        ///  The constructor for the inner handler class, to list the EmailTemplate entity.
        /// </summary>
        /// <param name="context">The database interface context.</param>
        public CompanyEmailTemplateListQueryHandler(IDb_Context context, IMediator mediator)
		{
			_context = context;
            Mediator = mediator;
        }
		//
		/// <summary>
		/// 'EmailTemplate' query handle method, passing two interfaces.
		/// </summary>
		/// <param name="queryRequest">This list query request.</param>
		/// <param name="cancellationToken">Cancel token.</param>
		/// <returns>Returns a list of EmailTemplateListQuery.</returns>
		public async Task<ViewModel> Handle(ListQuery queryRequest, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(queryRequest);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new ListQueryValidationException(_results.FluentValidationErrors());
			}
            // Check permissions
            GetUserCompanyListQueryHandler.ViewModel _companiesViewModel =
                await Mediator.Send(new GetUserCompanyListQueryHandler.ListQuery());
            if (!_companiesViewModel.CompanyList.Contains(queryRequest.CompanyId))
            {
                throw new CompanyEmailTemplateListQueryPermissionException(
                    $"User does not have permission for company: {queryRequest.CompanyId}");
            }
            //
            return new ViewModel()
			{
				EmailTemplatesList = await _context.EmailTemplates
                    .Where(_et => _et.CompanyId == queryRequest.CompanyId)
                    .Include(_in => _in.IncidentType)
                    .ToAsyncEnumerable()
					.Select(cnt => cnt.ToCompanyEmailTemplateListQuery()).ToList()
			};
		}
		//
		/// <summary>
		/// The EmailTemplate list query class view class.
		/// </summary>
		public class ViewModel
		{
			public List<CompanyEmailTemplateListQuery> EmailTemplatesList { get; set; }
		}
		//
		/// <summary>
		/// Get EmailTemplate list query class (the primary key).
		/// </summary>
		public class ListQuery : IRequest<ViewModel>
		{
            public int CompanyId { get; set; }
        }
		//
		/// <summary>
		/// FluentValidation of the 'EmailTemplateListQuery' class.
		/// </summary>
		public class Validator : AbstractValidator<ListQuery>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'EmailTemplateListQuery' validator.
			/// </summary>
			public Validator()
			{
                //
                RuleFor(x => x.CompanyId).NotNull();
                //
            }
            //
        }
		//
	}
	//
	/// <summary>
	/// Custom EmailTemplateListQuery validation exception.
	/// </summary>
	public class ListQueryValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of EmailTemplateListQuery validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public ListQueryValidationException(string errorMessage)
			: base($"EmailTemplateListQuery validation exception: errors: {errorMessage}")
		{
		}
	}
    //
    /// <summary>
    /// Custom EmailTemplateListQuery permission exception.
    /// </summary>
    public class CompanyEmailTemplateListQueryPermissionException : Exception
    {
        //
        /// <summary>
        /// Implementation of EmailTemplateListQuery permission exception.
        /// </summary>
        /// <param name="errorMessage">The permission error messages.</param>
        public CompanyEmailTemplateListQueryPermissionException(string errorMessage)
            : base($"EmailTemplateListQuery validation exception: errors: {errorMessage}")
        {
        }
    }
    // PermissionException
}
// ---------------------------------------------------------------------------
