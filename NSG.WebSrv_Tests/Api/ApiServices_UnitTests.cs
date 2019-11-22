using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
//
using Moq;
//
using NSG.WebSrv.UI.Api;
using NSG.WebSrv.Infrastructure.Services;
//
namespace NSG.WebSrv_Tests.Api
{
    [TestClass]
    public class ApiServices_UnitTests
    {
        //
        public IConfiguration Configuration { get; set; }
        ServicesController _servicesController;
        IOptions<ServicesSettings> _servicesSettings = null;
        //
        public ApiServices_UnitTests()
        {
            //
        }
        //
        [TestInitialize()]
        public void MyTestInitialize()
        {
            string _appSettings = "appsettings.json";
            Mock<ILogger<ServicesController>> _mockLogger =
                new Mock<ILogger<ServicesController>>();
            Configuration = new ConfigurationBuilder()
                .AddJsonFile(_appSettings, optional: true, reloadOnChange: false)
                .Build();
            _servicesSettings =
                Options.Create<ServicesSettings>(
                    Configuration.GetSection("ServicesSettings").Get<ServicesSettings>());
            _servicesController = new ServicesController(_mockLogger.Object, _servicesSettings);
            if (_servicesSettings == null)
                throw new Exception("Null services settings");
        }
        //
        [TestMethod]
        public void ApiServicesController_PingAddress46_161_Test()
        {
            // RIPE: Pinging pinspb.ru [46.161.62.245] with 32 bytes of data:
            string _expected = @"
Pinging pinspb.ru [46.161.62.245] with 32 bytes of data:";
            string _actual = ApiServicesController_PingCommand("46.161.62.245");
            Assert.AreEqual(_expected, _actual.Substring(0, _expected.Length));
        }
        //
        [TestMethod]
        public void ApiServicesController_PingAddress64_183_Test()
        {
            // ARIN: Pinging ec2-54-183-209-144.us-west-1.compute.amazonaws.com [54.183.209.144] with 32 bytes of data:
            string _expected = @"
Pinging ec2-54-183-209-144.us-west-1.compute.amazonaws.com [54.183.209.144] with 32 bytes of data:";
            string _actual = ApiServicesController_PingCommand("54.183.209.144");
            Assert.AreEqual(_expected, _actual.Substring(0, _expected.Length));
        }
        //
        [TestMethod]
        public void ApiServicesController_PingAddress01_009_Test()
        {
            // APNIC: Pinging 1.9.149.170 with 32 bytes of data:
            string _expected = @"
Pinging 1.9.149.170 with 32 bytes of data:";
            string _actual = ApiServicesController_PingCommand("1.9.149.170");
            Assert.AreEqual(_expected, _actual.Substring(0, _expected.Length));
        }
        //
        private string ApiServicesController_PingCommand(string ip)
        {
            ActionResult<string> _ping = _servicesController.Ping(ip);
            Console.WriteLine(_ping.Result);
            Console.WriteLine(_ping.Value);
            return _ping.Value;
        }
        //
        [TestMethod]
        public void ApiServicesController_WhoIs46_161_Test()
        {
            // RIPE: Pinging pinspb.ru [46.161.62.245] with 32 bytes of data:
            string ip = "46.161.62.245";
            ActionResult<string> _whois = _servicesController.WhoIs(ip);
            Console.WriteLine(_whois.Result);
            string _actual = _whois.Value;
            Console.WriteLine(ip);
            Console.WriteLine(_actual);
            Assert.IsTrue(_actual.Contains(
                "Abuse contact for '46.161.62.0 - 46.161.62.255' is 'noc@finetransit.eu'") == true);
        }
        //
        [TestMethod]
        public void ApiServicesController_WhoIs174_79_Test()
        {
            // RIPE: Pinging pinspb.ru [46.161.62.245] with 32 bytes of data:
            string ip = "174.79.60.55";
            ActionResult<string> _whois = _servicesController.WhoIs(ip);
            Console.WriteLine(_whois.Result);
            string _actual = _whois.Value;
            Console.WriteLine(ip);
            Console.WriteLine(_actual);
            Assert.IsTrue(_actual.Contains(ip.Substring(0, 6)) == true);
            Assert.IsTrue(_actual.Contains(
                "OrgAbuseEmail:  abuse@cox.net"));
        }
        //
        // WhoIs Tests
        //
        [TestMethod]
        public void ApiServicesController_WhoIsLink_Link_Test()
        {
            string _data = @"[Querying whois.arin.net]
[whois.arin.net]

#
# ARIN WHOIS data and services are subject to the Terms of Use
# available at: https://www.arin.net/whois_tou.html
#
# If you see inaccuracies in the results, please report at
# https://www.arin.net/public/whoisinaccuracy/index.xhtml
#
#
# The following results may also be obtained via:
# https://whois.arin.net/rest/nets;q=174.79.60.55?showDetails=true&showARIN=false&showNonArinTopLevelNet=false&ext=netref2
#

Cox Communications NETBLK-PH-CBS-174-79-32-0 (NET-174-79-32-0-1) 174.79.32.0 - 174.79.63.255
Cox Communications Inc. CXA (NET-174-64-0-0-1) 174.64.0.0 - 174.79.255.255

#
# ARIN WHOIS data and services are subject to the Terms of Use
# available at: https://www.arin.net/whois_tou.html
#
# If you see inaccuracies in the results, please report at
# https://www.arin.net/public/whoisinaccuracy/index.xhtml
#
";
            string _link = _servicesController.WhoIsLink(_data);
            System.Diagnostics.Debug.WriteLine(_link);
            Assert.IsTrue(_link != "");
        }
        //
        [TestMethod]
        public void ApiServicesController_WhoIsLink_NoLink1_Test()
        {
            string _data = @"[Querying whois.arin.net]
[whois.arin.net]

#
# ARIN WHOIS data and services are subject to the Terms of Use
# available at: https://www.arin.net/whois_tou.html
#
# If you see inaccuracies in the results, please report at
# https://www.arin.net/public/whoisinaccuracy/index.xhtml
#

NetRange:       174.79.32.0 - 174.79.63.255
CIDR:           174.79.32.0/19
NetName:        NETBLK-PH-CBS-174-79-32-0
NetHandle:      NET-174-79-32-0-1
Parent:         CXA (NET-174-64-0-0-1)
NetType:        Reassigned
OriginAS:       
Customer:       Cox Communications (C02309424)
RegDate:        2009-09-04
Updated:        2011-03-24
Ref:            https://whois.arin.net/rest/net/NET-174-79-32-0-1


CustName:       Cox Communications
Address:        1400 Lake Hearn Drive
City:           Atlanta
StateProv:      GA
PostalCode:     30319
Country:        US
RegDate:        2009-09-04
Updated:        2011-03-24
Ref:            https://whois.arin.net/rest/customer/C02309424

OrgAbuseHandle: IC146-ARIN
OrgAbuseName:   Cox Communications Inc
OrgAbusePhone:  +1-404-269-7626 
OrgAbuseEmail:  abuse@cox.net
OrgAbuseRef:    https://whois.arin.net/rest/poc/IC146-ARIN

OrgTechHandle: BAABO-ARIN
OrgTechName:   BA, Aboubakr 
OrgTechPhone:  +1-404-269-4416 
OrgTechEmail:  abuse@cox.net
OrgTechRef:    https://whois.arin.net/rest/poc/BAABO-ARIN

OrgTechHandle: ADA131-ARIN
OrgTechName:   Anderson, Alvin Demond
OrgTechPhone:  +1-404-269-4416 
OrgTechEmail:  alvin.anderson@cox.com
OrgTechRef:    https://whois.arin.net/rest/poc/ADA131-ARIN

OrgTechHandle: BERUB3-ARIN
OrgTechName:   Berube, Tori 
OrgTechPhone:  +1-404-269-4416 
OrgTechEmail:  tori.berube@cox.com
OrgTechRef:    https://whois.arin.net/rest/poc/BERUB3-ARIN

OrgTechHandle: NIA16-ARIN
OrgTechName:   National IP Administrator
OrgTechPhone:  +1-404-269-4416 
OrgTechEmail:  tiffany.coleman@cox.com
OrgTechRef:    https://whois.arin.net/rest/poc/NIA16-ARIN

OrgTechHandle: RWA196-ARIN
OrgTechName:   Waldron, Roderick 
OrgTechPhone:  +1-404-269-7626 
OrgTechEmail:  abuse@cox.net
OrgTechRef:    https://whois.arin.net/rest/poc/RWA196-ARIN

OrgTechHandle: MEROL3-ARIN
OrgTechName:   Merola, Cari 
OrgTechPhone:  +1-404-269-4416 
OrgTechEmail:  cari.merola@cox.com
OrgTechRef:    https://whois.arin.net/rest/poc/MEROL3-ARIN


#
# ARIN WHOIS data and services are subject to the Terms of Use
# available at: https://www.arin.net/whois_tou.html
#
# If you see inaccuracies in the results, please report at
# https://www.arin.net/public/whoisinaccuracy/index.xhtml
#
";
            string _link = _servicesController.WhoIsLink(_data);
            System.Diagnostics.Debug.WriteLine(_link);
            Assert.AreEqual("", _link);
        }
        //
        [TestMethod]
        public void ApiServicesController_WhoIsLink_NoLink2_Test()
        {
            string _data = @"[Querying whois.arin.net]
[Redirected to rwhois.singlehop.net:4321]
[Querying rwhois.singlehop.net]
[rwhois.singlehop.net]
%rwhois V-1.5:003eff:00 rwhois.singlehop.com (by Network Solutions, Inc. V-1.5.9.5)
network:Class-Name:network
network:ID:ORG-SINGL-8.184-154-139-0/26
network:Auth-Area:184.154.0.0/16
network:IP-Network:184.154.139.0/26
network:Organization:Innovative Business Services (dba SiteLock)
network:Street-Address:8125 N 86th Place
network:City:Scottsdale
network:State:AZ
network:Postal-Code:85258
network:Country-Code:US
network:Tech-Contact;I:NETWO1546-ARIN
network:Admin-Contact;I:NETWO1546-ARIN
network:Abuse-Contact;I:ABUSE2492-ARIN
network:Created:20150331
network:Updated:20150331

%referral rwhois://root.rwhois.net:4321/auth-area=.
%ok

";
            string _link = _servicesController.WhoIsLink(_data);
            System.Diagnostics.Debug.WriteLine(_link);
            Assert.AreEqual("", _link);
        }
        //
    }
}
