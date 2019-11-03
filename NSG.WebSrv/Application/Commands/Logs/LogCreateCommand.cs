//
// ---------------------------------------------------------------------------
// Log create command.
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
using System.Reflection;
using NSG.WebSrv.Infrastructure.Common;
//
namespace NSG.WebSrv.Application.Commands.Logs
{
	//
	/// <summary>
	/// 'Log' create command, handler and handle.
	/// </summary>
	public class LogCreateCommand : IRequest<LogData>
	{
        public LoggingLevel Level { get; set; }
        public MethodBase Method { get; set; }
		public string Message { get; set; }
		public Exception Exception { get; set; }
    }
    //
    /// <summary>
    /// 'Log' create command handler.
    /// </summary>
    public class LogCreateCommandHandler : IRequestHandler<LogCreateCommand, LogData>
	{
		private readonly IDb_Context _context;
        private IApplication _application;
        //
        /// <summary>
        ///  The constructor for the inner handler class, to create the Log entity.
        /// </summary>
        /// <param name="context">The database interface context.</param>
        public LogCreateCommandHandler(IDb_Context context, IApplication application)
        {
			_context = context;
            _application = application;
        }
		//
		/// <summary>
		/// 'Log' command handle method, passing two interfaces.
		/// </summary>
		/// <param name="request">This create command request.</param>
		/// <param name="cancellationToken">Cancel token.</param>
		/// <returns>The Log entity class.</returns>
		public async Task<LogData> Handle(LogCreateCommand request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new CreateCommandValidationException(_results.FluentValidationErrors());
			}
            string _method = request.Method.DeclaringType.FullName + "." + request.Method.Name;
            string _exception = (request.Exception == null ? "" : request.Exception.ToString());
            // Move from create command class to entity class.
            var _entity = new LogData
			{
				Date = _application.Now(),
				Application = _application.GetApplicationName(),
                Method = (_method.Length > 255 ? _method.Substring(0, 255) : _method),
                LogLevel = (byte)request.Level,
                Level = Enum.GetName(request.Level.GetType(), request.Level),
                UserAccount = _application.GetUserAccount(),
                Message = (request.Message.Length > 4000 ? request.Message.Substring(0, 4000) : request.Message),
				Exception = _exception.Length > 4000 ? _exception.Substring(0, 4000) : _exception
            };
            _context.Logs.Add(_entity);
			await _context.SaveChangesAsync(cancellationToken);
			// Return the entity class.
			return _entity;
		}
		//
		/// <summary>
		/// FluentValidation of the 'LogCreateCommand' class.
		/// </summary>
		public class Validator : AbstractValidator<LogCreateCommand>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'LogCreateCommand' validator.
			/// </summary>
			public Validator()
			{
				//
				//RuleFor(x => x.UserAccount).NotEmpty().MaximumLength(255);
				RuleFor(x => x.Message).NotEmpty();
				//RuleFor(x => x.Exception).MaximumLength(4000);
				//
			}
			//
		}
		//
	}
	//
	/// <summary>
	/// Custom LogCreateCommand validation exception.
	/// </summary>
	public class CreateCommandValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of LogCreateCommand validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public CreateCommandValidationException(string errorMessage)
			: base($"LogCreateCommand validation exception: errors: {errorMessage}")
		{
		}
	}
	//
}
// ---------------------------------------------------------------------------

