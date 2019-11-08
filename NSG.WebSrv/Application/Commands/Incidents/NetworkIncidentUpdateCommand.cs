//
// ---------------------------------------------------------------------------
// Incident update command.
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
using NSG.WebSrv.Infrastructure.Common;
using NSG.WebSrv.Application.Commands.Logs;
using System.Reflection;
using System.Linq;
using SendGrid.Helpers.Mail;
using Newtonsoft.Json;
using NSG.WebSrv.Infrastructure.Notification;
//
namespace NSG.WebSrv.Application.Commands.Incidents
{
	//
	/// <summary>
	/// 'Incident' update command, handler and handle.
	/// </summary>
	public class NetworkIncidentUpdateCommand : IRequest<NetworkIncidentDetailQuery>
	{
		public long IncidentId { get; set; }
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
        //
        public string Message;
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
    /// 'Incident' update command handler.
    /// </summary>
    public class NetworkIncidentUpdateCommandHandler : IRequestHandler<NetworkIncidentUpdateCommand, NetworkIncidentDetailQuery>
	{
		private readonly IDb_Context _context;
        protected IMediator Mediator;
        private IApplication _application;
        private INotificationService _notification;
        //
        /// <summary>
        ///  The constructor for the inner handler class, to update the Incident entity.
        /// </summary>
        /// <param name="context">The database interface context.</param>
        public NetworkIncidentUpdateCommandHandler(IDb_Context context, IMediator mediator, IApplication application, INotificationService notification)
        {
            _context = context;
            Mediator = mediator;
            _application = application;
            _notification = notification;
        }
        //
        /// <summary>
        /// 'Incident' command handle method, passing two interfaces.
        /// </summary>
        /// <param name="request">This update command request.</param>
        /// <param name="cancellationToken">Cancel token.</param>
        /// <returns>Returns the row count.</returns>
        public async Task<NetworkIncidentDetailQuery> Handle(NetworkIncidentUpdateCommand request, CancellationToken cancellationToken)
		{
            if (_application.IsEditableRole() == false)
            {
                throw new NetworkIncidentUpdateCommandPermissionsException("user not in editable group.");
            }
            Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new NetworkIncidentUpdateCommandValidationException(_results.FluentValidationErrors());
			}
            string _params = $"Entering with, incidentId: {request.IncidentId}, UserName: {request.User.UserName}";
            System.Diagnostics.Debug.WriteLine(_params);
            //
            var _entity = await _context.Incidents
                .Include(_i => _i.IncidentIncidentNotes)
				.SingleOrDefaultAsync(_r => _r.IncidentId == request.IncidentId, cancellationToken);
			if (_entity == null)
			{
				throw new NetworkIncidentUpdateCommandKeyNotFoundException(request.IncidentId);
			}
            try
            {
                await Mediator.Send(new LogCreateCommand(
                    LoggingLevel.Debug, MethodBase.GetCurrentMethod(), _params));
                bool _mailedBefore = _entity.Mailed;
                // Move from update command class to entity class.
                MoveRequestToEntity(request, _entity);
                //
                // Add and update notes
                await IncidentNotesAddUpdateAsync(request, _entity);
                // Delete notes and IncidentIncidentNotes
                IncidentNotesDelete(request, _entity);
                // ** NetworkLogs **
                // Update logs
                NetworkLogsUpdate(request, _entity);
                // Delete Logs;
                await NetworkLogsDeleteAsync(request, _entity);
                //
                await _context.SaveChangesAsync(cancellationToken);
                // "Inside, after save ... id: " + request.incident.IncidentId.ToString());
                //
                if (_mailedBefore == false && request.Mailed == true)
                {
                    await Task.Run(async () =>
                    {
                         await EMailIspReportAsync(request);
                    });
                }
                await Mediator.Send(new LogCreateCommand(
                    LoggingLevel.Info, MethodBase.GetCurrentMethod(),
                    $"Inside, Saved id: {request.IncidentId}"));
            }
            catch (Exception _ex)
            {
                await Mediator.Send(new LogCreateCommand(
                    LoggingLevel.Warning, MethodBase.GetCurrentMethod(),
                    _ex.GetBaseException().Message, _ex));
                System.Diagnostics.Debug.WriteLine(_ex.ToString());
                throw (_ex);
            }
            //
            return await Mediator.Send(new NetworkIncidentDetailQueryHandler.DetailQuery() { IncidentId = _entity.IncidentId });
        }
        //
        #region "MoveRequestToEntity"
        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="entity"></param>
        void MoveRequestToEntity(NetworkIncidentUpdateCommand request, Incident entity)
        {
            if( entity.ServerId != request.ServerId)
            {
                entity.ServerId = request.ServerId;
            }
            if(entity.IPAddress != request.IPAddress)
            {
                entity.IPAddress = request.IPAddress;
            }
            if (entity.NIC_Id != request.NIC_Id)
            {
                entity.NIC_Id = request.NIC_Id;
            }
            if (entity.NetworkName != request.NetworkName)
            {
                entity.NetworkName = request.NetworkName;
            }
            if (entity.AbuseEmailAddress != request.AbuseEmailAddress)
            {
                entity.AbuseEmailAddress = request.AbuseEmailAddress;
            }
            if (entity.ISPTicketNumber != request.ISPTicketNumber)
            {
                entity.ISPTicketNumber = request.ISPTicketNumber;
            }
            if (entity.Mailed != request.Mailed)
            {
                entity.Mailed = request.Mailed;
            }
            if (entity.Closed != request.Closed)
            {
                entity.Closed = request.Closed;
            }
            if (entity.Special != request.Special)
            {
                entity.Special = request.Special;
            }
            if (entity.Notes != request.Notes)
            {
                entity.Notes = request.Notes;
            }
        }
        //
        #endregion // MoveRequestToEntity
        //
        // IncidentNotesAddUpdateAsync
        // IncidentNotesDelete
        //
        #region "IncidentNotes processing"
        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        async Task IncidentNotesAddUpdateAsync(NetworkIncidentUpdateCommand request, Incident entity)
        {
            //var _incidentIncidentNotes = new List<IncidentIncidentNote>();
            //var _incidentNotes = new List<IncidentNote>();
            foreach (IncidentNoteData _ind in request.IncidentNotes.Where(_r => _r.IsChanged == true))
            {
                if (_ind.IncidentNoteId > 0)
                {
                    // Update notes
                    IncidentNote _incidentNote = await _context.IncidentNotes
                        .FirstOrDefaultAsync(_in => _in.IncidentNoteId == _ind.IncidentNoteId);
                    if (_incidentNote.NoteTypeId != _ind.NoteTypeId)
                    {
                        _incidentNote.NoteTypeId = _ind.NoteTypeId;
                    }
                    if (_incidentNote.Note != _ind.Note)
                    {
                        _incidentNote.Note = _ind.Note;
                    }
                }
                if (_ind.IncidentNoteId == 0)
                {
                    // Add notes and IncidentIncidentNotes
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
        }
        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="entity"></param>
        void IncidentNotesDelete(NetworkIncidentUpdateCommand request, Incident entity)
        {
            foreach (IncidentNoteData _ind in request.DeletedNotes)
            {
                if (_ind.IncidentNoteId > 0)
                {
                    IncidentIncidentNote _inn = entity.IncidentIncidentNotes.FirstOrDefault(_einn => _einn.IncidentNoteId == _ind.IncidentNoteId);
                    if( _inn != null)
                    {
                        _context.IncidentNotes.Remove(_inn.IncidentNote);
                        _context.IncidentIncidentNotes.Remove(_inn);
                    }
                }
            }
            //
        }
        //
        #endregion // IncidentNotes processing
        //
        // NetworkLogsUpdate
        // NetworkLogsDeleteAsync
        //
        #region "NetworkLogs processing"
        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="entity"></param>
        void NetworkLogsUpdate(NetworkIncidentUpdateCommand request, Incident entity)
        {
            foreach (NetworkLogData _nld in request.NetworkLogs.Where(_l => _l.IsChanged == true))
            {
                NetworkLog _networkLog = entity.NetworkLogs.FirstOrDefault(_r => _r.NetworkLogId == _nld.NetworkLogId);
                if (_networkLog != null)
                {
                    if(_networkLog.IncidentId != _nld.IncidentId)
                    {
                        _networkLog.IncidentId = (_nld.IncidentId == 0 ? null : _nld.IncidentId);
                    }
                }
            }
        }
        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        async Task NetworkLogsDeleteAsync(NetworkIncidentUpdateCommand request, Incident entity)
        {
            // Delete Logs;
            foreach (NetworkLogData _nld in request.DeletedLogs)
            {
                if (_nld.Selected == false)
                {
                    NetworkLog _networkLog = await _context.NetworkLogs
                        .FirstOrDefaultAsync(_in => _in.NetworkLogId == _nld.NetworkLogId);
                    if(_networkLog != null)
                    {
                        _context.NetworkLogs.Remove(_networkLog);
                    }
                }
            }
            //
        }
        //
        #endregion // NetworkLogs processing
        //
        //
        /// <summary>
        /// EMail the last ISP Report
        /// </summary>
        /// <param name="data"></param>
        private async Task EMailIspReportAsync(NetworkIncidentUpdateCommand data)
        {
            Incident _incident = await _context.Incidents
                .Include(_i => _i.IncidentIncidentNotes)
                .FirstOrDefaultAsync(i => i.IncidentId == data.IncidentId);
            var _note = _incident.IncidentIncidentNotes
                .Where(_iin => _iin.IncidentNote.NoteType.NoteTypeClientScript == "email")
                .Select(_iin => _iin.IncidentNote).FirstOrDefault();
            if (_note != null)
            {
                try
                {
                    // translate the message from json string of sendgrid type
                    SendGridMessage _sgm = JsonConvert.DeserializeObject<SendGridMessage>(_note.Note);
                    await _notification.SendEmailAsync(MimeKit.SendGridExtensions.NewMimeMessage(_sgm));
                }
                catch (Exception _ex)
                {
                    await Mediator.Send(new LogCreateCommand(
                        LoggingLevel.Warning, MethodBase.GetCurrentMethod(),
                        _ex.GetBaseException().Message, _ex));
                    System.Diagnostics.Debug.WriteLine(_ex.ToString());
                    throw (_ex);
                }
            }
        }
        //
        /// <summary>
        /// FluentValidation of the 'NetworkIncidentUpdateCommand' class.
        /// </summary>
        public class Validator : AbstractValidator<NetworkIncidentUpdateCommand>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'NetworkIncidentUpdateCommand' validator.
			/// </summary>
			public Validator()
			{
				//
				RuleFor(x => x.IncidentId).NotNull().GreaterThan(0);
				RuleFor(x => x.ServerId).NotNull().GreaterThan(0);
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
                //
                RuleFor(n => n.IncidentNotes).NotNull();
                RuleFor(n => n.DeletedNotes).NotNull();
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
	/// Custom NetworkIncidentUpdateCommand record not found exception.
	/// </summary>
	public class NetworkIncidentUpdateCommandKeyNotFoundException : KeyNotFoundException
	{
		//
		/// <summary>
		/// Implementation of NetworkIncidentUpdateCommand record not found exception.
		/// </summary>
		/// <param name="id">The key for the record.</param>
		public NetworkIncidentUpdateCommandKeyNotFoundException(long incidentId)
			: base($"NetworkIncidentUpdateCommand key not found exception: id: {incidentId}")
		{
		}
	}
	//
	/// <summary>
	/// Custom NetworkIncidentUpdateCommand validation exception.
	/// </summary>
	public class NetworkIncidentUpdateCommandValidationException : Exception
	{
		//
		/// <summary>
		/// Implementation of NetworkIncidentUpdateCommand validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public NetworkIncidentUpdateCommandValidationException(string errorMessage)
			: base($"NetworkIncidentUpdateCommand validation exception: errors: {errorMessage}")
		{
		}
	}
    //
    /// <summary>
    /// Custom NetworkIncidentUpdateCommand permissions exception.
    /// </summary>
    public class NetworkIncidentUpdateCommandPermissionsException : Exception
    {
        //
        /// <summary>
        /// Implementation of NetworkIncidentUpdateCommand permissions exception.
        /// </summary>
        /// <param name="errorMessage">The permissions error messages.</param>
        public NetworkIncidentUpdateCommandPermissionsException(string errorMessage)
            : base($"NetworkIncidentUpdateCommand permissions exception: {errorMessage}")
        {
        }
    }
    //
}
// ---------------------------------------------------------------------------
