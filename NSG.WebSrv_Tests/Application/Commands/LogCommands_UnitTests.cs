using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
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
        [TestMethod]
        public async Task LogCreateCommand_Simple01_TestAsync()
        {
            string _message = "Message";
            MethodBase _method = MethodBase.GetCurrentMethod();
            string _expectedMethod = "NSG.WebSrv_Tests.Application.Commands.LogCommands_UnitTests";
            _mockApplication.Setup(x => x.Now()).Returns(DateTime.Now);
            _mockApplication.Setup(x => x.GetApplicationName()).Returns("The Application!");
            _mockApplication.Setup(x => x.GetUserAccount()).Returns("Phil");
            LogCreateCommandHandler _handler = new LogCreateCommandHandler(db_context, _mockApplication.Object);
            LogCreateCommand _create = new LogCreateCommand(
                LoggingLevel.Warning, _method, _message, null);
            LogData _entity = await _handler.Handle(_create, CancellationToken.None);
            Console.WriteLine(_entity.LogToString());
            Assert.AreEqual(3, _entity.Id);
            Assert.AreEqual((byte)2, _entity.LogLevel);
            Assert.AreEqual("Warning", _entity.Level);
            Assert.AreEqual(_expectedMethod, _entity.Method.Substring(0, _expectedMethod.Length));
            Assert.AreEqual(_message, _entity.Message);
            Assert.AreEqual("", _entity.Exception);
        }
        //
        [TestMethod]
        public async Task LogCreateCommand_Simple02_Test()
        {
            string _message = "Message";
            string _method = "MethodBase";
            byte _severity = 2;
            _mockApplication.Setup(x => x.Now()).Returns(DateTime.Now);
            _mockApplication.Setup(x => x.GetApplicationName()).Returns("The Application!");
            _mockApplication.Setup(x => x.GetUserAccount()).Returns("Phil");
            LogCreateCommandHandler _handler = new LogCreateCommandHandler(db_context, _mockApplication.Object);
            LogCreateCommand _create = new LogCreateCommand(
                _severity, _method, _message, null);
            LogData _entity = await _handler.Handle(_create, CancellationToken.None);
            Console.WriteLine(_entity.LogToString());
            Assert.AreEqual(3, _entity.Id);
            Assert.AreEqual(_severity, _entity.LogLevel);
            Assert.AreEqual("Warning", _entity.Level);
            Assert.AreEqual(_method, _entity.Method);
            Assert.AreEqual(_message, _entity.Message);
            Assert.AreEqual("", _entity.Exception);
        }
        //
        [TestMethod]
        public async Task LogCreateCommand_Error01_TestAsync()
        {
            string _message = "Message";
            MethodBase _method = MethodBase.GetCurrentMethod();
            Exception _exception = new Exception("Test exception");
            string _expectedMethod = "NSG.WebSrv_Tests.Application.Commands.LogCommands_UnitTests";
            _mockApplication.Setup(x => x.Now()).Returns(DateTime.Now);
            _mockApplication.Setup(x => x.GetApplicationName()).Returns("The Application!");
            _mockApplication.Setup(x => x.GetUserAccount()).Returns("Phil");
            LogCreateCommandHandler _handler = new LogCreateCommandHandler(db_context, _mockApplication.Object);
            LogCreateCommand _create = new LogCreateCommand(
                LoggingLevel.Error, _method, _message, _exception);
            LogData _entity = await _handler.Handle(_create, CancellationToken.None);
            Console.WriteLine(_entity.LogToString());
            Assert.AreEqual(3, _entity.Id);
            Assert.AreEqual((byte)1, _entity.LogLevel);
            Assert.AreEqual("Error", _entity.Level);
            Assert.AreEqual(_expectedMethod, _entity.Method.Substring(0, _expectedMethod.Length));
            Assert.AreEqual(_message, _entity.Message);
            Assert.AreEqual("System.Exception: Test exception", _entity.Exception);
        }
        //
        [TestMethod]
        public async Task LogCreateCommand_Error02_Test()
        {
            string _message = "Message";
            string _method = "MethodBase";
            byte _severity = 1;
            string _exception = "System.Exception: Test exception";
            _mockApplication.Setup(x => x.Now()).Returns(DateTime.Now);
            _mockApplication.Setup(x => x.GetApplicationName()).Returns("The Application!");
            _mockApplication.Setup(x => x.GetUserAccount()).Returns("Phil");
            LogCreateCommandHandler _handler = new LogCreateCommandHandler(db_context, _mockApplication.Object);
            LogCreateCommand _create = new LogCreateCommand(
                _severity, _method, _message, _exception);
            LogData _entity = await _handler.Handle(_create, CancellationToken.None);
            Console.WriteLine(_entity.LogToString());
            Assert.AreEqual(3, _entity.Id);
            Assert.AreEqual(_severity, _entity.LogLevel);
            Assert.AreEqual("Error", _entity.Level);
            Assert.AreEqual(_method, _entity.Method);
            Assert.AreEqual(_message, _entity.Message);
            Assert.AreEqual(_exception, _entity.Exception);
        }
        //
        [TestMethod]
        public async Task LogCreateCommand_Audit_Test()
        {
            string _message = "Message";
            string _method = "MethodBase";
            byte _severity = 0;
            string _exception = "System.Exception: Test exception";
            _mockApplication.Setup(x => x.Now()).Returns(DateTime.Now);
            _mockApplication.Setup(x => x.GetApplicationName()).Returns("The Application!");
            _mockApplication.Setup(x => x.GetUserAccount()).Returns("Phil");
            LogCreateCommandHandler _handler = new LogCreateCommandHandler(db_context, _mockApplication.Object);
            LogCreateCommand _create = new LogCreateCommand(
                _severity, _method, _message, _exception);
            LogData _entity = await _handler.Handle(_create, CancellationToken.None);
            Console.WriteLine(_entity.LogToString());
            Assert.AreEqual(3, _entity.Id);
            Assert.AreEqual(_severity, _entity.LogLevel);
            Assert.AreEqual("Audit", _entity.Level);
            Assert.AreEqual(_method, _entity.Method);
            Assert.AreEqual(_message, _entity.Message);
            Assert.AreEqual(_exception, _entity.Exception);
        }
        //
        [TestMethod]
        public async Task LogCreateCommand_IncorrectSeverity_Test()
        {
            string _message = "Message";
            string _method = "MethodBase";
            byte _severity = 9;
            string _exception = "System.Exception: Test exception";
            _mockApplication.Setup(x => x.Now()).Returns(DateTime.Now);
            _mockApplication.Setup(x => x.GetApplicationName()).Returns("The Application!");
            _mockApplication.Setup(x => x.GetUserAccount()).Returns("Phil");
            LogCreateCommandHandler _handler = new LogCreateCommandHandler(db_context, _mockApplication.Object);
            LogCreateCommand _create = new LogCreateCommand(
                _severity, _method, _message, _exception);
            LogData _entity = await _handler.Handle(_create, CancellationToken.None);
            Console.WriteLine(_entity.LogToString());
            Assert.AreEqual(3, _entity.Id);
            Assert.AreEqual(_severity, _entity.LogLevel);
            Assert.AreEqual("Level-9", _entity.Level);
            Assert.AreEqual(_method, _entity.Method);
            Assert.AreEqual(_message, _entity.Message);
            Assert.AreEqual(_exception, _entity.Exception);
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
