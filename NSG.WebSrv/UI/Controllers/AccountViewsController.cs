using System.Globalization;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity.UI.Services;
//
using NSG.WebSrv.UI.ViewModels;
using NSG.WebSrv.Infrastructure.Notification;
using NSG.WebSrv.Domain.Entities;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
//
namespace NSG.WebSrv.UI.Controllers
{
    public class AccountViewsController : BaseController
    {
        private readonly string _codeName = "AccountController";
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AccountViewsController> _logger;
        private readonly INotificationService _emailSender;
        private readonly ApplicationDbContext _context;
        //
        public string ReturnUrl { get; set; }
        //
        public AccountViewsController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context,
            ILogger<AccountViewsController> logger,
            INotificationService emailSender) : base()
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
        }
        //
        #region "Register"
        //
        // GET: /Account/Register
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        //
        /// <summary>
        /// POST: /Account/Register
        /// </summary>
        /// <param name="model">RegisterBindingModel</param>
        /// <returns>view model</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(AccountRegisterViewModel model)
        {
            string returnUrl = Url.Content("~/");
            if (ModelState.IsValid)
            {
                //
                // Get server to be added (and also defines default company)...
                //
                Server _server = _context.Servers
                    .Where(s => s.ServerShortName == model.ServerShortName).FirstOrDefault();
                if (_server != null)
                {
                    int _companyId = _server.CompanyId;
                    ApplicationUser _user = new ApplicationUser()
                    {
                        UserName = model.UserName,
                        CompanyId = _companyId,
                        Email = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        FullName = String.Format("{0} {1}", model.FirstName, model.LastName),
                        UserNicName = (model.FirstName.Length > 16 ? model.FirstName.Substring(0, 16) : model.FirstName),
                        CreateDate = DateTime.Now
                    };
                    //
                    // Add default user role...
                    //
                    // add user with defined server and default role
                    var result = await _userManager.CreateAsync(_user, model.Password);
                    if (result.Succeeded)
                    {
                        //
                        // Add server that was passed as a part of the binding model...
                        //
                        ApplicationUserServer _appUserServer = new ApplicationUserServer() {
                            Id = _user.Id,
                            ServerId = _server.ServerId,
                            User = _user,
                            Server = _server
                        };
                        // _server.UserServers.Add(_appUserServer);
                        _user.UserServers.Add(_appUserServer);
                        _context.SaveChanges();

                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(_user);
                        var callbackUrl = Url.Action("ConfirmEmail", "Account",
                            new { userId = _user.Id, code = code },
                            protocol: Request.Scheme);

                        //var callbackUrl = Url.Page(
                        //    "/Account/ConfirmEmail",
                        //    pageHandler: null,
                        //    values: new { userId = _user.Id, code = code },
                        //    protocol: Request.Scheme);

                        Console.WriteLine("Email: " + _user.Email + ", Subject: "
                            + Constants.ConfirmEmailSubjectLine + ", Body: " + Constants.ConfirmEmailBody
                            + " " + HtmlEncoder.Default.Encode(callbackUrl));

                        await _emailSender.SendEmailAsync(_user.Email,
                            Constants.ConfirmEmailSubjectLine,
                            string.Format(Constants.ConfirmEmailBody, HtmlEncoder.Default.Encode(callbackUrl)));

                        await _signInManager.SignInAsync(_user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                    Account_AddErrors(result);
                }
                else
                    ModelState.AddModelError("", string.Format(Constants.ServerNotFoundException, _codeName, model.ServerShortName));
            }
            // If we got this far, something failed, redisplay form
            Base_AddErrors(ModelState);
            return View(model);
        }
        //
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            //
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }
            //
            var _user = await _userManager.FindByIdAsync(userId);
            if (_user == null)
            {
                return NotFound(string.Format(Constants.UserNotFoundException, _codeName, userId));
            }
            //
            var result = await _userManager.ConfirmEmailAsync(_user, code);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(_user, "pub");
                _logger.LogInformation("User email confirmed, 'public' role added to account.");
            }
            else
            {
                throw new InvalidOperationException($"Error confirming email for user with ID '{userId}':");
            }
            //
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }
        //
        #endregion // Register
        //
        #region "Login"
        //
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        //
        [TempData]
        public string ErrorMessage { get; set; }
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }
            returnUrl = returnUrl ?? Url.Content("~/");
            ReturnUrl = returnUrl;
            ViewBag.ReturnUrl = returnUrl;
            //
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            //
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            ViewBag.ExternalLogins = ExternalLogins;

            return View();
        }
        //
        [AllowAnonymous]
        public async Task<IActionResult> Login(AccountLoginViewModel model, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View();
                }
            }

            // If we got this far, something failed, redisplay form
            return View();
        }
        //
        #endregion // Login
        //
        #region "ForgotPassword"
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public IActionResult ForgotPassword(string returnUrl = null)
        {
            //
            ReturnUrl = returnUrl;
            ViewBag.ReturnUrl = returnUrl;
            //
            return View();
        }
        //
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(AccountForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { code },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(
                    user.Email,
                    Constants.ForgotPasswordSubjectLine,
                    string.Format(Constants.ForgotPasswordBody, HtmlEncoder.Default.Encode(callbackUrl)));

                return RedirectToPage("./ForgotPasswordConfirmation");
            }
            return View();
        }
        //
        /// <summary>
        /// Display 'Please check your email to reset your password.' message.
        /// </summary>
        /// <returns>The view</returns>
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        //
        #endregion // ForgotPassword
        //
        #region "Logout"
        //
        [Authorize]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return LocalRedirect("/home");
            }
        }
        //
        #endregion // Logout
        //
        #region "Helpers"
        //
        //
        /// <summary>
        /// iterate call to Error for unsuccessful identity-result errors.
        /// </summary>
        /// <param name="modelState">IdentityResult </param>
        public void Account_AddErrors(IdentityResult modelState)
        {
            if (!modelState.Succeeded)
                foreach (var _failure in modelState.Errors)
                {
                    base.Error(string.Format(
                        "{0}: {1}\n", _failure.Code, _failure.Description));
                }
        }
        //
        #endregion // Helpers
        //
    }
}