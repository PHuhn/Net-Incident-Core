using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
//
using NSG.WebSrv.Domain.Entities;
using NSG.WebSrv.Application.Commands.ApplicationUsers;
using MediatR;
//
namespace NSG.WebSrv.UI.Identity.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private IMediator _mediator;
        //
        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            IMediator mediator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _mediator = mediator;
        }
        //
        public string Username { get; set; }
        //
        public bool IsEmailConfirmed { get; set; }
        //
        [TempData]
        public string StatusMessage { get; set; }
        //
        [BindProperty]
        public InputModel Input { get; set; }
        //
        public class InputModel
        {
            public string UserName { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
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
            [MaxLength(100)]
            [Display(Name = "Full Name")]
            public string FullName { get; set; }
            //
            [Required]
            [MaxLength(16)]
            [Display(Name = "Nic Name")]
            public string UserNicName { get; set; }
            //
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userName = await _userManager.GetUserNameAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

            Username = userName;

            Input = new InputModel
            {
                UserName = userName,
                Email = email,
                PhoneNumber = phoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                UserNicName = user.UserNicName
            };

            return Page();
        }
        //
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            //
            ApplicationUserManageUpdateCommand model = new ApplicationUserManageUpdateCommand()
            {
                UserName = Input.UserName,
                Email = Input.Email,
                PhoneNumber = Input.PhoneNumber,
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                FullName = Input.FullName,
                UserNicName = Input.UserNicName
            };
            //
            int ret = await _mediator.Send(model);
            //
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }


            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = userId, code = code },
                protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(
                email,
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToPage();
        }
    }
}
