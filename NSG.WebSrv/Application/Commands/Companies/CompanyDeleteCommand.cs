//
// ---------------------------------------------------------------------------
// Companies delete command.
//
using System;
using System.Text;
using System.Collections.Generic;
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
//
namespace NSG.WebSrv.Application.Commands.Companies
{
    //
    /// <summary>
    /// 'Company' delete command, handler and handle.
    /// </summary>
    public class CompanyDeleteCommand : IRequest<int>
    {
        public int CompanyId { get; set; }
    }
    //
    /// <summary>
    /// 'Company' delete command handler.
    /// </summary>
    public class CompanyDeleteCommandHandler : IRequestHandler<CompanyDeleteCommand, int>
    {
        private readonly IDb_Context _context;
        protected IMediator Mediator;
        //
        /// <summary>
        ///  The constructor for the inner handler class, to delete the Company entity.
        /// </summary>
        /// <param name="context">The database interface context.</param>
        public CompanyDeleteCommandHandler(IDb_Context context, IMediator mediator)
        {
            _context = context;
            Mediator = mediator;
        }
        //
        /// <summary>
        /// 'Company' command handle method, passing two interfaces.
        /// </summary>
        /// <param name="request">This update command request.</param>
        /// <param name="cancellationToken">Cancel token.</param>
        /// <returns>Returns the row count.</returns>
        public async Task<int> Handle(CompanyDeleteCommand request, CancellationToken cancellationToken)
        {
            Validator _validator = new Validator();
            ValidationResult _results = _validator.Validate(request);
            if (!_results.IsValid)
            {
                // Call the FluentValidationErrors extension method.
                throw new DeleteCommandValidationException(_results.FluentValidationErrors());
            }
            var _entity = await _context.Companies
                .Include(cmp => cmp.Servers)
                .SingleOrDefaultAsync(r => r.CompanyId == request.CompanyId, cancellationToken);
            if (_entity == null)
            {
                throw new DeleteCommandKeyNotFoundException(request.CompanyId);
            }
            // require user to delete all servers before deleting company.
            if (_entity.Servers.Count > 0)
            {
                throw new CompanyDeleteCommandActiveServersException(
                    string.Format("Server count: {0}", _entity.Servers.Count));
            }
            //
            _context.Companies.Remove(_entity);
            await _context.SaveChangesAsync(cancellationToken);
            await Mediator.Send(new LogCreateCommand(
                LoggingLevel.Warning, MethodBase.GetCurrentMethod(),
                "Deleted Commpany : " + _entity.CompanyToString(), null));
            // Return the row count affected.
            return 1;
        }
        //
        /// <summary>
        /// FluentValidation of the 'CompanyDeleteCommand' class.
        /// </summary>
        public class Validator : AbstractValidator<CompanyDeleteCommand>
        {
            //
            /// <summary>
            /// Constructor that will invoke the 'CompanyDeleteCommand' validator.
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
    /// Custom CompanyDeleteCommand record not found exception.
    /// </summary>
    public class DeleteCommandKeyNotFoundException : KeyNotFoundException
    {
        //
        /// <summary>
        /// Implementation of CompanyDeleteCommand record not found exception.
        /// </summary>
        /// <param name="id">The key for the record.</param>
        public DeleteCommandKeyNotFoundException(int id)
            : base($"CompanyDeleteCommand not found exception: Id: {id}")
        {
        }
    }
    //
    /// <summary>
    /// Custom CompanyDeleteCommand validation exception.
    /// </summary>
    public class DeleteCommandValidationException : Exception
    {
        //
        /// <summary>
        /// Implementation of CompanyDeleteCommand validation exception.
        /// </summary>
        /// <param name="errorMessage">The validation error messages.</param>
        public DeleteCommandValidationException(string errorMessage)
            : base($"CompanyDeleteCommand validation exception: errors: {errorMessage}")
        {
        }
    }
    //
    /// <summary>
    /// Custom CompanyDeleteCommand validation exception.
    /// </summary>
    public class CompanyDeleteCommandActiveServersException : Exception
    {
        //
        /// <summary>
        /// Implementation of ServerDeleteCommand validation exception.
        /// </summary>
        /// <param name="errorMessage">The validation error messages.</param>
        public CompanyDeleteCommandActiveServersException(string errorMessage)
            : base($"CompanyDeleteCommand contains server exception (delete all servers): errors: {errorMessage}")
        {
        }
    }
    //
}
// ---------------------------------------------------------------------------
