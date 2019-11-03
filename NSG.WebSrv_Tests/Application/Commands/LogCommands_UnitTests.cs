using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Moq;
using MediatR;
//
using NSG.WebSrv.Domain.Entities;
using NSG.WebSrv_Tests.Helpers;
using NSG.WebSrv.Application.Commands.Logs;
using NSG.Integration.Helpers;
using NSG.WebSrv.Infrastructure.Common;
//
namespace NSG.WebSrv_Tests.Application.Commands
{
    [TestClass]
    public class LogCommands_UnitTests : UnitTestFixture
    {
        //
        static Mock<IApplication> _mockApplication = null;
        static string _userName;
        //
        public LogCommands_UnitTests()
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
            //
            _mockApplication = new Mock<IApplication>();
            _userName = "Phil";
            //
            DatabaseSeeder _seeder = new DatabaseSeeder(db_context, userManager, roleManager);
            _seeder.Seed().Wait();
            LogData _log1 = new LogData
            {
                Date = DateTime.Now, Application = "This application", Method = "The method",
                LogLevel = (byte)LoggingLevel.Info, Level = Enum.GetName(LoggingLevel.Info.GetType(), LoggingLevel.Info),
                UserAccount = _userName, Message = "Information Message",
                Exception = ""
            };
            LogData _log2 = new LogData
            {
                Date = DateTime.Now, Application = "This application", Method = "The method",
                LogLevel = (byte)LoggingLevel.Error, Level = Enum.GetName(LoggingLevel.Error.GetType(), LoggingLevel.Info),
                UserAccount = _userName, Message = "Information Message",
                Exception = ""
            };
            db_context.Logs.Add(_log1);
            db_context.Logs.Add(_log2);
            db_context.SaveChanges();
        }
        //
        //
        [TestMethod]
        public void LogCreateCommand_Test()
        {
            _mockApplication.Setup(x => x.Now()).Returns(DateTime.Now);
            _mockApplication.Setup(x => x.GetApplicationName()).Returns("The Application!");
            _mockApplication.Setup(x => x.GetUserAccount()).Returns("Phil");
            LogCreateCommandHandler _handler = new LogCreateCommandHandler(db_context, _mockApplication.Object);
            LogCreateCommand _create = new LogCreateCommand()
            {
                Method = MethodBase.GetCurrentMethod(),
                Level = LoggingLevel.Warning,
                Message = "Message",
                Exception = null
            };
            Task<LogData> _createResults = _handler.Handle(_create, CancellationToken.None);
            LogData _entity = _createResults.Result;
            Assert.AreEqual(3, _entity.Id);
        }
        //
        [TestMethod]
        public void LogListQuery_Test()
        {
            LogListQueryHandler _handler = new LogListQueryHandler(db_context);
            LogListQueryHandler.ListQuery _listQuery =
                new LogListQueryHandler.ListQuery() { UserAccount = _userName };
            Task<LogListQueryHandler.ViewModel> _viewModelResults =
                _handler.Handle(_listQuery, CancellationToken.None);
            IList<LogListQuery> _list = _viewModelResults.Result.LogsList;
            Assert.AreEqual(2, _list.Count);
        }
        //
    }
}
