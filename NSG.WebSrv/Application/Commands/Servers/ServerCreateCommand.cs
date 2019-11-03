//
// ---------------------------------------------------------------------------
// Servers create command.
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
using NSG.WebSrv.Application.Infrastructure;
//
namespace NSG.WebSrv.Application.Commands.Servers
{
	//
	/// <summary>
	/// 'Server' create command, handler and handle.
	/// </summary>
	public class ServerCreateCommand : IRequest<Server>
	{
		public int ServerId { get; set; }
		public int CompanyId { get; set; }
		public string ServerShortName { get; set; }
		public string ServerName { get; set; }
		public string ServerDescription { get; set; }
		public string WebSite { get; set; }
		public string ServerLocation { get; set; }
		public string FromName { get; set; }
		public string FromNicName { get; set; }
		public string FromEmailAddress { get; set; }
		public string TimeZone { get; set; }
		public bool DST { get; set; }
		public string TimeZone_DST { get; set; }
		public DateTime? DST_Start { get; set; }
		public DateTime? DST_End { get; set; }
	}
	//
	/// <summary>
	/// 'Server' create command handler.
	/// </summary>
	public class ServerCreateCommandHandler : IRequestHandler<ServerCreateCommand, Server>
	{
		private readonly IDb_Context _context;
        private IMediator Mediator;
        //
        //
        /// <summary>
        ///  The constructor for the inner handler class, to create the Server entity.
        /// </summary>
        /// <param name="context">The database interface context.</param>
        public ServerCreateCommandHandler(IDb_Context context, IMediator mediator)
		{
			_context = context;
            Mediator = mediator;

        }
		//
		/// <summary>
		/// 'Server' command handle method, passing two interfaces.
		/// </summary>
		/// <param name="request">This create command request.</param>
		/// <param name="cancellationToken">Cancel token.</param>
		/// <returns>The Server entity class.</returns>
		public async Task<Server> Handle(ServerCreateCommand request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new ServerCreateCommandValidationException(_results.FluentValidationErrors());
			}
            // Check permissions
            GetUserCompanyListQueryHandler.ViewModel _companiesViewModel =
                await Mediator.Send(new GetUserCompanyListQueryHandler.ListQuery());
            if( !_companiesViewModel.CompanyList.Contains(request.CompanyId))
            {
                throw new ServerCreateCommandPermissionException($"User does not have permission for company: {request.CompanyId}");
            }
            // Move from create command class to entity class.
            var _entity = new Server
			{
				ServerId = request.ServerId,
				CompanyId = request.CompanyId,
				ServerShortName = request.ServerShortName,
				ServerName = request.ServerName,
				ServerDescription = request.ServerDescription,
				WebSite = request.WebSite,
				ServerLocation = request.ServerLocation,
				FromName = request.FromName,
				FromNicName = request.FromNicName,
				FromEmailAddress = request.FromEmailAddress,
				TimeZone = request.TimeZone,
				DST = request.DST,
				TimeZone_DST = request.TimeZone_DST,
				DST_Start = request.DST_Start,
				DST_End = request.DST_End,
			};
			_context.Servers.Add(_entity);
			await _context.SaveChangesAsync(cancellationToken);
			// Return the entity class.
			return _entity;
		}
		//
		/// <summary>
		/// FluentValidation of the 'ServerCreateCommand' class.
		/// </summary>
		public class Validator : AbstractValidator<ServerCreateCommand>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'ServerCreateCommand' validator.
			/// </summary>
			public Validator()
			{
				//
				RuleFor(x => x.CompanyId).NotNull();
				RuleFor(x => x.ServerShortName).NotEmpty().MaximumLength(12);
				RuleFor(x => x.ServerName).NotEmpty().MaximumLength(80);
				RuleFor(x => x.ServerDescription).NotEmpty().MaximumLength(255);
				RuleFor(x => x.WebSite).NotEmpty().MaximumLength(255);
				RuleFor(x => x.ServerLocation).NotEmpty().MaximumLength(255);
				RuleFor(x => x.FromName).NotEmpty().MaximumLength(255);
				RuleFor(x => x.FromNicName).NotEmpty().MaximumLength(16);
				RuleFor(x => x.FromEmailAddress).NotEmpty().MaximumLength(255);
				RuleFor(x => x.TimeZone).NotEmpty().MaximumLength(16);
				RuleFor(x => x.DST).NotNull();
				RuleFor(x => x.TimeZone_DST).MaximumLength(16);
                When(srv => srv.DST, () => {
                    RuleFor(x => x.DST_Start).NotNull().WithMessage("DST Start is required when DST is set.");
                    RuleFor(x => x.DST_End).NotNull().WithMessage("DST End is required when DST is set.");
                });
                //
            }
            //
        }
		//
	}
	//
	/// <summary>
	/// Custom ServerCreateCommand validation exception.
	/// </summary>
	public class ServerCreateCommandValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of ServerCreateCommand validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public ServerCreateCommandValidationException(string errorMessage)
			: base($"ServerCreateCommand validation exception: errors: {errorMessage}")
		{
		}
	}
    //
    /// <summary>
    /// Custom ServerCreateCommand validation exception.
    /// </summary>
    public class ServerCreateCommandPermissionException : Exception
    {
        //
        /// <summary>
        /// Implementation of ServerCreateCommand validation exception.
        /// </summary>
        /// <param name="errorMessage">The validation error messages.</param>
        public ServerCreateCommandPermissionException(string errorMessage)
            : base($"ServerCreateCommand permission exception: errors: {errorMessage}")
        {
        }
    }
    //
}
// ---------------------------------------------------------------------------

