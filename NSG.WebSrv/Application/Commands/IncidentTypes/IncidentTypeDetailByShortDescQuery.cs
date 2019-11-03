//
// ---------------------------------------------------------------------------
// IncidentType detail query by Short-Desc.
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
namespace NSG.WebSrv.Application.Commands.IncidentTypes
{
    //
    /// <summary>
    /// 'IncidentType' detail query by Short-Desc, handler and handle.
    /// </summary>
    public class IncidentTypeDetailByShortDescQuery
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int IncidentTypeId { get; set; }
        public string IncidentTypeShortDesc { get; set; }
        public string IncidentTypeDesc { get; set; }
        public bool IncidentTypeFromServer { get; set; }
        public string IncidentTypeSubjectLine { get; set; }
        public string IncidentTypeEmailTemplate { get; set; }
        public string IncidentTypeTimeTemplate { get; set; }
        public string IncidentTypeThanksTemplate { get; set; }
        public string IncidentTypeLogTemplate { get; set; }
        public string IncidentTypeTemplate { get; set; }
    }
    //
    /// <summary>
    /// 'IncidentType' detail query handler.
    /// </summary>
    public class IncidentTypeDetailByShortDescQueryHandler : IRequestHandler<IncidentTypeDetailByShortDescQueryHandler.DetailQuery, IncidentTypeDetailByShortDescQuery>
    {
        private readonly IDb_Context _context;
        //
        /// <summary>
        ///  The constructor for the inner handler class, to detail the IncidentType entity.
        /// </summary>
        /// <param name="context">The database interface context.</param>
        public IncidentTypeDetailByShortDescQueryHandler(IDb_Context context)
        {
            _context = context;
        }
        //
        /// <summary>
        /// 'IncidentType' query handle method, passing two interfaces.
        /// </summary>
        /// <param name="request">This detail query request.</param>
        /// <param name="cancellationToken">Cancel token.</param>
        /// <returns>Returns the row count.</returns>
        public async Task<IncidentTypeDetailByShortDescQuery> Handle(DetailQuery request, CancellationToken cancellationToken)
        {
            Validator _validator = new Validator();
            ValidationResult _results = _validator.Validate(request);
            if (!_results.IsValid)
            {
                // Call the FluentValidationErrors extension method.
                throw new IncidentTypeDetailByShortDescQueryValidationException(_results.FluentValidationErrors());
            }
            var _entity = await GetEntityByShortDesc(request.IncidentTypeShortDesc);
            if (_entity == null)
            {
                throw new IncidentTypeDetailByShortDescQueryKeyNotFoundException(request.IncidentTypeShortDesc);
            }
            //
            // Return the detail query model.
            return _entity.ToIncidentTypeDetailByShortDescQuery();
        }
        //
        /// <summary>
        /// Get an entity record via the primary key.
        /// </summary>
        /// <param name="incidentTypeId">int key</param>
        /// <returns>Returns a IncidentType entity record.</returns>
        private Task<IncidentType> GetEntityByShortDesc(string incidentTypeShortDesc)
        {
            return _context.IncidentTypes.SingleOrDefaultAsync(r => r.IncidentTypeShortDesc == incidentTypeShortDesc);
        }
        //
        /// <summary>
        /// Get IncidentType detail query class (the primary key).
        /// </summary>
        public class DetailQuery : IRequest<IncidentTypeDetailByShortDescQuery>
        {
            public string IncidentTypeShortDesc { get; set; }
        }
        //
        /// <summary>
        /// FluentValidation of the 'IncidentTypeDetailQuery' class.
        /// </summary>
        public class Validator : AbstractValidator<DetailQuery>
        {
            //
            /// <summary>
            /// Constructor that will invoke the 'IncidentTypeDetailQuery' validator.
            /// </summary>
            public Validator()
            {
                //
                RuleFor(x => x.IncidentTypeShortDesc).NotNull();
                //
            }
            //
        }
        //
    }
    //
    /// <summary>
    /// Custom IncidentTypeDetailQuery record not found exception.
    /// </summary>
    public class IncidentTypeDetailByShortDescQueryKeyNotFoundException : KeyNotFoundException
    {
        //
        /// <summary>
        /// Implementation of IncidentTypeDetailQuery record not found exception.
        /// </summary>
        /// <param name="id">The key for the record.</param>
        public IncidentTypeDetailByShortDescQueryKeyNotFoundException(string shortDesc)
            : base($"IncidentTypeDetailByShortDescQuery key not found exception: Id: {shortDesc}")
        {
        }
    }
    //
    /// <summary>
    /// Custom IncidentTypeDetailQuery validation exception.
    /// </summary>
    public class IncidentTypeDetailByShortDescQueryValidationException : Exception
    {
        //
        /// <summary>
        /// Implementation of IncidentTypeDetailQuery validation exception.
        /// </summary>
        /// <param name="errorMessage">The validation error messages.</param>
        public IncidentTypeDetailByShortDescQueryValidationException(string errorMessage)
            : base($"IncidentTypeDetailByShortDescQuery validation exception: errors: {errorMessage}")
        {
        }
    }
    //
}
// ---------------------------------------------------------------------------

