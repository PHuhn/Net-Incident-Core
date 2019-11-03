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
using Microsoft.AspNetCore.Mvc.Rendering;
//
namespace NSG.WebSrv.Application.Commands.CompanyEmailTemplates
{
	//
	/// <summary>
	/// 'EmailTemplate' list query handler.
	/// </summary>
	public class CompanyEmailTemplateSelectionListQueryHandler : IRequestHandler<CompanyEmailTemplateSelectionListQueryHandler.ListQuery, CompanyEmailTemplateSelectionListQueryHandler.ViewModel>
	{
		private readonly IDb_Context _context;
		//
		/// <summary>
		///  The constructor for the inner handler class, to list the EmailTemplate entity.
		/// </summary>
		/// <param name="context">The database interface context.</param>
		public CompanyEmailTemplateSelectionListQueryHandler(IDb_Context context)
		{
			_context = context;
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
				throw new CompanyEmailTemplateSelectionListQueryValidationException(_results.FluentValidationErrors());
			}
			//
			return new ViewModel()
			{
				EmailTemplatesList = await _context.EmailTemplates
                    .Where(_et => _et.CompanyId == queryRequest.CompanyId)
                    .Include(_in => _in.IncidentType)
                    .ToAsyncEnumerable()
					.Select(cnt => cnt.ToCompanyEmailTemplateSelectionListQuery()).ToList()
			};
		}
		//
		/// <summary>
		/// The EmailTemplate list query class view class.
		/// </summary>
		public class ViewModel
		{
			public List<SelectListItem> EmailTemplatesList { get; set; }
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
    /// Custom CompanyEmailTemplateSelectionListQuery validation exception.
    /// </summary>
    public class CompanyEmailTemplateSelectionListQueryValidationException : Exception
	{
		//
		/// <summary>
		/// Implementation of EmailTemplateListQuery validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public CompanyEmailTemplateSelectionListQueryValidationException(string errorMessage)
			: base($"EmailTemplateListQuery validation exception: errors: {errorMessage}")
		{
		}
	}
	//
}
// ---------------------------------------------------------------------------
