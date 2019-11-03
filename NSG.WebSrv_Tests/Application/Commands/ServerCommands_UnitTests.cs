using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Moq;
using MediatR;
//
using NSG.WebSrv.Domain.Entities;
using NSG.WebSrv_Tests.Helpers;
using NSG.WebSrv.Infrastructure.Notification;
using NSG.WebSrv.Application.Commands.Servers;
using NSG.Integration.Helpers;
using Microsoft.Extensions.Options;
using NSG.WebSrv.Application.Infrastructure;
//
namespace NSG.WebSrv_Tests.Application.Commands
{
    [TestClass]
    public class ServerCommands_UnitTests : UnitTestFixture
    {
        //
        static Mock<IMediator> _mockGetCompaniesMediator = null;
        static CancellationToken _cancelToken = CancellationToken.None;
        //
        public ServerCommands_UnitTests()
        {
            //
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
            // set up mock to get list of permissible list of companies
            GetUserCompanyListQueryHandler.ViewModel _retViewModel =
                new GetUserCompanyListQueryHandler.ViewModel() { CompanyList = new List<int>() { 1 } };
            _mockGetCompaniesMediator = new Mock<IMediator>();
            _mockGetCompaniesMediator.Setup(x => x.Send(
                It.IsAny<GetUserCompanyListQueryHandler.ListQuery>(), _cancelToken))
                .Returns(Task.FromResult(_retViewModel));
            //
            DatabaseSeeder _seeder = new DatabaseSeeder(db_context, userManager, roleManager);
            _seeder.Seed().Wait();
        }
        //
        [TestMethod]
        public void ServerCreateCommand_Test()
        {
            ServerCreateCommandHandler _handler = new ServerCreateCommandHandler(db_context, _mockGetCompaniesMediator.Object);
            ServerCreateCommand _create = new ServerCreateCommand()
            {
                CompanyId = 1,
                ServerShortName = "ServerShortN",
                ServerName = "ServerName",
                ServerDescription = "ServerDescription",
                WebSite = "WebSite",
                ServerLocation = "ServerLocation",
                FromName = "FromName",
                FromNicName = "FromNicName",
                FromEmailAddress = "FromEmailAddress",
                TimeZone = "TimeZone",
                DST = false,
                TimeZone_DST = "TimeZone_DST",
                DST_Start = DateTime.Now,
                DST_End = DateTime.Now
            };
            Task<Server> _createResults = _handler.Handle(_create, CancellationToken.None);
            Server _entity = _createResults.Result;
            Assert.AreEqual(2, _entity.ServerId);
        }
        //
        [TestMethod]
        public void ServerUpdateCommand_Test()
        {
            ServerUpdateCommandHandler _handler = new ServerUpdateCommandHandler(db_context, _mockGetCompaniesMediator.Object);
            ServerUpdateCommand _update = new ServerUpdateCommand()
            {
                ServerId = 1,
                CompanyId = 1,
                ServerShortName = "ServerShortN",
                ServerName = "ServerName",
                ServerDescription = "ServerDescription",
                WebSite = "WebSite",
                ServerLocation = "ServerLocation",
                FromName = "FromName",
                FromNicName = "FromNicName",
                FromEmailAddress = "FromEmailAddress",
                TimeZone = "TimeZone",
                DST = false,
                TimeZone_DST = "TimeZone_DST",
                DST_Start = new DateTime(2019, 3, 10, 2, 0, 0),
                DST_End = new DateTime(2019, 11, 3, 2, 0, 0)
            };
            Task<int> _updateResults = _handler.Handle(_update, CancellationToken.None);
            int _count = _updateResults.Result;
            Assert.AreEqual(1, _count);
        }
        //
        [TestMethod]
        public void ServerDeleteCommand_Test()
        {
            // Add a row to be deleted.
            Server _create = new Server()
            {
                CompanyId = 1,
                ServerShortName = "ServerShortN",
                ServerName = "ServerName",
                ServerDescription = "ServerDescription",
                WebSite = "WebSite",
                ServerLocation = "ServerLocation",
                FromName = "FromName",
                FromNicName = "FromNicName",
                FromEmailAddress = "FromEmailAddress",
                TimeZone = "TimeZone",
                DST = false,
                TimeZone_DST = "TimeZone_DST",
                DST_Start = DateTime.Now,
                DST_End = DateTime.Now,
            };
            db_context.Servers.Add(_create);
            db_context.SaveChanges();
            //
            // Now delete what was just created ...
            ServerDeleteCommandHandler _handler = new ServerDeleteCommandHandler(db_context, _mockGetCompaniesMediator.Object);
            ServerDeleteCommand _delete = new ServerDeleteCommand()
            {
                ServerId = _create.ServerId,
            };
            Task<int> _deleteResults = _handler.Handle(_delete, CancellationToken.None);
            int _count = _deleteResults.Result;
            Assert.AreEqual(1, _count);
        }
        //
    }
}
