
//
// ---------------------------------------------------------------------------
// NIC create command.
//
using System;
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
	/// 'NIC' create command, handler and handle.
	/// </summary>
	public class NICCreateCommand : IRequest<NIC>
	{
		public string NIC_Id { get; set; }
		public string NICDescription { get; set; }
		public string NICAbuseEmailAddress { get; set; }
		public string NICRestService { get; set; }
		public string NICWebSite { get; set; }
	}
	//
	/// <summary>
	/// 'NIC' create command handler.
	/// </summary>
	public class NICCreateCommandHandler : IRequestHandler<NICCreateCommand, NIC>
	{
		private readonly IDb_Context _context;
		//
		//
		/// <summary>
		///  The constructor for the inner handler class, to create the NIC entity.
		/// </summary>
		/// <param name="context">The database interface context.</param>
		public NICCreateCommandHandler(IDb_Context context)
		{
			_context = context;
		}
		//
		/// <summary>
		/// 'NIC' command handle method, passing two interfaces.
		/// </summary>
		/// <param name="request">This create command request.</param>
		/// <param name="cancellationToken">Cancel token.</param>
		/// <returns>The NIC entity class.</returns>
		public async Task<NIC> Handle(NICCreateCommand request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new CreateCommandValidationException(_results.FluentValidationErrors());
			}
			var _duplicate = await _context.NICs
				.SingleOrDefaultAsync(r => r.NIC_Id == request.NIC_Id, cancellationToken);
			if (_duplicate != null)
			{
				throw new CreateCommandDuplicateException(request.NIC_Id);
			}
			// Move from create command class to entity class.
			var _entity = new NIC
			{
				NIC_Id = request.NIC_Id,
				NICDescription = request.NICDescription,
				NICAbuseEmailAddress = request.NICAbuseEmailAddress,
				NICRestService = request.NICRestService,
				NICWebSite = request.NICWebSite,
			};
			_context.NICs.Add(_entity);
			await _context.SaveChangesAsync(cancellationToken);
			// Return the entity class.
			return _entity;
		}
		//
		/// <summary>
		/// FluentValidation of the 'NICCreateCommand' class.
		/// </summary>
		public class Validator : AbstractValidator<NICCreateCommand>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'NICCreateCommand' validator.
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
	/// Custom NICCreateCommand duplicate exception.
	/// </summary>
	public class CreateCommandDuplicateException: Exception
	{
		//
		/// <summary>
		/// Implementation of NICCreateCommand duplicate exception.
		/// </summary>
		/// <param name="id">The key for the record.</param>
		public CreateCommandDuplicateException(string id)
			: base($"NICCreateCommand duplicate id exception: Id: {id}")
		{
		}
	}
	//
	/// <summary>
	/// Custom NICCreateCommand validation exception.
	/// </summary>
	public class CreateCommandValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of NICCreateCommand validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public CreateCommandValidationException(string errorMessage)
			: base($"NICCreateCommand validation exception: errors: {errorMessage}")
		{
		}
	}
	//
}
// ---------------------------------------------------------------------------

