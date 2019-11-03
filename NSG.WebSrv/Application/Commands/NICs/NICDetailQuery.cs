
//
// ---------------------------------------------------------------------------
// NIC detail query.
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
//
namespace NSG.WebSrv.Application.Commands.NICs
{
	//
	/// <summary>
	/// 'NIC' detail query, handler and handle.
	/// </summary>
	public class NICDetailQuery
	{
		[System.ComponentModel.DataAnnotations.Key]
		public string NIC_Id { get; set; }
		public string NICDescription { get; set; }
		public string NICAbuseEmailAddress { get; set; }
		public string NICRestService { get; set; }
		public string NICWebSite { get; set; }
	}
	//
	/// <summary>
	/// 'NIC' detail query handler.
	/// </summary>
	public class NICDetailQueryHandler : IRequestHandler<NICDetailQueryHandler.DetailQuery, NICDetailQuery>
	{
		private readonly IDb_Context _context;
		//
		/// <summary>
		///  The constructor for the inner handler class, to detail the NIC entity.
		/// </summary>
		/// <param name="context">The database interface context.</param>
		public NICDetailQueryHandler(IDb_Context context)
		{
			_context = context;
		}
		//
		/// <summary>
		/// 'NIC' query handle method, passing two interfaces.
		/// </summary>
		/// <param name="request">This detail query request.</param>
		/// <param name="cancellationToken">Cancel token.</param>
		/// <returns>Returns the row count.</returns>
		public async Task<NICDetailQuery> Handle(DetailQuery request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new DetailQueryValidationException(_results.FluentValidationErrors());
			}
			var _entity = await GetEntityByKey(request.NIC_Id);
			if (_entity == null)
			{
				throw new DetailQueryKeyNotFoundException(request.NIC_Id);
			}
			//
			// Return the detail query model.
			return _entity.ToNICDetailQuery();
		}
		//
		/// <summary>
		/// Get an entity record via the primary key.
		/// </summary>
		/// <param name="nIC_Id">string key</param>
		/// <returns>Returns a NIC entity record.</returns>
		private Task<NIC> GetEntityByKey(string nIC_Id)
		{
			return _context.NICs.SingleOrDefaultAsync(r => r.NIC_Id == nIC_Id);
		}
		//
		/// <summary>
		/// Get NIC detail query class (the primary key).
		/// </summary>
		public class DetailQuery : IRequest<NICDetailQuery>
		{
			public string NIC_Id { get; set; }
		}
		//
		/// <summary>
		/// FluentValidation of the 'NICDetailQuery' class.
		/// </summary>
		public class Validator : AbstractValidator<DetailQuery>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'NICDetailQuery' validator.
			/// </summary>
			public Validator()
			{
				//
				RuleFor(x => x.NIC_Id).NotEmpty().MaximumLength(16);
				//
			}
			//
		}
		//
	}
	//
	/// <summary>
	/// Custom NICDetailQuery record not found exception.
	/// </summary>
	public class DetailQueryKeyNotFoundException: KeyNotFoundException
	{
		//
		/// <summary>
		/// Implementation of NICDetailQuery record not found exception.
		/// </summary>
		/// <param name="id">The key for the record.</param>
		public DetailQueryKeyNotFoundException(string nIC_Id)
			: base($"NICDetailQuery key not found exception: Id: {nIC_Id}")
		{
		}
	}
	//
	/// <summary>
	/// Custom NICDetailQuery validation exception.
	/// </summary>
	public class DetailQueryValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of NICDetailQuery validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public DetailQueryValidationException(string errorMessage)
			: base($"NICDetailQuery validation exception: errors: {errorMessage}")
		{
		}
	}
	//
}
// ---------------------------------------------------------------------------

