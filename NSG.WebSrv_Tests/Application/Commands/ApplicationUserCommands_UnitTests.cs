using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
//
using Moq;
using MediatR;
//
using NSG.Integration.Helpers;
using NSG.WebSrv_Tests.Helpers;
using NSG.WebSrv.Domain.Entities;
using NSG.WebSrv.Application.Commands.ApplicationUsers;
using NSG.WebSrv.Application.Infrastructure;
using System.Linq;
//
namespace NSG.WebSrv_Tests.Application.Commands
{
    // NSG.WebSrv_Tests.Helpers.UnitTestFixture
    [TestClass]
    public class ApplicationUserCommands_UnitTests : UnitTestFixture
    {
        //
        static Mock<IMediator> _mockGetCompaniesMediator = null;
        static List<int> _companyList = null;
        static CancellationToken _cancelToken = CancellationToken.None; 
        string _testName;
        //
        public ApplicationUserCommands_UnitTests()
        {
            Console.WriteLine("ApplicationUserCommands_UnitTests");
        }
        //
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Console.WriteLine("ClassInitialize");
        }
        //
        [TestInitialize]
        public void TestInitialize()
        {
            Console.WriteLine("TestInitialize");
            //
            UnitTestSetup();
            //
            _mockGetCompaniesMediator = new Mock<IMediator>();
            // GetUserCompanyListQueryHandler.ViewModel _companiesViewModel =
            //     await Mediator.Send(new GetUserCompanyListQueryHandler.ListQuery());
            // set up mock to get list of permissible list of companies
            //         mediator.Setup(m => m.Send(It.IsAny<TransferNotificationCommand>(), default(CancellationToken))).Verifiable("Notification was not sent.");
            _companyList = new List<int>() { 1 };
            GetUserCompanyListQueryHandler.ViewModel _retViewModel =
                new GetUserCompanyListQueryHandler.ViewModel() { CompanyList = _companyList };
            _mockGetCompaniesMediator.Setup(x => x.Send(
                It.IsAny<GetUserCompanyListQueryHandler.ListQuery>(), _cancelToken))
                .Returns(Task.FromResult(_retViewModel));
            //
            DatabaseSeeder _seeder = new DatabaseSeeder(db_context, userManager, roleManager);
            _seeder.Seed().Wait();
            foreach( ApplicationUser _u in db_context.Users)
            {
                Console.Write(_u.UserName + ", ");
            }
            Console.WriteLine("");
            foreach (Company _c in db_context.Companies)
            {
                Console.Write(_c.CompanyId.ToString() + " "+ _c.CompanyShortName + ", ");
            }
            Console.WriteLine("");
        }
        //
        // You will need to check that the indexes work with you test data.
        //
        [TestMethod]
        public void ApplicationUserCreateCommand_Test()
        {
            _testName = "ApplicationUserCreateCommand_Test";
            Console.WriteLine($"{_testName} ...");
            ApplicationUserCreateCommandHandler _handler = new ApplicationUserCreateCommandHandler(db_context);
            ApplicationUserCreateCommand _create = new ApplicationUserCreateCommand()
            {
                Id = "Id",
                UserName = "UserName",
                NormalizedUserName = "NormalizedUserName",
                Email = "Email",
                NormalizedEmail = "NormalizedEmail",
                EmailConfirmed = false,
                PasswordHash = "PasswordHash",
                SecurityStamp = "SecurityStamp",
                ConcurrencyStamp = "ConcurrencyStamp",
                PhoneNumber = "PhoneNumber",
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnd = DateTime.Now,
                LockoutEnabled = false,
                AccessFailedCount = 1,
                CompanyId = 1,
                CreateDate = DateTime.Now,
                FirstName = "FirstName",
                FullName = "FullName",
                LastName = "LastName",
                UserNicName = "UserNicName",
            };
            Task<ApplicationUser> _createResults = _handler.Handle(_create, CancellationToken.None);
            ApplicationUser _entity = _createResults.Result;
            Assert.AreEqual("Id", _entity.Id);
        }
        //
        [TestMethod]
        public void ApplicationUserUpdateCommand_Test()
        {
            _testName = "ApplicationUserUpdateCommand_Test";
            Console.WriteLine($"{_testName} ...");
            ApplicationUserUpdateCommandHandler _handler = new ApplicationUserUpdateCommandHandler(db_context, userManager, _mockGetCompaniesMediator.Object);
            ApplicationUser _author = db_context.Users.Where(_u => _u.UserName == "author").FirstOrDefault();
            ApplicationUserUpdateCommand _update = new ApplicationUserUpdateCommand()
            {
                Id = _author.Id,
                UserName = _author.UserName,
                Email = _author.Email,
                PhoneNumber = _author.PhoneNumber,
                CompanyId = _author.CompanyId,
                FirstName = "FirstName",
                FullName = "FullName",
                LastName = "LastName",
                UserNicName = "UserNicName",
                //
                SelectedRoles = new string[] { "usr" },
                SelectedServers = new int[] { 1 },
                //
            };
            Task<int> _updateResults = _handler.Handle(_update, CancellationToken.None);
            int _count = _updateResults.Result;
            Assert.AreEqual(1, _count);
        }
        //
        [TestMethod]
        public void ApplicationUserDeleteCommand_Test()
        {
            _testName = "ApplicationUserDeleteCommand_Test";
            Console.WriteLine($"{_testName} ...");
            // Add a row to be deleted.
            ApplicationUser _create = new ApplicationUser()
            {
                Id = "Id",
                UserName = "UserName",
                NormalizedUserName = "NormalizedUserName",
                Email = "Email",
                NormalizedEmail = "NormalizedEmail",
                EmailConfirmed = false,
                PasswordHash = "PasswordHash",
                SecurityStamp = "SecurityStamp",
                ConcurrencyStamp = "ConcurrencyStamp",
                PhoneNumber = "PhoneNumber",
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnd = DateTime.Now,
                LockoutEnabled = false,
                AccessFailedCount = 1,
                CompanyId = 1,
                CreateDate = DateTime.Now,
                FirstName = "FirstName",
                FullName = "FullName",
                LastName = "LastName",
                UserNicName = "UserNicName",
            };
            db_context.Users.Add(_create);
            db_context.SaveChanges();
            //
            // Now delete what was just created ...
            ApplicationUserDeleteCommandHandler _handler = new ApplicationUserDeleteCommandHandler(db_context, userManager, _mockGetCompaniesMediator.Object);
            ApplicationUserDeleteCommand _delete = new ApplicationUserDeleteCommand()
            {
                UserName = _create.UserName,
            };
            Task<int> _deleteResults = _handler.Handle(_delete, CancellationToken.None);
            int _count = _deleteResults.Result;
            Assert.AreEqual(1, _count);
        }
        //
        [TestMethod]
        public async Task ApplicationUserDetailQuery_Test()
        {
            _testName = "ApplicationUserDetailQuery_Test";
            Console.WriteLine($"{_testName} ...");
            ApplicationUserDetailQueryHandler.DetailQuery _detailQuery =
                new ApplicationUserDetailQueryHandler.DetailQuery() { UserName = "Phil" };
            ApplicationUserDetailQueryHandler _handler = new ApplicationUserDetailQueryHandler(userManager, _mockGetCompaniesMediator.Object);
            ApplicationUserDetailQuery _detail =
                await _handler.Handle(_detailQuery, _cancelToken);
            Assert.AreEqual(_detailQuery.UserName, _detail.UserName);
        }
        //
        [TestMethod]
        public void ApplicationUserListQuery_Test()
        {
            _testName = "ApplicationUserListQuery_Test";
            Console.WriteLine($"{_testName} ...");
            ApplicationUserListQueryHandler _handler = new ApplicationUserListQueryHandler(userManager, _mockGetCompaniesMediator.Object);
            ApplicationUserListQueryHandler.ListQuery _listQuery =
                new ApplicationUserListQueryHandler.ListQuery();
            Task<ApplicationUserListQueryHandler.ViewModel> _viewModelResults =
                _handler.Handle(_listQuery, CancellationToken.None);
            IList<ApplicationUserListQuery> _list = _viewModelResults.Result.ApplicationUsersList;
            Assert.AreEqual(2, _list.Count);
        }
        //
    }
    //
}
