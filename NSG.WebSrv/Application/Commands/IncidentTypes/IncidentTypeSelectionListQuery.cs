//
// ---------------------------------------------------------------------------
// IncidentType select list query.
//
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
//
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using MediatR;
using FluentValidation;
using FluentValidation.Results;
using NSG.WebSrv.Domain.Entities;
//
namespace NSG.WebSrv.Application.Commands.IncidentTypes
{
    //
    /// <summary>
    /// 'IncidentType' list query handler.
    /// </summary>
    public class IncidentTypeSelectionListQueryHandler : IRequestHandler<IncidentTypeSelectionListQueryHandler.ListQuery, IncidentTypeSelectionListQueryHandler.ViewModel>
    {
        private readonly IDb_Context _context;
        //
        /// <summary>
        ///  The constructor for the inner handler class, to list the IncidentType entity.
        /// </summary>
        /// <param name="context">The database interface context.</param>
        public IncidentTypeSelectionListQueryHandler(IDb_Context context)
        {
            _context = context;
        }
        //
        /// <summary>
        /// 'IncidentType' query handle method, passing two interfaces.
        /// </summary>
        /// <param name="queryRequest">This list query request.</param>
        /// <param name="cancellationToken">Cancel token.</param>
        /// <returns>Returns a list of IncidentTypeSelectionListQuery.</returns>
        public async Task<ViewModel> Handle(ListQuery queryRequest, CancellationToken cancellationToken)
        {
            Validator _validator = new Validator();
            ValidationResult _results = _validator.Validate(queryRequest);
            if (!_results.IsValid)
            {
                // Call the FluentValidationErrors extension method.
                throw new IncidentTypeSelectionListQueryValidationException(_results.FluentValidationErrors());
            }
            //
            return new ViewModel()
            {
                IncidentTypesList = await _context.IncidentTypes.ToAsyncEnumerable()
                    .Select(cnt => cnt.ToIncidentTypeSelectList(queryRequest.SelectIncidentTypeId)).ToList()
            };
        }
        //
        /// <summary>
        /// The IncidentType list query class view class.
        /// </summary>
        public class ViewModel
        {
            public List<SelectListItem> IncidentTypesList { get; set; }
        }
        //
        /// <summary>
        /// Get IncidentType list query class (the primary key).
        /// </summary>
        public class ListQuery : IRequest<ViewModel>
        {
            public int SelectIncidentTypeId { get; set; }
        }
        //
        /// <summary>
        /// FluentValidation of the 'IncidentTypeSelectionListQuery' class.
        /// </summary>
        public class Validator : AbstractValidator<ListQuery>
        {
            //
            /// <summary>
            /// Constructor that will invoke the 'IncidentTypeSelectionListQuery' validator.
            /// </summary>
            public Validator()
            {
                //
                RuleFor(x => x.SelectIncidentTypeId).NotNull();
                //
            }
            //
        }
        //
    }
    //
    /// <summary>
    /// Custom IncidentTypeSelectionListQuery validation exception.
    /// </summary>
    public class IncidentTypeSelectionListQueryValidationException : Exception
    {
        //
        /// <summary>
        /// Implementation of IncidentTypeSelectionListQuery validation exception.
        /// </summary>
        /// <param name="errorMessage">The validation error messages.</param>
        public IncidentTypeSelectionListQueryValidationException(string errorMessage)
            : base($"IncidentTypeSelectionListQuery validation exception: errors: {errorMessage}")
        {
        }
    }
    //
}
// ---------------------------------------------------------------------------
