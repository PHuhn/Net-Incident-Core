//
// ---------------------------------------------------------------------------
// EmailTemplates detail query.
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
using System.Linq;
//
namespace NSG.WebSrv.Application.Commands.CompanyEmailTemplates
{
    //
    /// <summary>
    /// 'EmailTemplate' detail query, handler and handle.
    /// </summary>
    public class CompanyEmailTemplateDetailQuery
	{
		[System.ComponentModel.DataAnnotations.Key]
		public int CompanyId { get; set; }
        public string CompanyShortName { get; set; }
        public string CompanyName { get; set; }
        [System.ComponentModel.DataAnnotations.Key]
		public int IncidentTypeId { get; set; }
        public string IncidentTypeShortDesc { get; set; }
        public string IncidentTypeDesc { get; set; }
        public string SubjectLine { get; set; }
		public string EmailBody { get; set; }
		public string TimeTemplate { get; set; }
		public string ThanksTemplate { get; set; }
		public string LogTemplate { get; set; }
		public string Template { get; set; }
		public bool FromServer { get; set; }
	}
	//
	/// <summary>
	/// 'EmailTemplate' detail query handler.
	/// </summary>
	public class CompanyEmailTemplateDetailQueryHandler : IRequestHandler<CompanyEmailTemplateDetailQueryHandler.DetailQuery, CompanyEmailTemplateDetailQuery>
	{
		private readonly IDb_Context _context;
		//
		/// <summary>
		///  The constructor for the inner handler class, to detail the EmailTemplate entity.
		/// </summary>
		/// <param name="context">The database interface context.</param>
		public CompanyEmailTemplateDetailQueryHandler(IDb_Context context)
		{
			_context = context;
		}
		//
		/// <summary>
		/// 'EmailTemplate' query handle method, passing two interfaces.
		/// </summary>
		/// <param name="request">This detail query request.</param>
		/// <param name="cancellationToken">Cancel token.</param>
		/// <returns>Returns the row count.</returns>
		public async Task<CompanyEmailTemplateDetailQuery> Handle(DetailQuery request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new DetailQueryValidationException(_results.FluentValidationErrors());
			}
			var _entity = await GetEntityByKey(request.CompanyId, request.IncidentTypeId);
			if (_entity == null)
			{
				throw new DetailQueryKeyNotFoundException(request.CompanyId, request.IncidentTypeId);
			}
			//
			// Return the detail query model.
			return _entity.ToCompanyEmailTemplateDetailQuery();
		}
		//
		/// <summary>
		/// Get an entity record via the primary key.
		/// </summary>
		/// <param name="companyId">int key</param>
		/// <param name="incidentTypeId">int key</param>
		/// <returns>Returns a EmailTemplate entity record.</returns>
		private Task<EmailTemplate> GetEntityByKey(int companyId, int incidentTypeId)
		{
            return _context.EmailTemplates
                .Where(_et => _et.CompanyId == companyId && _et.IncidentTypeId == incidentTypeId)
                .Include(_c => _c.Company)
                .Include(_it => _it.IncidentType)
                .SingleOrDefaultAsync();
        }
        //
        /// <summary>
        /// Get EmailTemplate detail query class (the primary key).
        /// </summary>
        public class DetailQuery : IRequest<CompanyEmailTemplateDetailQuery>
		{
			public int CompanyId { get; set; }
			public int IncidentTypeId { get; set; }
		}
		//
		/// <summary>
		/// FluentValidation of the 'EmailTemplateDetailQuery' class.
		/// </summary>
		public class Validator : AbstractValidator<DetailQuery>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'EmailTemplateDetailQuery' validator.
			/// </summary>
			public Validator()
			{
				//
				RuleFor(x => x.CompanyId).NotNull();
				RuleFor(x => x.IncidentTypeId).NotNull();
				//
			}
			//
		}
		//
	}
	//
	/// <summary>
	/// Custom EmailTemplateDetailQuery record not found exception.
	/// </summary>
	public class DetailQueryKeyNotFoundException: KeyNotFoundException
	{
		//
		/// <summary>
		/// Implementation of EmailTemplateDetailQuery record not found exception.
		/// </summary>
		/// <param name="id">The key for the record.</param>
		public DetailQueryKeyNotFoundException(int companyId, int incidentTypeId)
			: base($"EmailTemplateDetailQuery key not found exception: Id: {companyId}-{incidentTypeId}")
		{
		}
	}
	//
	/// <summary>
	/// Custom EmailTemplateDetailQuery validation exception.
	/// </summary>
	public class DetailQueryValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of EmailTemplateDetailQuery validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public DetailQueryValidationException(string errorMessage)
			: base($"EmailTemplateDetailQuery validation exception: errors: {errorMessage}")
		{
		}
	}
    //
}
// ---------------------------------------------------------------------------
