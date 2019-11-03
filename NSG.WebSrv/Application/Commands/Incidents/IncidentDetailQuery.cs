//
// ---------------------------------------------------------------------------
// Incident detail query.
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
using NSG.WebSrv.Infrastructure.Common;
//
namespace NSG.WebSrv.Application.Commands.Incidents
{
	//
	/// <summary>
	/// 'Incident' detail query, handler and handle.
	/// </summary>
	public class IncidentDetailQuery
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
	/// 'Incident' detail query handler.
	/// </summary>
	public class IncidentDetailQueryHandler : IRequestHandler<IncidentDetailQueryHandler.DetailQuery, IncidentDetailQuery>
	{
		private readonly IDb_Context _context;
        private IApplication _application;
        //
        /// <summary>
        ///  The constructor for the inner handler class, to detail the Incident entity.
        /// </summary>
        /// <param name="context">The database interface context.</param>
        public IncidentDetailQueryHandler(IDb_Context context)
		{
			_context = context;
		}
		//
		/// <summary>
		/// 'Incident' query handle method, passing two interfaces.
		/// </summary>
		/// <param name="request">This detail query request.</param>
		/// <param name="cancellationToken">Cancel token.</param>
		/// <returns>Returns the row count.</returns>
		public async Task<IncidentDetailQuery> Handle(DetailQuery request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new DetailQueryValidationException(_results.FluentValidationErrors());
			}
			var _entity = await GetEntityByKey(request.IncidentId);
			if (_entity == null)
			{
				throw new DetailQueryKeyNotFoundException(request.IncidentId);
			}
			//
			// Return the detail query model.
			return _entity.ToIncidentDetailQuery();
		}
		//
		/// <summary>
		/// Get an entity record via the primary key.
		/// </summary>
		/// <param name="incidentId">long key</param>
		/// <returns>Returns a Incident entity record.</returns>
		private Task<Incident> GetEntityByKey(long incidentId)
		{
			return _context.Incidents.SingleOrDefaultAsync(r => r.IncidentId == incidentId);
		}
		//
		/// <summary>
		/// Get Incident detail query class (the primary key).
		/// </summary>
		public class DetailQuery : IRequest<IncidentDetailQuery>
		{
			public long IncidentId { get; set; }
		}
		//
		/// <summary>
		/// FluentValidation of the 'IncidentDetailQuery' class.
		/// </summary>
		public class Validator : AbstractValidator<DetailQuery>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'IncidentDetailQuery' validator.
			/// </summary>
			public Validator()
			{
				//
				RuleFor(x => x.IncidentId).NotNull();
				//
			}
			//
		}
		//
	}
	//
	/// <summary>
	/// Custom IncidentDetailQuery record not found exception.
	/// </summary>
	public class DetailQueryKeyNotFoundException: KeyNotFoundException
	{
		//
		/// <summary>
		/// Implementation of IncidentDetailQuery record not found exception.
		/// </summary>
		/// <param name="id">The key for the record.</param>
		public DetailQueryKeyNotFoundException(long incidentId)
			: base($"IncidentDetailQuery key not found exception: Id: {incidentId}")
		{
		}
	}
	//
	/// <summary>
	/// Custom IncidentDetailQuery validation exception.
	/// </summary>
	public class DetailQueryValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of IncidentDetailQuery validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public DetailQueryValidationException(string errorMessage)
			: base($"IncidentDetailQuery validation exception: errors: {errorMessage}")
		{
		}
	}
	//
}
// ---------------------------------------------------------------------------
