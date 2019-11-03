//
// ---------------------------------------------------------------------------
// Incident list query.
//
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
//
using MediatR;
using FluentValidation;
using FluentValidation.Results;
using Newtonsoft.Json;
//
using NSG.WebSrv.Domain.Entities;
using NSG.PrimeNG.LazyLoading;
using NSG.WebSrv.Infrastructure.Common;
//
namespace NSG.WebSrv.Application.Commands.Incidents
{
	//
	/// <summary>
	/// 'Incident' list query, handler and handle.
    ///  Incident Id: 52
    ///  Server Id: 4
    ///  IP Address: 103.46.138.150
    ///  NIC: apnic.net
    ///  Network Name: ZHIYUNET
    ///  Abuse Email Address: ipas@cnnic.cn
    ///  I S P Ticket Number: 
    ///  Mailed/Closed/Special: 
    ///  Notes: 
    ///  Created Date: 2018-12-19 20:42:28
	/// </summary>
	public class IncidentListQuery
	{
		[System.ComponentModel.DataAnnotations.Key]
		public long IncidentId { get; set; }
		public int ServerId { get; set; }
		public string IPAddress { get; set; }
		public string NIC_Id { get; set; }
		public string NetworkName { get; set; }
		public string AbuseEmailAddress { get; set; }
		public string ISPTicketNumber { get; set; }
		public bool Mailed { get; set; }
		public bool Closed { get; set; }
		public bool Special { get; set; }
		public string Notes { get; set; }
		public DateTime CreatedDate { get; set; }
	}
	//
	/// <summary>
	/// 'Incident' list query handler.
	/// </summary>
	public class IncidentListQueryHandler : IRequestHandler<IncidentListQueryHandler.ListQuery, IncidentListQueryHandler.ViewModel>
	{
		private readonly IDb_Context _context;
        private IApplication _application;
        //
        /// <summary>
        ///  The constructor for the inner handler class, to list the Incident entity.
        /// </summary>
        /// <param name="context">The database interface context.</param>
        public IncidentListQueryHandler(IDb_Context context, IApplication application)
		{
			_context = context;
            _application = application;
        }
        //
        /// <summary>
        /// 'Incident' query handle method, passing two interfaces.
        /// </summary>
        /// <param name="queryRequest">This list query request.</param>
        /// <param name="cancellationToken">Cancel token.</param>
        /// <returns>Returns a list of IncidentListQuery.</returns>
        public async Task<ViewModel> Handle(ListQuery queryRequest, CancellationToken cancellationToken)
		{
            if(_application.IsAuthenticated() == false)
            {
                throw new IncidentListQueryPermissionsException("user not authenticated.");
            }
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(queryRequest);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new IncidentListQueryValidationException(_results.FluentValidationErrors());
			}
            ViewModel _return = new ViewModel();
            _return.loadEvent = JsonConvert.DeserializeObject<LazyLoadEvent>(queryRequest.JsonString);
            IQueryable<Incident> _incidentQuery = _context.Incidents;
            _incidentQuery = _incidentQuery.LazyFilters(_return.loadEvent);
            if (!string.IsNullOrEmpty(_return.loadEvent.sortField))
            {
                _incidentQuery = _incidentQuery.LazyOrderBy(_return.loadEvent);
            }
            else // Default sort order
            {
                _incidentQuery = _incidentQuery.OrderByDescending(_r => _r.IncidentId);
            }
            // 'OrderBy' must be called before the method 'Skip'.
            _incidentQuery = _incidentQuery.LazySkipTake(_return.loadEvent);
            // Execute query and convert from Incident to IncidentData (POCO) ...
            _return.IncidentsList = await _incidentQuery
                .ToAsyncEnumerable().Select(incid => incid.ToIncidentListQuery()).ToList();
            _return.totalRecords = GetCountPagination(_return.loadEvent);
            //
            return _return;
		}
        // Return a count of filtered rows of Incident
        private long GetCountPagination(LazyLoadEvent jsonData)
        {
            IQueryable<Incident> _incidentQuery = _context.Incidents;
            _incidentQuery = _incidentQuery.LazyFilters(jsonData);
            long _incidentCount = _incidentQuery.Count();
            return _incidentCount;
        }
        //
        /// <summary>
        /// The Incident list query class view class.
        /// </summary>
        public class ViewModel
		{
            public IList<IncidentListQuery> IncidentsList { get; set; }
            //
            public LazyLoadEvent loadEvent;
            //
            public long totalRecords;
            //
            public string message;
            //
            public ViewModel()
            {
                IncidentsList = new List<IncidentListQuery>();
                loadEvent = null;
                totalRecords = 0;
                message = "";
            }
        }
        //
        /// <summary>
        /// Get Incident list query class (the primary key).
        /// </summary>
        public class ListQuery : IRequest<ViewModel>
		{
            public string JsonString { get; set; }
        }
        //
        /// <summary>
        /// FluentValidation of the 'IncidentListQuery' class.
        /// <example>
        /// An example of the JSON:
        /// {"first":0,"rows":3,"sortOrder":1,
        ///   "filters":{"ServerId":{"value":1,"matchMode":"eq"},
        ///     "Mailed":{"value":"false","matchMode":"eq"},
        ///     "Closed":{"value":"false","matchMode":"eq"},
        ///     "Special":{"value":"false","matchMode":"eq"}},
        ///    "globalFilter":null}
        /// </example>
        /// </summary>
        public class Validator : AbstractValidator<ListQuery>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'IncidentListQuery' validator.
			/// </summary>
			public Validator()
			{
                RuleFor(x => x.JsonString).NotNull()
                    .Must(Extensions.IsValidLazyLoadEventString)
                    .WithMessage("Invalid JSON paging request (LazyLoadEvent).");
                //
            }
            //
        }
		//
	}
	//
	/// <summary>
	/// Custom IncidentListQuery validation exception.
	/// </summary>
	public class IncidentListQueryValidationException : Exception
	{
		//
		/// <summary>
		/// Implementation of IncidentListQuery validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public IncidentListQueryValidationException(string errorMessage)
			: base($"IncidentListQuery validation exception: errors: {errorMessage}")
		{
		}
	}
    //
    /// <summary>
    /// Custom IncidentListQuery permissions exception.
    /// </summary>
    public class IncidentListQueryPermissionsException : Exception
    {
        //
        /// <summary>
        /// Implementation of IncidentListQuery permissions exception.
        /// </summary>
        /// <param name="errorMessage">The permissions error messages.</param>
        public IncidentListQueryPermissionsException(string errorMessage)
            : base($"IncidentListQuery permissions exception: {errorMessage}")
        {
        }
    }
    //
}
// ---------------------------------------------------------------------------
