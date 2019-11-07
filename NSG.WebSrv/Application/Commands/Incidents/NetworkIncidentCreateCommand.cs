//
// ---------------------------------------------------------------------------
// Incident create command.
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
using NSG.WebSrv.Infrastructure.Common;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSG.WebSrv.Application.Commands.Logs;
//
namespace NSG.WebSrv.Application.Commands.Incidents
{
	//
	/// <summary>
	/// 'Incident' create command, handler and handle.
	/// </summary>
	public class NetworkIncidentCreateCommand : IRequest<NetworkIncidentDetailQuery>
	{
		public int ServerId { get; set; }
		public string IPAddress { get; set; }
		public string NIC_Id { get; set; }
		public string NetworkName { get; set; }
		public string AbuseEmailAddress { get; set; }
		public string ISPTicketNumber { get; set; }
		public bool Mailed { get; set; }
		public bool Closed { get; set; }
		public bool Special { get; set; }
		public string Notes { get; set; }
		public DateTime CreatedDate { get; set; }
        //
        public string message;
        //
        public List<IncidentNoteData> IncidentNotes;
        public List<IncidentNoteData> DeletedNotes;
        //
        public List<NetworkLogData> NetworkLogs;
        public List<NetworkLogData> DeletedLogs;
        //
        public UserServerData User;
        //
    }
    //
    /// <summary>
    /// 'Incident' create command handler.
    /// </summary>
    public class NetworkIncidentCreateCommandHandler : IRequestHandler<NetworkIncidentCreateCommand, NetworkIncidentDetailQuery>
	{
		private readonly IDb_Context _context;
        protected IMediator Mediator;
        private IApplication _application;
        //
        //
        /// <summary>
        ///  The constructor for the inner handler class, to create the Incident entity.
        /// </summary>
        /// <param name="context">The database interface context.</param>
        public NetworkIncidentCreateCommandHandler(IDb_Context context, IMediator mediator, IApplication application)
        {
            _context = context;
            Mediator = mediator;
            _application = application;
        }
        //
        /// <summary>
        /// 'Incident' command handle method, passing two interfaces.
        /// </summary>
        /// <param name="request">This create command request.</param>
        /// <param name="cancellationToken">Cancel token.</param>
        /// <returns>The Incident entity class.</returns>
        public async Task<NetworkIncidentDetailQuery> Handle(NetworkIncidentCreateCommand request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new NetworkIncidentCreateCommandValidationException(_results.FluentValidationErrors());
			}
            string _userNameIP = $"Entering with, User: {request.User.UserName}, IP: {request.IPAddress}";
            System.Diagnostics.Debug.WriteLine(_userNameIP);
            // Move from create command class to entity class.
            Incident _entity = CreateIncidentFromRequest(request);
            _context.Incidents.Add(_entity);
            //
            try
            {
                // Add the IncidentNotes and link IncidentIncidentNotes
                AddIncidentNotes(request, _entity);
                // List<NetworkLogData> networkLogs;
                // var _networkLogs = new List<NetworkLog>();
                foreach (NetworkLogData _nld in request.NetworkLogs.Where(_l => _l.Selected == true))
                {
                    NetworkLog _networkLog = await _context.NetworkLogs.FirstOrDefaultAsync(_nl => _nl.NetworkLogId == _nld.NetworkLogId);
                    _networkLog.Incident = _entity;
                }
                // List<NetworkLogData> deletedLogs;
                foreach (NetworkLogData _nld in request.DeletedLogs)
                {
                    NetworkLog _networkLog = await _context.NetworkLogs.FirstOrDefaultAsync(_nl => _nl.NetworkLogId == _nld.NetworkLogId);
                    _context.NetworkLogs.Remove(_networkLog);
                }
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception _ex)
            {
                await Mediator.Send(new LogCreateCommand(
                    LoggingLevel.Warning, MethodBase.GetCurrentMethod(),
                    _ex.GetBaseException().Message, _ex));
                System.Diagnostics.Debug.WriteLine(_ex.ToString());
                throw (_ex);
            }
            return await Mediator.Send(new NetworkIncidentDetailQueryHandler.DetailQuery() { IncidentId = _entity.IncidentId });
		}
        Incident CreateIncidentFromRequest(NetworkIncidentCreateCommand request)
        {
            return new Incident
            {
                ServerId = request.ServerId,
                IPAddress = request.IPAddress,
                NIC_Id = request.NIC_Id,
                NetworkName = request.NetworkName,
                AbuseEmailAddress = request.AbuseEmailAddress,
                ISPTicketNumber = request.ISPTicketNumber,
                // cannot mail or close incident on creation
                Mailed = false, // request.Mailed (need IncidentId),
                Closed = false, // request.Closed,
                Special = request.Special,
                Notes = request.Notes,
                CreatedDate = request.CreatedDate,
            };
        }
        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        void AddIncidentNotes(NetworkIncidentCreateCommand request, Incident entity)
        {
            //var _incidentIncidentNotes = new List<IncidentIncidentNote>();
            //var _incidentNotes = new List<IncidentNote>();
            foreach (IncidentNoteData _ind in request.IncidentNotes)
            {
                var _incidentNote = new IncidentNote()
                {
                    NoteTypeId = _ind.NoteTypeId,
                    Note = _ind.Note,
                    CreatedDate = _ind.CreatedDate
                };
                _context.IncidentNotes.Add(_incidentNote);
                var _incidentIncidentNote = new IncidentIncidentNote()
                {
                    Incident = entity,
                    IncidentNote = _incidentNote
                };
                _context.IncidentIncidentNotes.Add(_incidentIncidentNote);
            }
        }
        //
        /// <summary>
        /// FluentValidation of the 'NetworkIncidentCreateCommand' class.
        /// </summary>
        public class Validator : AbstractValidator<NetworkIncidentCreateCommand>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'NetworkIncidentCreateCommand' validator.
			/// </summary>
			public Validator()
			{
				//
				RuleFor(x => x.ServerId).NotNull();
				RuleFor(x => x.IPAddress).NotEmpty().MinimumLength(7).MaximumLength(50)
                    .Must(Extensions.ValidateIPv4);
				RuleFor(x => x.NIC_Id).NotEmpty().MaximumLength(16);
				RuleFor(x => x.NetworkName).MaximumLength(255);
				RuleFor(x => x.AbuseEmailAddress).MaximumLength(255);
				RuleFor(x => x.ISPTicketNumber).MaximumLength(50);
				RuleFor(x => x.Mailed).NotNull();
				RuleFor(x => x.Closed).NotNull();
				RuleFor(x => x.Special).NotNull();
				RuleFor(x => x.Notes).MaximumLength(1073741823);
				RuleFor(x => x.CreatedDate).NotNull();
                //
                RuleFor(n => n.IncidentNotes).NotNull();
                RuleFor(n => n.NetworkLogs).NotNull();
                RuleFor(n => n.DeletedLogs).NotNull();
                RuleFor(u => u.User).NotNull();
                //
            }
            //
        }
		//
	}
	//
	/// <summary>
	/// Custom NetworkIncidentCreateCommand validation exception.
	/// </summary>
	public class NetworkIncidentCreateCommandValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of NetworkIncidentCreateCommand validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public NetworkIncidentCreateCommandValidationException(string errorMessage)
			: base($"NetworkIncidentCreateCommand validation exception: errors: {errorMessage}")
		{
		}
	}
	//
}
// ---------------------------------------------------------------------------
