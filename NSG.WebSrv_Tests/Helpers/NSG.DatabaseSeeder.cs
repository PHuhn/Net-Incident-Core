//
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NSG.WebSrv.Domain.Entities;
using NSG.WebSrv.Domain.Entities.Migrations;
//
namespace NSG.Integration.Helpers
{
    public class DatabaseSeeder
    {
        //
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        //
        public DatabaseSeeder(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        //
        public async Task Seed()
        {
            //
            Console.WriteLine("Entering Seed ...");
            //
            await SeedData.Initialize(_context, _roleManager, true);
            await SeedData.SeedEmailTemplate(_context, true);
            //
            var users = await _context.Users.ToListAsync(); // needs Microsoft.EntityFrameworkCore;
            Console.WriteLine("User count: " + users.Count.ToString());
            Console.WriteLine("NoteType count: " + (await _context.NoteTypes.CountAsync()).ToString());
            if (users.Count == 0)
            {
                await Seed_Users();
            }
            var _incidents = await _context.Incidents.ToListAsync();
            Console.WriteLine("Incident count: " + _incidents.Count.ToString());
            if (_incidents.Count == 0)
            {
                // serverId, incidentNoteId (for ping)
                await Seed_Incidents(1, 1);
            }
            //
        }
        //
        private async Task Seed_Users()
        {
            //
            try
            {
                // Add all the predefined profiles using the predefined password
                ApplicationUser _user1 = new ApplicationUser { Email = TestData.User_Email, UserName = TestData.User_Name, PasswordHash = "AQAAAAEAACcQAAAAEB4oAR8WhJGi5QVXpuONJ4z69YqF/69GlCz4TtjbQVLA4ef69x0iDq5GLTzvqM2vwQ==", SecurityStamp = "VFV7PXFFMU4VZF57I3T7A6TXVF4VAY2M", ConcurrencyStamp = "24240e95-400c-434e-b498-16542c90de13", CompanyId = 1, FirstName = "Phillip", LastName = "Huhn", FullName = "Phillip Huhn", UserNicName = TestData.User_Name };
                IdentityResult _identityResults1 = await _userManager.CreateAsync(_user1, TestData.Password);
                if (_identityResults1.Succeeded)
                {
                    await _userManager.AddToRoleAsync(_user1, "admin");
                    // _context.UserRoles.Add(new ApplicationUserRole() { UserId = _user1.Id, RoleId = "adm" });
                    _context.UserServers.Add(new ApplicationUserServer() { Id = _user1.Id, ServerId = 1 });
                    _context.SaveChanges();
                }
                ApplicationUser _user2 = new ApplicationUser { Email = "author@any.net", UserName = "author", PasswordHash = "AQAAAAEAACcQAAAAEGG4L+8q4FXRLAhrLWuALpnyStwzaYOaVA6LjBUrHHqs3AreDKMm9DnRa3B4PM1DYg==", SecurityStamp = "LTCQ4W2NCVQRESG6ZWMC7IBMWDZNICK7", ConcurrencyStamp = "2dab2144-81e5-4b76-a09c-c3b6c37f0f3b", CompanyId = 1, FirstName = "Author", LastName = "Huhn", FullName = "Author Huhn", UserNicName = "Art" };
                IdentityResult _identityResults2 = await _userManager.CreateAsync(_user2, "ABCdefghijk@1234567890");
                if (_identityResults2.Succeeded)
                {
                    await _userManager.AddToRoleAsync(_user2, "user");
                    // _context.UserRoles.Add(new ApplicationUserRole() { UserId = _user2.Id, RoleId = "usr" });
                    _context.UserServers.Add(new ApplicationUserServer() { Id = _user2.Id, ServerId = 1 });
                    _context.SaveChanges();
                }
            }
            catch (Exception _ex)
            {
                foreach (ApplicationUser _u in _context.Users)
                {
                    Console.WriteLine(_u.Id + " " + _u.UserName);
                }
                Console.WriteLine(_ex.Message);
                Console.WriteLine(_ex.ToString());
                throw _ex;
            }
            //
        }
        //
        private async Task Seed_Incidents(int serverId, int pingNoteId)
        {
            //
            try
            {
                Incident _inc = new Incident()
                {
                    ServerId = serverId,
                    IPAddress = "94.41.53.106",
                    NIC_Id = "ripe.net",
                    NetworkName = "UBN Network",
                    AbuseEmailAddress = "abuse@AbuseEmailAddress.com",
                    ISPTicketNumber = "",
                    Mailed = false,
                    Closed = false,
                    Special = false,
                    Notes = "",
                    CreatedDate = DateTime.Now,
                };
                _context.Incidents.Add(_inc);
                IncidentNote _incNote = new IncidentNote()
                {
                    NoteTypeId = pingNoteId,
                    Note = "Pinging 94.41.53.106 with 32 bytes of data:",
                    CreatedDate = DateTime.Now
                };
                _context.IncidentNotes.Add(_incNote);
                await _context.SaveChangesAsync(CancellationToken.None);
                //
                _context.IncidentIncidentNotes.Add(new IncidentIncidentNote() {
                    IncidentId = _inc.IncidentId, IncidentNoteId = _incNote.IncidentNoteId });
                // NetworkLog
                DateTime _dt = DateTime.Now.AddDays(-1);
                int _incTypeSql = 3;
                _context.NetworkLogs.AddRange(
                    new NetworkLog() { ServerId = serverId, IncidentId = _inc.IncidentId, IPAddress = "94.41.53.106", NetworkLogDate = _dt.AddMilliseconds(15), Log = "Fake log 1, Fake log 1, Fake log 1", IncidentTypeId = _incTypeSql },
                    new NetworkLog() { ServerId = serverId, IncidentId = _inc.IncidentId, IPAddress = "94.41.53.106", NetworkLogDate = _dt.AddMilliseconds(15), Log = "Fake log 1, Fake log 1, Fake log 1", IncidentTypeId = _incTypeSql }
                    //new NetworkLog() { ServerId = serverId, IncidentId = null, IPAddress = "104.42.229.49", NetworkLogDate = _dt.AddMinutes(4), Log = "Fake log 2, Fake log 2, Fake log 2", IncidentTypeId = _incTypeSql },
                    //new NetworkLog() { ServerId = serverId, IncidentId = null, IPAddress = "104.42.229.49", NetworkLogDate = _dt.AddMinutes(5), Log = "Fake log 3, Fake log 3, Fake log 3", IncidentTypeId = _incTypeSql },
                    //new NetworkLog() { ServerId = serverId, IncidentId = null, IPAddress = "54.183.209.144", NetworkLogDate = _dt.AddMinutes(10), Log = "Fake log 4, Fake log 4, Fake log 4", IncidentTypeId = _incTypeSql }
                );
                //
                await _context.SaveChangesAsync(CancellationToken.None);
                //
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex.Message);
                Console.WriteLine(_ex.ToString());
                throw _ex;
            }
            //
        }
    }
}
