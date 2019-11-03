using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
//
using NSG.WebSrv.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace NSG.WebSrv.Domain.Entities.Migrations
{
    public static class SeedData
    {
        public static async Task Initialize(ApplicationDbContext context,
            RoleManager<ApplicationRole> roleManager, bool inMemory)
        {
            context.Database.EnsureCreated();
            // initialize company (NSG)
            int _companyId = await SeedCompany(context);
            // initialize server
            int _serverId = await SeedServer(context, _companyId);
            // Roles
            await SeedRole(roleManager);
            //  Network Incident Note Type
            await SeedNoteType(context, inMemory);
            // NIC
            await SeedNIC(context);
            // Incident Types
            await SeedIncidentType(context, inMemory);
            //  Add a couple of fake network incident logs.
            await SeedFakeIncidents(context, _serverId);
            //
        }
        //
        public static async Task<int> SeedCompany(ApplicationDbContext context)
        {
            int _companyId = 1;
            Company _company = null;
            try
            {
                _company = context.Companies.Where(c => c.CompanyShortName == "NSG").FirstOrDefault();
                if (_company == null)
                {
                    _company = new Company()
                    {
                        CompanyShortName = "NSG",
                        CompanyName = "Northern Software Group",
                        Address = "123 Any St.",
                        City = "Ann Arbor", // Anytown
                        State = "MI",
                        PostalCode = "48104",
                        Country = "USA",
                        PhoneNumber = "(734)662-1688",
                        Notes = "Nothing of note."
                    };
                    await context.Companies.AddAsync(_company, CancellationToken.None);
                    context.SaveChanges();
                }
                _companyId = _company.CompanyId;
                //
            }
            catch (Exception _ex)
            {
                Console.WriteLine("SeedCompany");
                Console.WriteLine(_ex.ToString());
            }
            //
            return _companyId;
        }
        //
        public static async Task<int> SeedServer(ApplicationDbContext context, int companyId)
        {
            int _serverId = 1;
            try
            {
                Server _server = null;
                _server = context.Servers.Where(s => s.ServerShortName == "NSG Memb").FirstOrDefault();
                if (_server == null)
                {
                    _server = new Server()
                    {
                        CompanyId = companyId,
                        ServerShortName = "NSG Memb",
                        ServerName = "Members Web-site",
                        ServerDescription = "Public facing members Web-site",
                        WebSite = "Web-site address: www.mimilk.com",
                        ServerLocation = "We are in Michigan, USA.",
                        FromName = "Phil Huhn",
                        FromNicName = "Phil",
                        FromEmailAddress = "PhilHuhn@yahoo.com",
                        TimeZone = "EST (UTC-5)",
                        DST = true,
                        TimeZone_DST = "EDT (UTC-4)",
                        // Daylight saving time 2019 in Michigan began at 2:00 AM on
                        // Sunday, March 10
                        // and ends at 2:00 AM on Sunday, November 3
                        DST_Start = new DateTime(2019, 3, 10, 2, 0, 0),
                        DST_End = new DateTime(2019, 11, 3, 2, 0, 0)
                    };
                    await context.Servers.AddAsync(_server, CancellationToken.None);
                    context.SaveChanges();
                }
                _serverId = _server.ServerId;
                //
            }
            catch (Exception _ex)
            {
                Console.WriteLine("SeedServer");
                Console.WriteLine(_ex.ToString());
            }
            //
            return _serverId;
        }
        //
        // Roles
        //
        public static async Task SeedRole(RoleManager<ApplicationRole> roleManager)
        {
            try
            {
                var _rolePub = new ApplicationRole() { Id = "pub", Name = "Public" };
                var _roleUsr = new ApplicationRole() { Id = "usr", Name = "User" };
                var _roleAdm = new ApplicationRole() { Id = "adm", Name = "Admin" };
                var _roleCAdm = new ApplicationRole() { Id = "cadm", Name = "CompanyAdmin" };
                if (await roleManager.FindByIdAsync(_rolePub.Id) == null)
                {
                    await roleManager.CreateAsync(_rolePub);
                }
                if (await roleManager.FindByIdAsync(_roleUsr.Id) == null)
                {
                    await roleManager.CreateAsync(_roleUsr);
                }
                if (await roleManager.FindByIdAsync(_roleAdm.Id) == null)
                {
                    await roleManager.CreateAsync(_roleAdm);
                }
                if (await roleManager.FindByIdAsync(_roleCAdm.Id) == null)
                {
                    await roleManager.CreateAsync(_roleCAdm);
                }
                //
            }
            catch(Exception _ex)
            {
                Console.WriteLine("SeedRole");
                Console.WriteLine(_ex.ToString());
                System.Diagnostics.Debug.WriteLine(_ex.ToString());
                throw (_ex);
            }
            //
        }
        //
        //  Network Incident Note Type
        //
        public static async Task SeedNoteType(ApplicationDbContext context, bool inMemory)
        {
            //
            if (context.NoteTypes.Count() == 0)
            {
                NoteType[] _noteTypes = new NoteType[]
                {
                    new NoteType() { NoteTypeShortDesc = "Ping", NoteTypeDesc = "Ping", NoteTypeClientScript = "ping" },
                    new NoteType() { NoteTypeShortDesc = "WhoIs", NoteTypeDesc = "WhoIs", NoteTypeClientScript = "whois" },
                    new NoteType() { NoteTypeShortDesc = "ISP Rpt", NoteTypeDesc = "Abuse Report to ISP", NoteTypeClientScript = "email" },
                    new NoteType() { NoteTypeShortDesc = "ISP Addl", NoteTypeDesc = "Additional Communication from ISP", NoteTypeClientScript = "" },
                    new NoteType() { NoteTypeShortDesc = "ISP Resp", NoteTypeDesc = "ISP Response", NoteTypeClientScript = "" }
                };
                // await context.NoteTypes.AddRangeAsync(_noteTypes);
                // inner exception. --->System.Data.SqlClient.SqlException: Cannot insert explicit value for identity column in table 'NoteType' when IDENTITY_INSERT is set to OFF.
                try
                {
                    foreach( NoteType _nt in _noteTypes )
                    {
                        await context.NoteTypes.AddAsync(_nt, CancellationToken.None);
                    }
                    //if (!inMemory)
                    //    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT NoteType ON");
                    context.SaveChanges();
                    //if (!inMemory)
                    //    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT NoteType OFF");
                }
                catch (Exception _ex)
                {
                    Console.WriteLine("SeedNoteType");
                    Console.WriteLine(_ex.ToString());
                    System.Diagnostics.Debug.WriteLine(_ex.ToString());
                    throw (_ex);
                }
                finally
                {
                }
            }
            //
        }
        //
        // NIC
        //
        public static async Task SeedNIC(ApplicationDbContext context)
        {
            //
            if (context.NICs.Count() == 0)
            {
                NIC[] _nics = new NIC[]
                {
                    new NIC() { NIC_Id = "afrinic.net", NICDescription = "Africian Network Information Centre", NICAbuseEmailAddress = " ", NICRestService = "http://www.afrinic.net/", NICWebSite = "http://www.afrinic.net/" },
                    new NIC() { NIC_Id = "apnic.net", NICDescription = "Asian-Pacfic Network Information Centre", NICAbuseEmailAddress = "abuse@org.apnic.net", NICRestService = "https://wq.apnic.net/whois-search/static/search.html?query=", NICWebSite = " " },
                    new NIC() { NIC_Id = "arin.net", NICDescription = "Americian (North) Registry of Internet Numbers", NICAbuseEmailAddress = "abuse@arin.net", NICRestService = "http://whois.arin.net/rest/ip/", NICWebSite = "https://www.arin.net/" },
                    new NIC() { NIC_Id = "lacnic.net", NICDescription = "Latin America and Caribbean Network Information Centre", NICAbuseEmailAddress = "abuse@lacnic.net", NICRestService = "https://rdap.lacnic.net/rdap-web/home", NICWebSite = "http://www.lacnic.net/web/lacnic/inicio" },
                    new NIC() { NIC_Id = "jpnic.net", NICDescription = "Japan", NICAbuseEmailAddress = " ", NICRestService = "https://wq.apnic.net/whois-search/static/search.html?query=", NICWebSite = " " },
                    new NIC() { NIC_Id = "nic.br", NICDescription = "Brazilian Network Information Center", NICAbuseEmailAddress = "cert@cert.br", NICRestService = "https://registro.br/2/whois?query=", NICWebSite = " " },
                    new NIC() { NIC_Id = "ripe.net", NICDescription = "Réseaux IP Européens Network Coordination Centre (Europe)", NICAbuseEmailAddress = "abuse@ripe.net", NICRestService = "https://apps.db.ripe.net/db-web-ui/#/query?searchtext=", NICWebSite = "https://www.ripe.net/" },
                    new NIC() { NIC_Id = "twnic.net", NICDescription = "Taiwan NIC", NICAbuseEmailAddress = " ", NICRestService = "https://www.twnic.net.tw/en_index.php", NICWebSite = "https://www.twnic.net.tw/" },
                    new NIC() { NIC_Id = "hostwinds.com", NICDescription = "hostwinds NIC", NICAbuseEmailAddress = " ", NICRestService = " ", NICWebSite = "https://www.hostwinds.com/" },
                    new NIC() { NIC_Id = "unknown", NICDescription = "Unknown", NICAbuseEmailAddress = " ", NICRestService = " ", NICWebSite = " " },
                    new NIC() { NIC_Id = "other", NICDescription = "Other", NICAbuseEmailAddress = " ", NICRestService = " ", NICWebSite = " " }
                };
                await context.NICs.AddRangeAsync(_nics);
                // requires a single session, thus open it...
                try
                {
                    context.SaveChanges();
                }
                catch (Exception _ex)
                {
                    Console.WriteLine("SeedNIC");
                    Console.WriteLine(_ex.ToString());
                    System.Diagnostics.Debug.WriteLine(_ex.ToString());
                    throw (_ex);
                }
            }
            //
        }
        //
        // Incident Types
        //
        public static async Task SeedIncidentType(ApplicationDbContext context, bool inMemory)
        {
            //
            if (context.IncidentTypes.Count() == 0)
            {
                IncidentType[] _incidentTypes = new IncidentType[]
                {
                    new IncidentType() { IncidentTypeId = 1, IncidentTypeShortDesc = "Unk", IncidentTypeDesc = "Unknown", IncidentTypeFromServer = true, IncidentTypeSubjectLine = "Unknown probe from ${IPAddress}", IncidentTypeEmailTemplate = "Hi\\n\\nStop the intrusion from your IP address ${IPAddress}.\\nThe following IP address probe my network.\\nPlease contain the following reference # in all communications: ${IncidentId}\\n\\n${Device}\\n${ServerLocation}\\nIncident times:", IncidentTypeTimeTemplate = "${NetworkLogDate} ${TimeZone}", IncidentTypeThanksTemplate = "\\nThank you,\\n${FromName}\\n================", IncidentTypeLogTemplate = "\\n${Log}\\n--------------------------------", IncidentTypeTemplate = "-" },
                    new IncidentType() { IncidentTypeId = 2, IncidentTypeShortDesc = "Multiple", IncidentTypeDesc = "Multiple Types", IncidentTypeFromServer = true, IncidentTypeSubjectLine = "Network abuse from ${IPAddress}", IncidentTypeEmailTemplate = "Hi\\n\\nStop the intrusion from your IP address ${IPAddress}.\\nThe following IP address probe my network, probing for multiple vulnerabilities.\\nPlease contain the following reference # in all communications: ${IncidentId}\\n\\n${Device}\\n${ServerLocation}\\nIncident times:", IncidentTypeTimeTemplate = "${IncidentTypeShortDesc}: ${NetworkLogDate} ${TimeZone}", IncidentTypeThanksTemplate = "\\nThank you,\\n${FromName}\\n================", IncidentTypeLogTemplate = "\\n${Log}\\n--------------------------------", IncidentTypeTemplate = "-" },
                    new IncidentType() { IncidentTypeId = 3, IncidentTypeShortDesc = "SQL", IncidentTypeDesc = "SQL Injection", IncidentTypeFromServer = true, IncidentTypeSubjectLine = "SQL Injection probe from ${IPAddress}", IncidentTypeEmailTemplate = "Hi\\n\\nStop the intrusion from your IP address ${IPAddress}.  This is testing SQL injection vulnerabilities.\\nPlease contain the following reference # in all communications: ${IncidentId}\\n\\n${Device}\\n${ServerLocation}\\n\\nIncident times:", IncidentTypeTimeTemplate = "${NetworkLogDate} ${TimeZone}", IncidentTypeThanksTemplate = "\\nThank you,\\n${FromName}\\n================", IncidentTypeLogTemplate = "\\n${Log}\\n--------------------------------", IncidentTypeTemplate = "-" },
                    new IncidentType() { IncidentTypeId = 4, IncidentTypeShortDesc = "PHP", IncidentTypeDesc = "PHP", IncidentTypeFromServer = true, IncidentTypeSubjectLine = "PHP probe from ${IPAddress}", IncidentTypeEmailTemplate = "Hi\\n\\nStop the intrusion from your IP address ${IPAddress}.\\nThe following IP address probe my network, probing for ${IncidentTypeDesc} vulnerabilities.\\nPlease use the following reference # in all communications: ${IncidentId}\\n\\n${Device}\\n${ServerLocation}\\n\\nIncident times:", IncidentTypeTimeTemplate = "${NetworkLogDate} ${TimeZone}", IncidentTypeThanksTemplate = "\\nThank you,\\n${FromName}\\n================", IncidentTypeLogTemplate = "\\n${Log}\\n--------------------------------", IncidentTypeTemplate = "-" },
                    new IncidentType() { IncidentTypeId = 5, IncidentTypeShortDesc = "XSS", IncidentTypeDesc = "Cross Site Scripting", IncidentTypeFromServer = true, IncidentTypeSubjectLine = "XSS probe from ${IPAddress}", IncidentTypeEmailTemplate = "Hi\\n\\nStop the intrusion from your IP address ${IPAddress}.\\nThe following IP address probe my network, probing for ${IncidentTypeDesc} vulnerabilities.\\nPlease use the following reference # in all communications: ${IncidentId}\\n\\n${Device}\\n${ServerLocation}\\n\\nIncident times:", IncidentTypeTimeTemplate = "${NetworkLogDate} ${TimeZone}", IncidentTypeThanksTemplate = "\\nThank you,\\n${FromName}\\n================", IncidentTypeLogTemplate = "\\n${Log}\\n--------------------------------", IncidentTypeTemplate = "-" },
                    new IncidentType() { IncidentTypeId = 6, IncidentTypeShortDesc = "VS", IncidentTypeDesc = "ViewState", IncidentTypeFromServer = true, IncidentTypeSubjectLine = "ViewState probe from ${IPAddress}", IncidentTypeEmailTemplate = "Hi\\n\\nStop the intrusion from your IP address ${IPAddress}.\\nThe following IP address probe my network, probing for ${IncidentTypeDesc} vulnerabilities.\\nPlease use the following reference # in all communications: ${IncidentId}\\n\\n${Device}\\n${ServerLocation}\\n\\nIncident times:", IncidentTypeTimeTemplate = "${NetworkLogDate} ${TimeZone}", IncidentTypeThanksTemplate = "\\nThank you,\\n${FromName}\\n================", IncidentTypeLogTemplate = "\\n${Log}\\n--------------------------------", IncidentTypeTemplate = "-" },
                    new IncidentType() { IncidentTypeId = 7, IncidentTypeShortDesc = "DIR", IncidentTypeDesc = "Directory traversal", IncidentTypeFromServer = true, IncidentTypeSubjectLine = "Directory traversal probe from ${IPAddress}", IncidentTypeEmailTemplate = "Hi\\n\\nStop the intrusion from your IP address ${IPAddress}.\\nThe following IP address probe my network, probing for ${IncidentTypeDesc} vulnerabilities.\\nPlease use the following reference # in all communications: ${IncidentId}\\n\\n${Device}\\n${ServerLocation}\\n\\nIncident times:", IncidentTypeTimeTemplate = "${NetworkLogDate} ${TimeZone}", IncidentTypeThanksTemplate = "\\nThank you,\\n${FromName}\\n================", IncidentTypeLogTemplate = "\\n${Log}\\n--------------------------------", IncidentTypeTemplate = "-" },
                    new IncidentType() { IncidentTypeId = 8, IncidentTypeShortDesc = "DoS", IncidentTypeDesc = "Denial-of-service attack", IncidentTypeFromServer = true, IncidentTypeSubjectLine = "Denial-of-service attack from ${IPAddress}", IncidentTypeEmailTemplate = "Hi\n\nStop the intrusion from your IP address ${IPAddress}.  This is a DoS, affecting by my router.\nPlease contain the following reference # in all communications: ${IncidentId}\n\n${Device}\n${ServerLocation}\n\nIncident times:", IncidentTypeTimeTemplate = "${NetworkLogDate} ${TimeZone}", IncidentTypeThanksTemplate = "\\nThank you,\\n${FromName}\\n================", IncidentTypeLogTemplate = "\\n${Log}\\n--------------------------------", IncidentTypeTemplate = "-" }
                };
                try
                {
                    foreach (IncidentType _it in _incidentTypes)
                    {
                        await context.IncidentTypes.AddAsync(_it, CancellationToken.None);
                    }
                    if (!inMemory)
                        context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT IncidentType ON");
                    context.SaveChanges();
                    if (!inMemory)
                        context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT IncidentType OFF");
                }
                catch (Exception _ex)
                {
                    Console.WriteLine("SeedIncidentType");
                    Console.WriteLine(_ex.ToString());
                    System.Diagnostics.Debug.WriteLine(_ex.ToString());
                    throw (_ex);
                }
                finally
                {
                }
            }
            //
        }
        //
        public static async Task SeedEmailTemplate(ApplicationDbContext context, bool inMemory)
        {
            //
            if (context.EmailTemplates.Count() == 0)
            {
                //CompanyId IncidentTypeId  SubjectLine EmailBody   TimeTemplate ThanksTemplate  LogTemplate Template    FromServer
                //IncidentTypeId  IncidentTypeShortDesc IncidentTypeDesc    IncidentTypeFromServer IncidentTypeSubjectLine IncidentTypeEmailTemplate IncidentTypeTimeTemplate    IncidentTypeThanksTemplate IncidentTypeLogTemplate IncidentTypeTemplate
                // 
                EmailTemplate _emailTemplate = new EmailTemplate()
                {
                    CompanyId = 1,
                    IncidentTypeId = 8,
                    FromServer = true,
                    SubjectLine = "Denial-of-service attack from ${ IPAddress}",
                    EmailBody = "Hi\n\nStop the intrusion from your IP address ${IPAddress}.  This is a DoS, affecting by my router.\nPlease contain the following reference # in all communications: ${IncidentId}\n\n${Device}\n${ServerLocation}\nIncident times:",
                    TimeTemplate = "${NetworkLogDate} ${TimeZone}",
                    ThanksTemplate = "\nThank you,\n${FromName}\n================",
                    LogTemplate = "\n${Log}\n--------------------------------",
                    Template = "-"
                };
                try
                {
                    await context.EmailTemplates.AddAsync(_emailTemplate, CancellationToken.None);
                    context.SaveChanges();
                }
                catch (Exception _ex)
                {
                    Console.WriteLine("Seed_emailTemplate");
                    Console.WriteLine(_ex.ToString());
                    System.Diagnostics.Debug.WriteLine(_ex.ToString());
                    throw (_ex);
                }
                finally
                {
                }
            }
            //
        }
        //
        //  Add a couple of fake network incident logs.
        //
        public static async Task SeedFakeIncidents(ApplicationDbContext context, int serverId)
        {
            //
            if (context.NetworkLogs.Count() == 0)
            {
                try
                {
                    DateTime _dt = DateTime.Now.AddDays(-1);
                    int _incTypeSql = 3;
                    NetworkLog[] _networkLogs = new NetworkLog[]
                    {
                        new NetworkLog() { ServerId = serverId, IncidentId = null, IPAddress = "94.41.54.105", NetworkLogDate = _dt.AddMilliseconds(15), Log = "Fake log 1, Fake log 1, Fake log 1", IncidentTypeId = _incTypeSql },
                        new NetworkLog() { ServerId = serverId, IncidentId = null, IPAddress = "104.42.229.49", NetworkLogDate = _dt.AddMinutes(4), Log = "Fake log 2, Fake log 2, Fake log 2", IncidentTypeId = _incTypeSql },
                        new NetworkLog() { ServerId = serverId, IncidentId = null, IPAddress = "104.42.229.49", NetworkLogDate = _dt.AddMinutes(5), Log = "Fake log 3, Fake log 3, Fake log 3", IncidentTypeId = _incTypeSql },
                        new NetworkLog() { ServerId = serverId, IncidentId = null, IPAddress = "54.183.209.144", NetworkLogDate = _dt.AddMinutes(10), Log = "Fake log 4, Fake log 4, Fake log 4", IncidentTypeId = _incTypeSql }
                    };
                    await context.NetworkLogs.AddRangeAsync(_networkLogs);
                    context.SaveChanges();
                }
                catch (Exception _ex)
                {
                    Console.WriteLine(_ex.ToString());
                    System.Diagnostics.Debug.WriteLine(_ex.ToString());
                }
                //
            }
            //
        }
        //
        public static async Task AddRoleToPhil(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            //
            // 301ff4c5-6b04-4912-bc75-4d2bc3be8ce3
            ApplicationUser _user = context.Users.Where( _u => _u.Id == "301ff4c5-6b04-4912-bc75-4d2bc3be8ce3").FirstOrDefault();
            try
            {
                await userManager.AddToRoleAsync(_user, "admin");
            }
            catch( Exception _ex)
            {
                Console.WriteLine(_ex.Message);
                Console.WriteLine(_ex.ToString());
            }
        }
        //
        public static async Task TestUserManager(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            //
            ApplicationUser _userByMgr =
                ((ApplicationUser)await userManager.FindByNameAsync("Phil"));
            var _roles = await userManager.GetRolesAsync(_userByMgr);
            Console.WriteLine( _userByMgr );
            //
            ApplicationUser _userByCnt = context.Users
                .Include(u => u.Company)
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Include(u => u.UserServers).ThenInclude(us => us.Server)
                .FirstOrDefault(u => u.UserName == "Phil");
            Console.WriteLine(_userByCnt);
            ApplicationRole _roleByCnt = context.Roles
                .Include(u => u.UserRoles).ThenInclude(ur => ur.User)
                .FirstOrDefault(r => r.Id == "adm");
            Console.WriteLine(_userByCnt);
            //
        }
        //
    }
}
//
