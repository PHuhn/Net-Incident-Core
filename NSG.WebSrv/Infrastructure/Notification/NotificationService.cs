//
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
//
using MimeKit;
using MimeKit.NSG;
//
namespace NSG.WebSrv.Infrastructure.Notification
{
    //
    // ASP.NET Core 2.2 - SMTP EmailSender Implementation
    // https://kenhaggerty.com/articles/article/aspnet-core-22-smtp-emailsender-implementation
    //
    public class NotificationService : INotificationService, IEmailSender
    {
        //private readonly IHostingEnvironment _env;
        //IHostingEnvironment env,
        //_env = env;
        private readonly EmailSettings _emailSettings;
        ILogger _logger;
        //
        public NotificationService(
            IOptions<EmailSettings> emailSettings,
            ILogger<NotificationService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }
        //
        /// <summary>
        /// For account controller communications, such as:
        /// </summary>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns>void/complete</returns>
        public Task SendEmailAsync(string email, string subject, string message)
        {
            return SendEmailAsync(Extensions.NewMimeMessage()
                .From(_emailSettings.UserEmail, _emailSettings.UserName).To(email)
                .Subject(subject).Body(Extensions.TextBody(message)));
        }
        //
        /// <summary>
        /// For general communications, such as:
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns>void/complete</returns>
        public Task SendEmailAsync(string from, string to, string subject, string message)
        {
            return SendEmailAsync(MimeKit.Extensions.NewMimeMessage()
                .From(from).To(to).Subject(subject).Body(Extensions.TextBody(message)));
        }
        //
        /// <summary>
        /// For very general communications, including attachments and html messages.
        /// </summary>
        /// <param name="mimeMessage"></param>
        /// <returns>void/complete</returns>
        public Task SendEmailAsync(MimeKit.MimeMessage mimeMessage)
        {
            try
            {
                Task<MimeKit.MimeMessage> _email = mimeMessage.SendAsync(_emailSettings);
                _logger.LogInformation(_email.Result.EmailToString());
            }
            catch (System.Exception _ex)
            {
                _logger.LogError(_ex.ToString());
                throw new System.InvalidOperationException(_ex.Message);
            }
            return Task.CompletedTask;
        }
        //
    }
}
//
