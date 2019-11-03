//
// ---------------------------------------------------------------------------
// Companies update command.
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
namespace NSG.WebSrv.Application.Commands.Companies
{
    //
    /// <summary>
    /// 'Company' update command, handler and handle.
    /// </summary>
    public class CompanyUpdateCommand : IRequest<int>
    {
        public int CompanyId { get; set; }
        public string CompanyShortName { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string Notes { get; set; }
    }
    //
    /// <summary>
    /// 'Company' update command handler.
    /// </summary>
    public class CompanyUpdateCommandHandler : IRequestHandler<CompanyUpdateCommand, int>
    {
        private readonly IDb_Context _context;
        private IMediator Mediator;
        //
        /// <summary>
        ///  The constructor for the inner handler class, to update the Company entity.
        /// </summary>
        /// <param name="context">The database interface context.</param>
        public CompanyUpdateCommandHandler(IDb_Context context, IMediator mediator)
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
        public async Task<int> Handle(CompanyUpdateCommand request, CancellationToken cancellationToken)
        {
            Validator _validator = new Validator();
            ValidationResult _results = _validator.Validate(request);
            if (!_results.IsValid)
            {
                // Call the FluentValidationErrors extension method.
                throw new CompanyUpdateCommandValidationException(_results.FluentValidationErrors());
            }
            // Check permissions
            GetUserCompanyListQueryHandler.ViewModel _companiesViewModel =
                await Mediator.Send(new GetUserCompanyListQueryHandler.ListQuery());
            if (!_companiesViewModel.CompanyList.Contains(request.CompanyId))
            {
                throw new CompanyUpdateCommandPermissionException($"User does not have permission for company: {request.CompanyId}");
            }
            //
            var _entity = await _context.Companies
                .SingleOrDefaultAsync(r => r.CompanyId == request.CompanyId, cancellationToken);
            if (_entity == null)
            {
                throw new CompanyUpdateCommandKeyNotFoundException(request.CompanyId);
            }
            // Move from update command class to entity class.
            _entity.CompanyShortName = request.CompanyShortName;
            _entity.CompanyName = request.CompanyName;
            _entity.Address = request.Address;
            _entity.City = request.City;
            _entity.State = request.State;
            _entity.PostalCode = request.PostalCode;
            _entity.Country = request.Country;
            _entity.PhoneNumber = request.PhoneNumber;
            _entity.Notes = request.Notes;
            //
            await _context.SaveChangesAsync(cancellationToken);
            // Return the row count.
            return 1;
        }
        //
        /// <summary>
        /// FluentValidation of the 'CompanyUpdateCommand' class.
        /// </summary>
        public class Validator : AbstractValidator<CompanyUpdateCommand>
        {
            //
            /// <summary>
            /// Constructor that will invoke the 'CompanyUpdateCommand' validator.
            /// </summary>
            public Validator()
            {
                //
                RuleFor(x => x.CompanyId).NotNull();
                RuleFor(x => x.CompanyShortName).NotEmpty().MaximumLength(12);
                RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(80);
                RuleFor(x => x.Address).MaximumLength(80);
                RuleFor(x => x.City).MaximumLength(50);
                RuleFor(x => x.State).MaximumLength(4);
                RuleFor(x => x.PostalCode).MaximumLength(15);
                RuleFor(x => x.Country).MaximumLength(50);
                RuleFor(x => x.PhoneNumber).MaximumLength(50);
                RuleFor(x => x.Notes).MaximumLength(1073741823);
                //
            }
            //
        }
        //
    }
    //
    /// <summary>
    /// Custom CompanyUpdateCommand record not found exception.
    /// </summary>
    public class CompanyUpdateCommandKeyNotFoundException: KeyNotFoundException
    {
        //
        /// <summary>
        /// Implementation of CompanyUpdateCommand record not found exception.
        /// </summary>
        /// <param name="id">The key for the record.</param>
        public CompanyUpdateCommandKeyNotFoundException(int companyId)
            : base($"CompanyUpdateCommand key not found exception: id: {companyId}")
        {
        }
    }
    //
    /// <summary>
    /// Custom CompanyUpdateCommand validation exception.
    /// </summary>
    public class CompanyUpdateCommandValidationException: Exception
    {
        //
        /// <summary>
        /// Implementation of CompanyUpdateCommand validation exception.
        /// </summary>
        /// <param name="errorMessage">The validation error messages.</param>
        public CompanyUpdateCommandValidationException(string errorMessage)
            : base($"CompanyUpdateCommand validation exception: errors: {errorMessage}")
        {
        }
    }
    //
    /// <summary>
    /// Custom CompanyUpdateCommand permission exception.
    /// </summary>
    public class CompanyUpdateCommandPermissionException : Exception
    {
        //
        /// <summary>
        /// Implementation of CompanyUpdateCommand permission exception.
        /// </summary>
        /// <param name="errorMessage">The validation error messages.</param>
        public CompanyUpdateCommandPermissionException(string errorMessage)
            : base($"CompanyUpdateCommand validation exception: errors: {errorMessage}")
        {
        }
    }
    //
}
// ---------------------------------------------------------------------------
