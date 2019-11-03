//
// ---------------------------------------------------------------------------
// Servers update command.
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
using NSG.WebSrv.Application.Infrastructure;
//
namespace NSG.WebSrv.Application.Commands.Servers
{
	//
	/// <summary>
	/// 'Server' update command, handler and handle.
	/// </summary>
	public class ServerUpdateCommand : IRequest<int>
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
	/// 'Server' update command handler.
	/// </summary>
	public class ServerUpdateCommandHandler : IRequestHandler<ServerUpdateCommand, int>
	{
		private readonly IDb_Context _context;
        private IMediator Mediator;
        //
        /// <summary>
        ///  The constructor for the inner handler class, to update the Server entity.
        /// </summary>
        /// <param name="context">The database interface context.</param>
        public ServerUpdateCommandHandler(IDb_Context context, IMediator mediator)
		{
			_context = context;
            Mediator = mediator;
        }
		//
		/// <summary>
		/// 'Server' command handle method, passing two interfaces.
		/// </summary>
		/// <param name="request">This update command request.</param>
		/// <param name="cancellationToken">Cancel token.</param>
		/// <returns>Returns the row count.</returns>
		public async Task<int> Handle(ServerUpdateCommand request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new ServerUpdateCommandValidationException(_results.FluentValidationErrors());
			}
            GetUserCompanyListQueryHandler.ViewModel _companiesViewModel =
                await Mediator.Send(new GetUserCompanyListQueryHandler.ListQuery());
            if (!_companiesViewModel.CompanyList.Contains(request.CompanyId))
            {
                throw new ServerUpdateCommandPermissionException($"User does not have permission for company: {request.CompanyId}");
            }
            Server _entity = await _context.Servers
				.SingleOrDefaultAsync(r => r.ServerId == request.ServerId, cancellationToken);
			if (_entity == null)
			{
				throw new UpdateCommandKeyNotFoundException(request.ServerId);
			}
			// Move from update command class to entity class.
			_entity.CompanyId = request.CompanyId;
			_entity.ServerShortName = request.ServerShortName;
			_entity.ServerName = request.ServerName;
			_entity.ServerDescription = request.ServerDescription;
			_entity.WebSite = request.WebSite;
			_entity.ServerLocation = request.ServerLocation;
			_entity.FromName = request.FromName;
			_entity.FromNicName = request.FromNicName;
			_entity.FromEmailAddress = request.FromEmailAddress;
			_entity.TimeZone = request.TimeZone;
			_entity.DST = request.DST;
			_entity.TimeZone_DST = request.TimeZone_DST;
			_entity.DST_Start = request.DST_Start;
			_entity.DST_End = request.DST_End;
			//
			await _context.SaveChangesAsync(cancellationToken);
			// Return the row count.
			return 1;
		}
		//
		/// <summary>
		/// FluentValidation of the 'ServerUpdateCommand' class.
		/// </summary>
		public class Validator : AbstractValidator<ServerUpdateCommand>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'ServerUpdateCommand' validator.
			/// </summary>
			public Validator()
			{
				//
				RuleFor(x => x.ServerId).NotNull();
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
	/// Custom ServerUpdateCommand record not found exception.
	/// </summary>
	public class UpdateCommandKeyNotFoundException: KeyNotFoundException
	{
		//
		/// <summary>
		/// Implementation of ServerUpdateCommand record not found exception.
		/// </summary>
		/// <param name="id">The key for the record.</param>
		public UpdateCommandKeyNotFoundException(int serverId)
			: base($"ServerUpdateCommand key not found exception: id: {serverId}")
		{
		}
	}
	//
	/// <summary>
	/// Custom ServerUpdateCommand validation exception.
	/// </summary>
	public class ServerUpdateCommandValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of ServerUpdateCommand validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public ServerUpdateCommandValidationException(string errorMessage)
			: base($"ServerUpdateCommand validation exception: errors: {errorMessage}")
		{
		}
	}
    //
    /// <summary>
    /// Custom ServerUpdateCommand permission exception.
    /// </summary>
    public class ServerUpdateCommandPermissionException : Exception
    {
        //
        /// <summary>
        /// Implementation of ServerUpdateCommand validation exception.
        /// </summary>
        /// <param name="errorMessage">The validation error messages.</param>
        public ServerUpdateCommandPermissionException(string errorMessage)
            : base($"ServerUpdateCommand validation exception: errors: {errorMessage}")
        {
        }
    }
    //
}
// ---------------------------------------------------------------------------
