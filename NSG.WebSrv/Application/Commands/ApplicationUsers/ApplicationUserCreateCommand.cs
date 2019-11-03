//
// ---------------------------------------------------------------------------
// ApplicationUsers create command.
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
//
namespace NSG.WebSrv.Application.Commands.ApplicationUsers
{
	//
	/// <summary>
	/// 'ApplicationUser' create command, handler and handle.
	/// </summary>
	public class ApplicationUserCreateCommand : IRequest<ApplicationUser>
	{
		public string Id { get; set; }
		public string UserName { get; set; }
		public string NormalizedUserName { get; set; }
		public string Email { get; set; }
		public string NormalizedEmail { get; set; }
		public bool EmailConfirmed { get; set; }
		public string PasswordHash { get; set; }
		public string SecurityStamp { get; set; }
		public string ConcurrencyStamp { get; set; }
		public string PhoneNumber { get; set; }
		public bool PhoneNumberConfirmed { get; set; }
		public bool TwoFactorEnabled { get; set; }
		public DateTimeOffset? LockoutEnd { get; set; }
		public bool LockoutEnabled { get; set; }
		public int AccessFailedCount { get; set; }
		public int CompanyId { get; set; }
		public DateTime CreateDate { get; set; }
		public string FirstName { get; set; }
		public string FullName { get; set; }
		public string LastName { get; set; }
		public string UserNicName { get; set; }
	}
	//
	/// <summary>
	/// 'ApplicationUser' create command handler.
	/// </summary>
	public class ApplicationUserCreateCommandHandler : IRequestHandler<ApplicationUserCreateCommand, ApplicationUser>
	{
		private readonly IDb_Context _context;
		//
		//
		/// <summary>
		///  The constructor for the inner handler class, to create the ApplicationUser entity.
		/// </summary>
		/// <param name="context">The database interface context.</param>
		public ApplicationUserCreateCommandHandler(IDb_Context context)
		{
			_context = context;
		}
		//
		/// <summary>
		/// 'ApplicationUser' command handle method, passing two interfaces.
		/// </summary>
		/// <param name="request">This create command request.</param>
		/// <param name="cancellationToken">Cancel token.</param>
		/// <returns>The ApplicationUser entity class.</returns>
		public async Task<ApplicationUser> Handle(ApplicationUserCreateCommand request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new CreateCommandValidationException(_results.FluentValidationErrors());
			}
			var _duplicate = await _context.Users
				.SingleOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
			if (_duplicate != null)
			{
				throw new CreateCommandDuplicateException(request.Id);
			}
			// Move from create command class to entity class.
			var _entity = new ApplicationUser
			{
				Id = request.Id,
				UserName = request.UserName,
				NormalizedUserName = request.NormalizedUserName,
				Email = request.Email,
				NormalizedEmail = request.NormalizedEmail,
				EmailConfirmed = request.EmailConfirmed,
				PasswordHash = request.PasswordHash,
				SecurityStamp = request.SecurityStamp,
				ConcurrencyStamp = request.ConcurrencyStamp,
				PhoneNumber = request.PhoneNumber,
				PhoneNumberConfirmed = request.PhoneNumberConfirmed,
				TwoFactorEnabled = request.TwoFactorEnabled,
				LockoutEnd = request.LockoutEnd,
				LockoutEnabled = request.LockoutEnabled,
				AccessFailedCount = request.AccessFailedCount,
				CompanyId = request.CompanyId,
				CreateDate = request.CreateDate,
				FirstName = request.FirstName,
				FullName = request.FullName,
				LastName = request.LastName,
				UserNicName = request.UserNicName,
			};
			_context.Users.Add(_entity);
			await _context.SaveChangesAsync(cancellationToken);
			// Return the entity class.
			return _entity;
		}
		//
		/// <summary>
		/// FluentValidation of the 'ApplicationUserCreateCommand' class.
		/// </summary>
		public class Validator : AbstractValidator<ApplicationUserCreateCommand>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'ApplicationUserCreateCommand' validator.
			/// </summary>
			public Validator()
			{
				//
				RuleFor(x => x.Id).NotEmpty().MaximumLength(128);
				RuleFor(x => x.UserName).MaximumLength(256);
				RuleFor(x => x.NormalizedUserName).MaximumLength(256);
				RuleFor(x => x.Email).MaximumLength(256);
				RuleFor(x => x.NormalizedEmail).MaximumLength(256);
				RuleFor(x => x.EmailConfirmed).NotNull();
				RuleFor(x => x.PasswordHash).MaximumLength(1073741823);
				RuleFor(x => x.SecurityStamp).MaximumLength(1073741823);
				RuleFor(x => x.ConcurrencyStamp).MaximumLength(1073741823);
				RuleFor(x => x.PhoneNumber).MaximumLength(1073741823);
				RuleFor(x => x.PhoneNumberConfirmed).NotNull();
				RuleFor(x => x.TwoFactorEnabled).NotNull();
				RuleFor(x => x.LockoutEnabled).NotNull();
				RuleFor(x => x.AccessFailedCount).NotNull();
				RuleFor(x => x.CompanyId).NotNull();
				RuleFor(x => x.CreateDate).NotNull();
				RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
				RuleFor(x => x.FullName).NotEmpty().MaximumLength(100);
				RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
				RuleFor(x => x.UserNicName).NotEmpty().MaximumLength(16);
				//
			}
			//
		}
		//
	}
	//
	/// <summary>
	/// Custom ApplicationUserCreateCommand duplicate exception.
	/// </summary>
	public class CreateCommandDuplicateException: Exception
	{
		//
		/// <summary>
		/// Implementation of ApplicationUserCreateCommand duplicate exception.
		/// </summary>
		/// <param name="id">The key for the record.</param>
		public CreateCommandDuplicateException(string id)
			: base($"ApplicationUserCreateCommand duplicate id exception: Id: {id}")
		{
		}
	}
	//
	/// <summary>
	/// Custom ApplicationUserCreateCommand validation exception.
	/// </summary>
	public class CreateCommandValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of ApplicationUserCreateCommand validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public CreateCommandValidationException(string errorMessage)
			: base($"ApplicationUserCreateCommand validation exception: errors: {errorMessage}")
		{
		}
	}
	//
}
// ---------------------------------------------------------------------------

