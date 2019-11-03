//
// ---------------------------------------------------------------------------
// Servers delete command.
//
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
//
using Microsoft.EntityFrameworkCore;
using MediatR;
using FluentValidation;
using FluentValidation.Results;
using NSG.WebSrv.Domain.Entities;
using NSG.WebSrv.Application.Commands.Logs;
using NSG.WebSrv.Application.Infrastructure;
//
namespace NSG.WebSrv.Application.Commands.Servers
{
    //
    /// <summary>
    /// 'Server' delete command, handler and handle.
    /// </summary>
    public class ServerDeleteCommand : IRequest<int>
    {
        public int ServerId { get; set; }
    }
    //
    /// <summary>
    /// 'Server' delete command handler.
    /// </summary>
    public class ServerDeleteCommandHandler : IRequestHandler<ServerDeleteCommand, int>
    {
        private readonly IDb_Context _context;
        protected IMediator Mediator;
        //
        /// <summary>
        ///  The constructor for the inner handler class, to delete the Server entity.
        /// </summary>
        /// <param name="context">The database interface context.</param>
        public ServerDeleteCommandHandler(IDb_Context context, IMediator mediator)
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
        public async Task<int> Handle(ServerDeleteCommand request, CancellationToken cancellationToken)
        {
            Validator _validator = new Validator();
            ValidationResult _results = _validator.Validate(request);
            if (!_results.IsValid)
            {
                // Call the FluentValidationErrors extension method.
                throw new ServerDeleteCommandValidationException(_results.FluentValidationErrors());
            }
            var _entity = await _context.Servers
                .Include(nl => nl.NetworkLogs)
                .SingleOrDefaultAsync(r => r.ServerId == request.ServerId, cancellationToken);
            if (_entity == null)
            {
                throw new ServerDeleteCommandKeyNotFoundException(request.ServerId);
            }
            GetUserCompanyListQueryHandler.ViewModel _companiesViewModel =
                await Mediator.Send(new GetUserCompanyListQueryHandler.ListQuery());
            if (!_companiesViewModel.CompanyList.Contains(_entity.CompanyId))
            {
                throw new ServerDeleteCommandPermissionException($"User does not have permission for company: {_entity.CompanyId}");
            }
            // check any logs that do have an incident created.
            var netlogs = _entity.NetworkLogs.Where(_nl => _nl.IncidentId > 0);
            if (netlogs.Count() > 0)
            {
                throw new ServerDeleteCommandActiveNetworkLogsException(
                    string.Format("active NetworkLogs count: {0}", netlogs.Count()));
            }
            // remove any logs that do not have an incident created.
            foreach (NetworkLog _l in _entity.NetworkLogs)
            {
                _context.NetworkLogs.Remove(_l);
            }
            //
            _context.Servers.Remove(_entity);
            await _context.SaveChangesAsync(cancellationToken);
            await Mediator.Send(new LogCreateCommand() {
                Level = LoggingLevel.Warning, Method = MethodBase.GetCurrentMethod(),
                Message = "Deleted Server : " + _entity.ServerToString(), Exception = null });
            // Return the row count affected.
            return 1;
        }
        //
        /// <summary>
        /// FluentValidation of the 'ServerDeleteCommand' class.
        /// </summary>
        public class Validator : AbstractValidator<ServerDeleteCommand>
        {
            //
            /// <summary>
            /// Constructor that will invoke the 'ServerDeleteCommand' validator.
            /// </summary>
            public Validator()
            {
                //
                RuleFor(x => x.ServerId).NotNull();
                //
            }
            //
        }
        //
    }
    //
    /// <summary>
    /// Custom ServerDeleteCommand record not found exception.
    /// </summary>
    public class ServerDeleteCommandKeyNotFoundException : KeyNotFoundException
    {
        //
        /// <summary>
        /// Implementation of ServerDeleteCommand record not found exception.
        /// </summary>
        /// <param name="id">The key for the record.</param>
        public ServerDeleteCommandKeyNotFoundException(int serverId)
            : base($"ServerDeleteCommand key not found exception: id: {serverId}")
        {
        }
    }
    //
    /// <summary>
    /// Custom ServerDeleteCommand validation exception.
    /// </summary>
    public class ServerDeleteCommandValidationException : Exception
    {
        //
        /// <summary>
        /// Implementation of ServerDeleteCommand validation exception.
        /// </summary>
        /// <param name="errorMessage">The validation error messages.</param>
        public ServerDeleteCommandValidationException(string errorMessage)
            : base($"ServerDeleteCommand validation exception: errors: {errorMessage}")
        {
        }
    }
    //
    /// <summary>
    /// Custom ServerDeleteCommand validation exception.
    /// </summary>
    public class ServerDeleteCommandPermissionException : Exception
    {
        //
        /// <summary>
        /// Implementation of ServerDeleteCommand validation exception.
        /// </summary>
        /// <param name="errorMessage">The validation error messages.</param>
        public ServerDeleteCommandPermissionException(string errorMessage)
            : base($"ServerDeleteCommand permission exception: errors: {errorMessage}")
        {
        }
    }
    //
    /// <summary>
    /// Custom ServerDeleteCommand validation exception.
    /// </summary>
    public class ServerDeleteCommandActiveNetworkLogsException : Exception
    {
        //
        /// <summary>
        /// Implementation of ServerDeleteCommand validation exception.
        /// </summary>
        /// <param name="errorMessage">The validation error messages.</param>
        public ServerDeleteCommandActiveNetworkLogsException(string errorMessage)
            : base($"ServerDeleteCommand contains active incidents on NetworkLogs validation exception: errors: {errorMessage}")
        {
        }
    }
    //
}
// ---------------------------------------------------------------------------
