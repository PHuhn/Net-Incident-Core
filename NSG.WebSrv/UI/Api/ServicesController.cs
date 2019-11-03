//
// ---------------------------------------------------------------------------
//
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
//
using NSG.WebSrv.Infrastructure.Services;
//
namespace NSG.WebSrv.UI.Api
{
    //
    // -----------------------------------------------------------------------
    //
    // -Authorize(Policy = "AnyUserRole")
    // -Authorize
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController : BaseApiController
    {
        //
        /// <summary>
        /// This class maps to the appsettings.json 'ServicesSettings' JSON class.
        /// </summary>
        protected readonly ServicesSettings _servicesSettings = null;
        protected readonly ILogger<ServicesController> _logger = null;
        //
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="servicesSettings">
        /// Class that maps to the appsettings.json, the values needed for the services.
        /// </param>
        public ServicesController(ILogger<ServicesController> logger, IOptions<ServicesSettings> servicesSettings)
        {
            _logger = logger;
            _servicesSettings = servicesSettings.Value;
        }
        //
        // [ActionName("ping")]
        /// <summary>
        /// GET api/services/ping/192.169.3.2
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        [HttpGet("Ping/{ip}")]
        public ActionResult<string> Ping(string ip)
        {
            _logger?.LogDebug("'{0}' has been invoked with {1}.", nameof(Ping), ip);
            return IpAddressCommand(ip,
                _servicesSettings.PingDir, _servicesSettings.PingCmd);
        }
        //
        // [ActionName("whois")]
        /// <summary>
        /// GET api/services/whois/192.169.3.2
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        [HttpGet("WhoIs/{ip}")]
        public ActionResult<string> WhoIs(string ip)
        {
            _logger?.LogDebug("'{0}' has been invoked with {1}.", nameof(WhoIs), ip);
            string _whois = IpAddressCommand(ip,
                _servicesSettings.WhoisDir, _servicesSettings.WhoisCmd);
            string _link = WhoIsLink(_whois);
            if (_link != "")
                return IpAddressCommand(_link,
                    _servicesSettings.WhoisDir, _servicesSettings.WhoisCmd);
            return _whois;
        }
        //
        // -----------------------------------------------------------------------
        //
        private string IpAddressCommand(string ip, string dir, string command)
        {
            string _return = "";
            if (ip != "")
            {
                ip = ip.Trim();
                if (ip.Substring(0, 1).CompareTo("9") < 1 || ip.Substring(0, 3).ToLower() == "net")
                {
                    string _cmdText = String.Format(command, ip);
                    _return = NSG.Library.Helpers.OS.CallOperatingSystemCmd(_cmdText, dir);
                }
            }
            return _return;
        }
        //
        /// <summary>
        /// link code
        /// </summary>
        /// <param name="whoisData"></param>
        /// <returns>string of ip address or empty</returns>
        public string WhoIsLink(string whoisData)
        {
            string _link = "";
            bool _processFlag = true;
            string _nic = "";
            foreach (string _line in whoisData.Split(new[] { '\r', '\n' }))
            {
                if (_processFlag && _line.Length > 3 && (_line.Substring(0, 1) != "#" && _line.Substring(0, 1) != "%"))
                {
                    if (_line.Substring(0, 1) == "[")
                    {
                        // whois. and jwhois.
                        int _pos = _line.IndexOf("whois.");
                        if (_pos == -1)
                        {
                            // [vault.krypt.com] length is the same!
                            _pos = _line.IndexOf("vault.");
                        }
                        if (_pos > -1)
                        {
                            string[] parts = _line.Substring(_pos).Split('.');
                            _nic = _line.Substring(_pos + 6, _line.IndexOf("]") - (_pos + 6));
                        }
                    }
                    // System.Diagnostics.Debug.WriteLine(_line);
                    if (_line.Substring(0, 8).ToLower() == "netname:")
                        return "";
                    else
                    {
                        string[] _a1 = _line.Split('(');
                        if (_a1.Length > 1)
                        {
                            string[] _a2 = _a1[1].Split(')');
                            if (_a2.Length > 1)
                            {
                                if (_a2[0].Substring(0, 3) == "NET")
                                {
                                    _link = _a2[0];
                                    System.Diagnostics.Debug.WriteLine(_nic + " : " + _link);
                                    return _link;
                                }
                            }
                        }
                    }
                }
            }
            return _link;
        }
        //
    }
}