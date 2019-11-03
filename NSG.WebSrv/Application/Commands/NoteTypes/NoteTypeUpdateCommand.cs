//
// ---------------------------------------------------------------------------
// NoteType update command.
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
namespace NSG.WebSrv.Application.Commands.NoteTypes
{
	//
	/// <summary>
	/// 'NoteType' update command, handler and handle.
	/// </summary>
	public class NoteTypeUpdateCommand : IRequest<int>
	{
        [System.ComponentModel.DataAnnotations.Key]
		public int NoteTypeId { get; set; }
		public string NoteTypeShortDesc { get; set; }
		public string NoteTypeDesc { get; set; }
        public string NoteTypeClientScript { get; set; }
    }
    //
    /// <summary>
    /// 'NoteType' update command handler.
    /// </summary>
    public class NoteTypeUpdateCommandHandler : IRequestHandler<NoteTypeUpdateCommand, int>
	{
		private readonly IDb_Context _context;
		//
		/// <summary>
		///  The constructor for the inner handler class, to update the NoteType entity.
		/// </summary>
		/// <param name="context">The database interface context.</param>
		public NoteTypeUpdateCommandHandler(IDb_Context context)
		{
			_context = context;
		}
		//
		/// <summary>
		/// 'NoteType' command handle method, passing two interfaces.
		/// </summary>
		/// <param name="request">This update command request.</param>
		/// <param name="cancellationToken">Cancel token.</param>
		/// <returns>Returns the row count.</returns>
		public async Task<int> Handle(NoteTypeUpdateCommand request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new UpdateCommandValidationException(_results.FluentValidationErrors());
			}
			var _entity = await _context.NoteTypes
				.SingleOrDefaultAsync(r => r.NoteTypeId == request.NoteTypeId, cancellationToken);
			if (_entity == null)
			{
				throw new UpdateCommandKeyNotFoundException(request.NoteTypeId);
			}
			// Move from update command class to entity class.
			_entity.NoteTypeShortDesc = request.NoteTypeShortDesc;
			_entity.NoteTypeDesc = request.NoteTypeDesc;
            _entity.NoteTypeClientScript = request.NoteTypeClientScript.ToLower();
            //
            await _context.SaveChangesAsync(cancellationToken);
			// Return the row count.
			return 1;
		}
		//
		/// <summary>
		/// FluentValidation of the 'NoteTypeUpdateCommand' class.
		/// </summary>
		public class Validator : AbstractValidator<NoteTypeUpdateCommand>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'NoteTypeUpdateCommand' validator.
			/// </summary>
			public Validator()
			{
				//
				RuleFor(x => x.NoteTypeId).NotNull();
				RuleFor(x => x.NoteTypeShortDesc).NotEmpty().MaximumLength(8);
				RuleFor(x => x.NoteTypeDesc).NotEmpty().MaximumLength(50);
                RuleFor(x => x.NoteTypeClientScript).MaximumLength(12);
                //
            }
            //
        }
		//
	}
	//
	/// <summary>
	/// Custom NoteTypeUpdateCommand record not found exception.
	/// </summary>
	public class UpdateCommandKeyNotFoundException: KeyNotFoundException
	{
		//
		/// <summary>
		/// Implementation of NoteTypeUpdateCommand record not found exception.
		/// </summary>
		/// <param name="id">The key for the record.</param>
		public UpdateCommandKeyNotFoundException(int noteTypeId)
			: base($"NoteTypeUpdateCommand key not found exception: id: {noteTypeId}")
		{
		}
	}
	//
	/// <summary>
	/// Custom NoteTypeUpdateCommand validation exception.
	/// </summary>
	public class UpdateCommandValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of NoteTypeUpdateCommand validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public UpdateCommandValidationException(string errorMessage)
			: base($"NoteTypeUpdateCommand validation exception: errors: {errorMessage}")
		{
		}
	}
}
// ---------------------------------------------------------------------------
