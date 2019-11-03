using System;
using System.Diagnostics;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSG.WebSrv.Infrastructure.Common;
using NSG.WebSrv.Infrastructure.Notification;
//
// using NSG.WebSrv.Models;
using NSG.WebSrv.UI.ViewModels;
//
namespace NSG.WebSrv.UI.Controllers
{
    public class HomeController : BaseController
    {
        private INotificationService _notificationService;
        ILogger<HomeController> _logger;
        IApplication _application;
        //
        /// <summary>
        /// Base constructors, so initialize Alerts list of alert-messages.
        /// </summary>
        public HomeController(INotificationService notificationService, 
            IApplication application, ILogger<HomeController> logger)
        {
            _notificationService = notificationService;
            _application = application;
            _logger = logger;
        }
        public IActionResult Index()
        {
            _logger.Log(LogLevel.Information, "In home page");
            return View();
        }
        //
        public IActionResult Privacy()
        {
            return View();
        }
        //
        public IActionResult Bootstrap()
        {
            return View();
        }
        //
        public IActionResult TestAlerts()
        {
            Error("Error message...");
            Warning("Warning message...");
            Success("Success message...");
            Information("Information, About Page...");
            return View();
        }
        //
        public ActionResult About()
        {
            var _about = new AboutViewModel();
            //
            return View(_about);
        }
        //
        //  home/Contact
        public ActionResult Contact()
        {
            ViewBag.Title = "Contact Us";
            return View();
        }
        //
        //  home/help
        public ActionResult Help()
        {
            ViewBag.Title = "Home Page";
            return View();
        }
        //
        public ActionResult TestUser()
        {
            var _model = new TestUserViewModel()
            {
                UserHttpContext = _application.GetUserAccount(),
                UserClaimsPrincipal = Base_GetUserAccount()
            };
            return View(_model);
        }
        //
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
