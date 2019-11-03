//
using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
//
using MediatR;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
//
namespace NSG.WebSrv.UI.Controllers
{
    //
    /// <summary>
    /// The alert message with the log level of the message.
    /// </summary>
    public class AlertMessage
    {
        public string Level { get; set; }
        public string Message { get; set; }
        public AlertMessage(string level, string message)
        {
            // this.Key = "";
            // this.Code = "";
            this.Level = level;
            this.Message = message;
        }
    }
    //
    /// <summary>
    /// The lower case string value is appended to "nsg-msg-", to get
    /// the class to display the message with the appropriate colors.
    /// </summary>
    public enum AlertLevel
    {
        Error = 0,
        Warn = 1,
        Success = 2,
        Info = 3
    }
    //
    /// <summary>
    /// Base view controler.
    /// This requires adding the following:
    ///   partial name="_Alerts"
    /// </summary>
    public class BaseController : Controller
    {
        private IMediator _mediator;
        protected IMediator Mediator => _mediator ?? (_mediator = HttpContext.RequestServices.GetService<IMediator>());
        //
        // Issue:
        //  System.InvalidOperationException: The 'Microsoft.AspNetCore.Mvc.ViewFeatures.Internal.TempDataSerializer' cannot serialize an object of type 'NSG.WebSrv.Controllers.AlertMessage'.
        // Solution:
        //  Use static Alerts, and removed TempData.
        /// <summary>
        /// Collection of messages with level.  This is displayed with _Alerts.chtml
        /// shared partial view which is added to the _layout partial.
        /// </summary>
        public static List<AlertMessage> Alerts = new List<AlertMessage>();
        //
        /// <summary>
        /// Base constructors, so initialize Alerts list of alert-messages.
        /// </summary>
        public BaseController()
        {
            //
        }
        //
        //  Error
        //  Warning
        //  Success
        //  Information
        //  AddAlertMessage
        //
        #region "alert message"
        //
        /// <summary>
        /// Add an error (AlertLevel) AlertMessage to list of Alerts.
        /// </summary>
        /// <param name="message">an error message</param>
        public void Error(string message)
        {
            AddAlertMessage(AlertLevel.Error, message);
        }
        //
        /// <summary>
        /// Add a warning (AlertLevel) AlertMessage to list of Alerts.
        /// </summary>
        /// <param name="message">a warning message</param>
        public void Warning(string message)
        {
            AddAlertMessage(AlertLevel.Warn, message);
        }
        //
        /// <summary>
        /// Add a success (AlertLevel) AlertMessage to list of Alerts.
        /// </summary>
        /// <param name="message">a success message</param>
        public void Success(string message)
        {
            AddAlertMessage(AlertLevel.Success, message);
        }
        //
        /// <summary>
        /// Add a info (AlertLevel) AlertMessage to list of Alerts.
        /// </summary>
        /// <param name="message">a info message</param>
        public void Information(string message)
        {
            AddAlertMessage(AlertLevel.Info, message);
        }
        //
        /// <summary>
        /// do it in one place
        /// </summary>
        /// <param name="alertLevel">level of the message</param>
        /// <param name="message">a message</param>
        private void AddAlertMessage( AlertLevel alertLevel, string message )
        {
            Alerts.Add(new AlertMessage(alertLevel.ToString().ToLower(), message));
        }
        //
        #endregion // alert message
        //
        /// <summary>
        /// iterate call to Error for identity-result errors.
        /// </summary>
        /// <param name="result">results from identity call</param>
        public void Base_AddErrors(Exception except)
        {
            except = except.GetBaseException();
            this.AddAlertMessage(AlertLevel.Error, except.Message);
        }
        //
        /// <summary>
        /// iterate call to Error for invalid model state errors.
        /// </summary>
        /// <param name="modelState">ModelState from view model</param>
        public void Base_AddErrors(ValidationResult modelState)
        {
            if (!modelState.IsValid)
                foreach (var _failure in modelState.Errors)
                {
                    this.AddAlertMessage(AlertLevel.Error, 
                        string.Format("{0}: {1}\n", _failure.PropertyName, _failure.ErrorMessage));
                }
        }
        //
        /// <summary>
        /// iterate call to Error for unsuccessful identity-result errors.
        /// </summary>
        /// <param name="modelState">IdentityResult </param>
        public void Base_AddErrors(IdentityResult modelState)
        {
            if (!modelState.Succeeded)
                foreach (var _failure in modelState.Errors)
                {
                    this.AddAlertMessage(AlertLevel.Error,
                        string.Format("{0}: {1}\n", _failure.Code, _failure.Description));
                }
        }
        //
        /// <summary>
        /// iterate call to Error for invalid model state errors.
        /// </summary>
        /// <param name="modelState">ModelState from view model</param>
        public void Base_AddErrors(ModelStateDictionary modelState)
        {
            if (!modelState.IsValid)
                foreach (ModelError me in (modelState.Values.SelectMany(e => (e.Errors))))
                {
                    if (string.IsNullOrEmpty(me.ErrorMessage))
                    {
                        if (me.Exception != null)
                            this.AddAlertMessage(AlertLevel.Error, me.Exception.Message);
                    }
                    else
                        this.AddAlertMessage(AlertLevel.Error, me.ErrorMessage);
                }
        }
        //
        // -------------------------------------------------------------------
        //
        //
        /// <summary>
        /// Get the current user's user name identity via controller's ClaimsPrincipal.
        /// </summary>
        /// <returns>String of the current user.</returns>
        public string Base_GetUserAccount()
        {
            var currentUserName = "- Not Authenticated -";
            ClaimsPrincipal currentUser = this.User;
            if (currentUser.Identity.IsAuthenticated)
                currentUserName = currentUser.FindFirst(ClaimTypes.Name).Value;
            if (string.IsNullOrEmpty(currentUserName))
                currentUserName = "- Not Authenticated -";
            return currentUserName;
        }
        //
    }
    //
}
