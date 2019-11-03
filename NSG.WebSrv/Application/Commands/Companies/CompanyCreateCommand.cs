//
// ---------------------------------------------------------------------------
// Companies create command.
//
using System;
using System.Threading;
using System.Threading.Tasks;
//
using MediatR;
using FluentValidation;
using FluentValidation.Results;
using NSG.WebSrv.Domain.Entities;
//
namespace NSG.WebSrv.Application.Commands.Companies
{
    //
    /// <summary>
    /// 'Company' create command, handler and handle.
    /// </summary>
    public class CompanyCreateCommand : IRequest<Company>
    {
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
    /// 'Company' create command handler.
    /// </summary>
    public class CompanyCreateCommandHandler : IRequestHandler<CompanyCreateCommand, Company>
    {
        private readonly IDb_Context _context;
        //
        //
        /// <summary>
        ///  The constructor for the inner handler class, to create the Company entity.
        /// </summary>
        /// <param name="context">The database interface context.</param>
        public CompanyCreateCommandHandler(IDb_Context context)
        {
            _context = context;
        }
        //
        /// <summary>
        /// 'Company' command handle method, passing two interfaces.
        /// </summary>
        /// <param name="request">This create command request.</param>
        /// <param name="cancellationToken">Cancel token.</param>
        /// <returns>The Company entity class.</returns>
        public async Task<Company> Handle(CompanyCreateCommand request, CancellationToken cancellationToken)
        {
            Validator _validator = new Validator();
            ValidationResult _results = _validator.Validate(request);
            if (!_results.IsValid)
            {
                // Call the FluentValidationErrors extension method.
                throw new CreateCommandValidationException(_results.FluentValidationErrors());
            }
            // Move from create command class to entity class.
            var _entity = new Company
            {
                CompanyShortName = request.CompanyShortName,
                CompanyName = request.CompanyName,
                Address = request.Address,
                City = request.City,
                State = request.State,
                PostalCode = request.PostalCode,
                Country = request.Country,
                PhoneNumber = request.PhoneNumber,
                Notes = request.Notes,
            };
            _context.Companies.Add(_entity);
            await _context.SaveChangesAsync(cancellationToken);
            // Return the entity class.
            return _entity;
        }
        //
        /// <summary>
        /// FluentValidation of the 'CompanyCreateCommand' class.
        /// </summary>
        public class Validator : AbstractValidator<CompanyCreateCommand>
        {
            //
            /// <summary>
            /// Constructor that will invoke the 'CompanyCreateCommand' validator.
            /// </summary>
            public Validator()
            {
                //
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
    /// Custom CompanyCreateCommand validation exception.
    /// </summary>
    public class CreateCommandValidationException: Exception
    {
        //
        /// <summary>
        /// Implementation of CompanyCreateCommand validation exception.
        /// </summary>
        /// <param name="errorMessage">The validation error messages.</param>
        public CreateCommandValidationException(string errorMessage)
            : base($"CompanyCreateCommand validation exception: errors: {errorMessage}")
        {
        }
    }
    //
}
// ---------------------------------------------------------------------------
