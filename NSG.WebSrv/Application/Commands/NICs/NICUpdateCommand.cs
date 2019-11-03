
//
// ---------------------------------------------------------------------------
// NIC update command.
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
	/// 'NIC' update command, handler and handle.
	/// </summary>
	public class NICUpdateCommand : IRequest<int>
	{
		public string NIC_Id { get; set; }
		public string NICDescription { get; set; }
		public string NICAbuseEmailAddress { get; set; }
		public string NICRestService { get; set; }
		public string NICWebSite { get; set; }
	}
	//
	/// <summary>
	/// 'NIC' update command handler.
	/// </summary>
	public class NICUpdateCommandHandler : IRequestHandler<NICUpdateCommand, int>
	{
		private readonly IDb_Context _context;
		//
		/// <summary>
		///  The constructor for the inner handler class, to update the NIC entity.
		/// </summary>
		/// <param name="context">The database interface context.</param>
		public NICUpdateCommandHandler(IDb_Context context)
		{
			_context = context;
		}
		//
		/// <summary>
		/// 'NIC' command handle method, passing two interfaces.
		/// </summary>
		/// <param name="request">This update command request.</param>
		/// <param name="cancellationToken">Cancel token.</param>
		/// <returns>Returns the row count.</returns>
		public async Task<int> Handle(NICUpdateCommand request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new UpdateCommandValidationException(_results.FluentValidationErrors());
			}
			var _entity = await _context.NICs
				.SingleOrDefaultAsync(r => r.NIC_Id == request.NIC_Id, cancellationToken);
			if (_entity == null)
			{
				throw new UpdateCommandKeyNotFoundException(request.NIC_Id);
			}
			// Move from update command class to entity class.
			_entity.NICDescription = request.NICDescription;
			_entity.NICAbuseEmailAddress = request.NICAbuseEmailAddress;
			_entity.NICRestService = request.NICRestService;
			_entity.NICWebSite = request.NICWebSite;
			//
			await _context.SaveChangesAsync(cancellationToken);
			// Return the row count.
			return 1;
		}
		//
		/// <summary>
		/// FluentValidation of the 'NICUpdateCommand' class.
		/// </summary>
		public class Validator : AbstractValidator<NICUpdateCommand>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'NICUpdateCommand' validator.
			/// </summary>
			public Validator()
			{
				//
				RuleFor(x => x.NIC_Id).NotEmpty().MaximumLength(16);
				RuleFor(x => x.NICDescription).NotEmpty().MaximumLength(255);
				RuleFor(x => x.NICAbuseEmailAddress).MaximumLength(50);
				RuleFor(x => x.NICRestService).MaximumLength(255);
				RuleFor(x => x.NICWebSite).MaximumLength(255);
				//
			}
			//
		}
		//
	}
	//
	/// <summary>
	/// Custom NICUpdateCommand record not found exception.
	/// </summary>
	public class UpdateCommandKeyNotFoundException: KeyNotFoundException
	{
		//
		/// <summary>
		/// Implementation of NICUpdateCommand record not found exception.
		/// </summary>
		/// <param name="id">The key for the record.</param>
		public UpdateCommandKeyNotFoundException(string nIC_Id)
			: base($"NICUpdateCommand key not found exception: id: {nIC_Id}")
		{
		}
	}
	//
	/// <summary>
	/// Custom NICUpdateCommand validation exception.
	/// </summary>
	public class UpdateCommandValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of NICUpdateCommand validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public UpdateCommandValidationException(string errorMessage)
			: base($"NICUpdateCommand validation exception: errors: {errorMessage}")
		{
		}
	}
}
// ---------------------------------------------------------------------------

