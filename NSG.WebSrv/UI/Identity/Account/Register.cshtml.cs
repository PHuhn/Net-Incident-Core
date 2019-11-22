using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
//
using NSG.WebSrv.Domain.Entities;
//
namespace NSG.WebSrv.UI.Identity.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            //
            [Required]
            [MaxLength(256)]
            [Display(Name = "User Name")]
            public string UserName { get; set; }
            //
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }
            //
            [Required]
            [MaxLength(100)]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }
            //
            [Required]
            [MaxLength(100)]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }
            //
            [Required]
            [MaxLength(16)]
            [Display(Name = "Nic Name")]
            public string UserNicName { get; set; }
            //
            [Required]
            [StringLength(12)]
            [Display(Name = "Server Short Name")]
            public string ServerShortName { get; set; }
            //
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }
            //
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
            //
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var _ok = await CreateUserAsync(Input);
                if(_ok == true)
                {
                    return LocalRedirect(returnUrl);
                }
            }
            // If we got this far, something failed, redisplay form
            return Page();
        }
        //
        /// <summary>
        /// Can be called from this form or from web api.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>true = ok / false = problem</returns>
        public async Task<bool> CreateUserAsync(InputModel model)
        {
            Server _server = await _context.Servers
                .SingleOrDefaultAsync(r => r.ServerShortName == model.ServerShortName, CancellationToken.None);
            if( _server == null )
            {
                ModelState.AddModelError(string.Empty, $"Server: {model.ServerShortName} not found.");
                return false;
            }
            var _user = new ApplicationUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserNicName = model.UserNicName,
                CompanyId = _server.CompanyId
            };
            ApplicationUserServer _userServers = new ApplicationUserServer()
            {
                User = _user,
                Server = _server
            };
            //
            var result = await _userManager.CreateAsync(_user, model.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");
                //
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(_user);
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { userId = _user.Id, code = code },
                    protocol: Request.Scheme);
                //
                await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                //
                await _signInManager.SignInAsync(_user, isPersistent: false);
                return true;
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return false;
        }
    }
}
//
