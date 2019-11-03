using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging.Internal;
//
using Moq;
using NSG.WebSrv.Infrastructure.Notification;
using MimeKit;
using MimeKit.NSG;
//
namespace NSG.WebSrv_Tests.Infrastructure
{
    [TestClass]
    public class Notification_UnitTests
    {
        //
        public IConfiguration Configuration { get; set;  }
        IOptions<MimeKit.NSG.EmailSettings> emailSettings = null;
        Mock<ILogger<NotificationService>> mockLogger = null;

        //
        public Notification_UnitTests()
        {
            string _appSettings = "appsettings.json";
            if (_appSettings != "")
                if (!File.Exists(_appSettings))
                    throw new FileNotFoundException($"Settings file: {_appSettings} not found.");
            Configuration = new ConfigurationBuilder()
                .AddJsonFile(_appSettings, optional: true, reloadOnChange: false)
                .Build();
            emailSettings =
                Options.Create<MimeKit.NSG.EmailSettings>(Configuration.GetSection("EmailSettings").Get<MimeKit.NSG.EmailSettings>());
            //
            mockLogger = new Mock<ILogger<NotificationService>>();
            //
        }
        //
        [TestInitialize()]
        public void MyTestInitialize()
        {
        }
        //
        [TestMethod]
        public async Task SendEmailAsyncEmail_Test()
        {
            INotificationService _notificationService = new NotificationService(emailSettings, mockLogger.Object);
            await _notificationService.SendEmailAsync("Email@anybody.net", "Email Testing", "Email testing message.");
            // asserts
            mockLogger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once);
        }
        //
        [TestMethod]
        public async Task SendEmailAsyncFromTo_Test()
        {
            INotificationService _notificationService = new NotificationService(emailSettings, mockLogger.Object);
            await _notificationService.SendEmailAsync("FromTo@site.net", "FromTo@anybody.net", "FromTo Testing", "FromTo testing message.");
            // asserts
            mockLogger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once);
        }
        //
        [TestMethod]
        public async Task SendEmailAsyncMimeMessage_Test()
        {
            MimeMessage _mimeMessage = MimeKit.Extensions.NewMimeMessage()
                .From("mm@site.net").To("mm@anybody.net").Subject("mm Testing").Body(MimeKit.Extensions.TextBody("mm testing message."));
            INotificationService _notificationService = new NotificationService(emailSettings, mockLogger.Object);
            await _notificationService.SendEmailAsync(_mimeMessage);
            // asserts
            mockLogger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once);
        }
        //
        [TestMethod]
        public async Task SendEmailAsyncMimeMessageBad_Test()
        {
            MimeMessage _mimeMessage = MimeKit.Extensions.NewMimeMessage();
            INotificationService _notificationService = new NotificationService(emailSettings, mockLogger.Object);
            try
            {
                await _notificationService.SendEmailAsync(_mimeMessage);
            }
            catch (InvalidOperationException _ex)
            {
                Console.WriteLine(_ex.Message);
            }
            catch(Exception _ex)
            {
                Console.WriteLine(_ex.Message);
                Assert.Fail(_ex.Message);
            }
        }
        //
    }
}
