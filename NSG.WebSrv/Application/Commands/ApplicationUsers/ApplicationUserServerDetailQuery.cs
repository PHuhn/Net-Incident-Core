//
// ---------------------------------------------------------------------------
// ApplicationUsers detail query.
//
using System;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
//
using Microsoft.EntityFrameworkCore;
using MediatR;
using FluentValidation;
using FluentValidation.Results;
using NSG.WebSrv.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using NSG.WebSrv.Application.Infrastructure;
using NSG.WebSrv.Application.Commands.Incidents;
using NSG.WebSrv.Application.Commands.Servers;
//
namespace NSG.WebSrv.Application.Commands.ApplicationUsers
{
	//
	/// <summary>
	/// 'ApplicationUser' detail query, handler and handle.
	/// </summary>
	public class ApplicationUserServerDetailQuery
	{
        //
        #region "UserServer Class Properties"
        //
        /// <summary>
        /// For column Id
        /// </summary>
		[System.ComponentModel.DataAnnotations.Key]
        public string Id { get; set; }
        //
        /// <summary>
        /// For column UserName
        /// </summary>
        public string UserName { get; set; }
        //
        /// <summary>
        /// For column FirstName
        /// </summary>
        public string FirstName { get; set; }
        //
        /// <summary>
        /// For column LastName
        /// </summary>
        public string LastName { get; set; }
        //
        /// <summary>
        /// For column FullName
        /// </summary>
        public string FullName { get; set; }
        //
        /// <summary>
        /// For column UserNicName
        /// </summary>
        public string UserNicName { get; set; }
        //
        /// <summary>
        /// For column Email
        /// </summary>
        public string Email { get; set; }
        //
        /// <summary>
        /// For column EmailConfirmed
        /// </summary>
        public bool EmailConfirmed { get; set; }
        //
        /// <summary>
        /// For column PhoneNumber
        /// </summary>
        public string PhoneNumber { get; set; }
        //
        /// <summary>
        /// For column PhoneNumberConfirmed
        /// </summary>
        public bool PhoneNumberConfirmed { get; set; }
        //
        /// <summary>
        /// For column CompanyId
        /// </summary>
        public int CompanyId { get; set; }
        //
        /// <summary>
        /// For collection of ServerShortName
        /// </summary>
        public SelectItem[] ServerShortNames { get; set; }
        //
        /// <summary>
        /// For collection of ServerShortName
        /// </summary>
        public string ServerShortName { get; set; }
        //
        /// <summary>
        /// The currently selected server
        /// </summary>
        public ServerData Server { get; set; }
        //
        /// <summary>
        /// For collection of roles
        /// </summary>
        public string[] Roles { get; set; }
        //
        /// <summary>
        /// Create a 'to string'.
        /// </summary>
        public override string ToString()
        {
            //
            StringBuilder _return = new StringBuilder("record:[");
            _return.AppendFormat("Id: {0}, ", Id);
            _return.AppendFormat("UserName: {0}, ", UserName);
            _return.AppendFormat("Email: {0}, ", Email);
            _return.AppendFormat("FirstName: {0}, ", FirstName);
            _return.AppendFormat("LastName: {0}, ", LastName);
            _return.AppendFormat("FullName: {0}, ", FullName);
            _return.AppendFormat("UserNicName: {0}, ", UserNicName);
            _return.AppendFormat("CompanyId: {0}, ", CompanyId.ToString());
            _return.AppendFormat("EmailConfirmed: {0}, ", EmailConfirmed.ToString());
            _return.AppendFormat("PhoneNumber: {0}, ", PhoneNumber);
            _return.AppendFormat("PhoneNumberConfirmed: {0}, ", PhoneNumberConfirmed.ToString());
            _return.AppendFormat("ServerShortName: {0}, ", ServerShortName);
            if( Roles != null )
                _return.AppendFormat("Roles: {0}, ", string.Join(", ", Roles));
            return _return.ToString();
            //
        }
        //
        #endregion
        //
    }
    //
    /// <summary>
    /// 'ApplicationUser' detail query handler.
    /// </summary>
    public class ApplicationUserServerDetailQueryHandler : IRequestHandler<ApplicationUserServerDetailQueryHandler.DetailQuery, ApplicationUserServerDetailQuery>
	{
        private UserManager<ApplicationUser> _userManager;
        private IDb_Context _context;
        // private IMediator Mediator;
        // <param name="mediator">MediatR dependency injector.</param>
        //
        /// <summary>
        ///  The constructor for the inner handler class, to detail the ApplicationUser entity.
        /// </summary>
        /// <param name="userManager">The identity interface for users.</param>
        public ApplicationUserServerDetailQueryHandler(UserManager<ApplicationUser> userManager, IDb_Context context)
        {
            _userManager = userManager;
            _context = context;
            // , IMediator mediator
            // Mediator = mediator;
        }
        //
        /// <summary>
        /// 'ApplicationUser' query handle method, passing two interfaces.
        /// </summary>
        /// <param name="request">This detail query request.</param>
        /// <param name="cancellationToken">Cancel token.</param>
        /// <returns>Returns the row count.</returns>
        public async Task<ApplicationUserServerDetailQuery> Handle(DetailQuery request, CancellationToken cancellationToken)
		{
			Validator _validator = new Validator();
			ValidationResult _results = _validator.Validate(request);
			if (!_results.IsValid)
			{
				// Call the FluentValidationErrors extension method.
				throw new ApplicationUserServerDetailQueryValidationException(_results.FluentValidationErrors());
			}
            //
            ApplicationUser _entity = await _userManager.Users
                .Include(u => u.Company)
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Include(u => u.UserServers).ThenInclude(us => us.Server).ThenInclude(s => s.Company)
                .FirstOrDefaultAsync(r => r.UserName == request.UserName);
			if (_entity == null)
			{
				throw new ApplicationUserServerDetailQueryKeyNotFoundException(request.UserName);
			}
            //
            ApplicationUserServerDetailQuery _detail = _entity.ToApplicationUserServerDetailQuery();
            // Assign Roles array
            List<string> _roles = (List<string>)await _userManager.GetRolesAsync(_entity);
            _detail.Roles = _roles.ToArray();
            // Assign ServerShortName, ServerShortNames and Server
            AssignServer(_detail, _entity, request.ServerShortName);
            // Return the detail query model.
            return _detail;
		}
        //
        /// <summary>
        /// Assign ServerShortName, ServerShortNames and Server.
        /// </summary>
        /// <param name="detail">Output ApplicationUserServerDetailQuery</param>
        /// <param name="entity">Database ApplicationUser entity</param>
        /// <param name="serverShortName"></param>
        void AssignServer(ApplicationUserServerDetailQuery detail, ApplicationUser entity, string serverShortName)
        {
            detail.ServerShortName = "";
            var _serverShortNames = new List<SelectItem>();
            serverShortName = serverShortName.ToLower();
            foreach (var _usrSrv in entity.UserServers)
            {
                string _shortName = _usrSrv.Server.ServerShortName.ToLower();
                var _serverItem = new SelectItem()
                {
                    label = _shortName,
                    value = _usrSrv.Server.ServerId
                };
                if (_shortName == serverShortName)
                {
                    detail.ServerShortName = _usrSrv.Server.ServerShortName;
                    detail.Server = _usrSrv.Server.ToServerData();
                }
                _serverShortNames.Add(_serverItem);
            }
            detail.ServerShortNames = _serverShortNames.ToArray();
        }
        //
        /// <summary>
        /// Get ApplicationUser detail query class (the primary key).
        /// </summary>
        public class DetailQuery : IRequest<ApplicationUserServerDetailQuery>
		{
			public string UserName { get; set; }
            public string ServerShortName { get; set; }
        }
		//
		/// <summary>
		/// FluentValidation of the 'ApplicationUserServerDetailQuery' class.
		/// </summary>
		public class Validator : AbstractValidator<DetailQuery>
		{
			//
			/// <summary>
			/// Constructor that will invoke the 'ApplicationUserServerDetailQuery' validator.
			/// </summary>
			public Validator()
			{
				//
				RuleFor(x => x.UserName).NotEmpty().MaximumLength(256);
                RuleFor(x => x.ServerShortName).MaximumLength(12);
                //
            }
            //
        }
		//
	}
	//
	/// <summary>
	/// Custom ApplicationUserServerDetailQuery record not found exception.
	/// </summary>
	public class ApplicationUserServerDetailQueryKeyNotFoundException: KeyNotFoundException
	{
		//
		/// <summary>
		/// Implementation of ApplicationUserServerDetailQuery record not found exception.
		/// </summary>
		/// <param name="id">The key for the record.</param>
		public ApplicationUserServerDetailQueryKeyNotFoundException(string id)
			: base($"ApplicationUserServerDetailQuery key not found exception: Id: {id}")
		{
		}
	}
	//
	/// <summary>
	/// Custom ApplicationUserServerDetailQuery validation exception.
	/// </summary>
	public class ApplicationUserServerDetailQueryValidationException: Exception
	{
		//
		/// <summary>
		/// Implementation of ApplicationUserServerDetailQuery validation exception.
		/// </summary>
		/// <param name="errorMessage">The validation error messages.</param>
		public ApplicationUserServerDetailQueryValidationException(string errorMessage)
			: base($"ApplicationUserServerDetailQuery validation exception: errors: {errorMessage}")
		{
		}
	}
    //
    /// <summary>
    /// Extension method.
    /// </summary>
    public static partial class Extensions
	{
		//
		/// <summary>
		/// Extension method that translates from ApplicationUser to ApplicationUserServerDetailQuery.
		/// </summary>
		/// <param name="entity">The ApplicationUser entity class.</param>
		/// <returns>'ApplicationUserServerDetailQuery' or ApplicationUser detail query.</returns>
		public static ApplicationUserServerDetailQuery ToApplicationUserServerDetailQuery(this ApplicationUser entity)
		{
			return new ApplicationUserServerDetailQuery
			{
				Id = entity.Id,
				UserName = entity.UserName,
				Email = entity.Email,
				EmailConfirmed = entity.EmailConfirmed,
				PhoneNumber = entity.PhoneNumber,
				PhoneNumberConfirmed = entity.PhoneNumberConfirmed,
				CompanyId = entity.CompanyId,
				FirstName = entity.FirstName,
				FullName = entity.FullName,
				LastName = entity.LastName,
				UserNicName = entity.UserNicName,
            };
		}
	}
}
// ---------------------------------------------------------------------------
