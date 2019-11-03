//
// ---------------------------------------------------------------------------
// Company & Server detail query.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
//
using Microsoft.EntityFrameworkCore;
using MediatR;
using FluentValidation;
using FluentValidation.Results;
//
using NSG.WebSrv.Domain.Entities;
using NSG.WebSrv.Application.Infrastructure;
//
namespace NSG.WebSrv.Application.Commands.CompanyServers
{
    //
    /// <summary>
    /// 'Company' detail query, handler and handle.
    /// </summary>
    public class CompanyServerDetailQuery
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
        //
        public List<ServerDetailQuery> ServerList { get; set; }
        //
    }
    //
    /// <summary>
    /// 'Server' detail query, handler and handle.
    /// </summary>
    public class ServerDetailQuery
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
    /// 'Company' detail query handler.
    /// </summary>
    public class CompanyServerDetailQueryHandler : IRequestHandler<CompanyServerDetailQueryHandler.DetailQuery, CompanyServerDetailQuery>
    {
        private readonly IDb_Context _context;
        private IMediator Mediator;
        //
        /// <summary>
        ///  The constructor for the inner handler class, to detail the Company entity.
        /// </summary>
        /// <param name="context">The database interface context.</param>
        public CompanyServerDetailQueryHandler(IDb_Context context, IMediator mediator)
        {
            _context = context;
            Mediator = mediator;
        }
        //
        /// <summary>
        /// 'Company' query handle method, passing two interfaces.
        /// </summary>
        /// <param name="request">This detail query request.</param>
        /// <param name="cancellationToken">Cancel token.</param>
        /// <returns>Returns the row count.</returns>
        public async Task<CompanyServerDetailQuery> Handle(DetailQuery request, CancellationToken cancellationToken)
        {
            Validator _validator = new Validator();
            ValidationResult _results = _validator.Validate(request);
            if (!_results.IsValid)
            {
                // Call the FluentValidationErrors extension method.
                throw new CompanyServerDetailQueryValidationException(_results.FluentValidationErrors());
            }
            //
            GetUserCompanyListQueryHandler.ViewModel _companiesViewModel =
                await Mediator.Send(new GetUserCompanyListQueryHandler.ListQuery());
            if (!_companiesViewModel.CompanyList.Contains(request.CompanyId))
            {
                throw new CompanyServerDetailQueryPermissionException($"User does not have permission for company: {request.CompanyId}");
            }
            //
            var _entity = await GetEntityByKey(request.CompanyId);
            if (_entity == null)
            {
                throw new CompanyServerDetailQueryKeyNotFoundException(request.CompanyId);
            }
            //
            // Return the detail query model.
            return _entity.ToCompanyDetailQuery();
        }
        //
        /// <summary>
        /// Get an entity record via the primary key.
        /// </summary>
        /// <param name="companyId">int key</param>
        /// <returns>Returns a Company entity record.</returns>
        private Task<Company> GetEntityByKey(int companyId)
        {
            return _context.Companies
                .Include(cpy => cpy.Servers)
                .SingleOrDefaultAsync(r => r.CompanyId == companyId);
        }
        //
        /// <summary>
        /// Get Company detail query class (the primary key).
        /// </summary>
        public class DetailQuery : IRequest<CompanyServerDetailQuery>
        {
            public int CompanyId { get; set; }
        }
        //
        /// <summary>
        /// FluentValidation of the 'CompanyDetailQuery' class.
        /// </summary>
        public class Validator : AbstractValidator<DetailQuery>
        {
            //
            /// <summary>
            /// Constructor that will invoke the 'CompanyDetailQuery' validator.
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
    /// Custom CompanyDetailQuery record not found exception.
    /// </summary>
    public class CompanyServerDetailQueryKeyNotFoundException: KeyNotFoundException
    {
        //
        /// <summary>
        /// Implementation of CompanyDetailQuery record not found exception.
        /// </summary>
        /// <param name="id">The key for the record.</param>
        public CompanyServerDetailQueryKeyNotFoundException(int companyId)
            : base($"CompanyDetailQuery key not found exception: Id: {companyId}")
        {
        }
    }
    //
    /// <summary>
    /// Custom CompanyDetailQuery validation exception.
    /// </summary>
    public class CompanyServerDetailQueryValidationException: Exception
    {
        //
        /// <summary>
        /// Implementation of CompanyDetailQuery validation exception.
        /// </summary>
        /// <param name="errorMessage">The validation error messages.</param>
        public CompanyServerDetailQueryValidationException(string errorMessage)
            : base($"CompanyDetailQuery validation exception: errors: {errorMessage}")
        {
        }
    }
    //
    /// <summary>
    /// Custom CompanyDetailQuery permission exception.
    /// </summary>
    public class CompanyServerDetailQueryPermissionException : Exception
    {
        //
        /// <summary>
        /// Implementation of CompanyDetailQuery permission exception.
        /// </summary>
        /// <param name="errorMessage">The validation error messages.</param>
        public CompanyServerDetailQueryPermissionException(string errorMessage)
            : base($"CompanyDetailQuery validation exception: errors: {errorMessage}")
        {
        }
    }
    //
}
// ---------------------------------------------------------------------------