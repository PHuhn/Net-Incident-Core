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
using NSG.WebSrv.Application.Commands.ApplicationRoles;
using NSG.WebSrv.Application.Infrastructure;
using System.Linq;
//
namespace NSG.WebSrv_Tests.Application.Commands
{
    // NSG.WebSrv_Tests.Helpers.UnitTestFixture
    [TestClass]
    public class ApplicationRoleCommands_UnitTests : UnitTestFixture
    {
        //
        static Mock<IMediator> _mockMediator = null;
        static CancellationToken _cancelToken = CancellationToken.None; 
        string _testName;
        //
        public ApplicationRoleCommands_UnitTests()
        {
            Console.WriteLine("ApplicationRoleCommands_UnitTests");
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
            _mockMediator = new Mock<IMediator>();
            //
            DatabaseSeeder _seeder = new DatabaseSeeder(db_context, userManager, roleManager);
            _seeder.Seed().Wait();
            foreach( ApplicationRole _u in db_context.Roles)
            {
                Console.Write(_u.Name + ", ");
            }
            Console.WriteLine("");
        }
        //
        // You will need to check that the indexes work with you test data.
        //
        [TestMethod]
        public void ApplicationRoleCreateCommand_Test()
        {
            _testName = "ApplicationRoleCreateCommand_Test";
            Console.WriteLine($"{_testName} ...");
            ApplicationRoleCreateCommandHandler _handler = new ApplicationRoleCreateCommandHandler(roleManager, _mockMediator.Object);
            ApplicationRoleCreateCommand _create = new ApplicationRoleCreateCommand()
            {
                Id = "Id",
                Name = "Name",
            };
            Task<ApplicationRole> _createResults = _handler.Handle(_create, CancellationToken.None);
            ApplicationRole _entity = _createResults.Result;
            Assert.AreEqual("Id", _entity.Id);
        }
        //
        [TestMethod]
        public void ApplicationRoleUpdateCommand_Test()
        {
            _testName = "ApplicationRoleUpdateCommand_Test";
            Console.WriteLine($"{_testName} ...");
            ApplicationRoleUpdateCommandHandler _handler = new ApplicationRoleUpdateCommandHandler(roleManager, _mockMediator.Object);
            ApplicationRoleUpdateCommand _update = new ApplicationRoleUpdateCommand()
            {
                Id = "pub",
                Name = "Name"
            };
            Task<int> _updateResults = _handler.Handle(_update, CancellationToken.None);
            int _count = _updateResults.Result;
            Assert.AreEqual(1, _count);
        }
        //
        [TestMethod]
        public void ApplicationRoleDeleteCommand_Test()
        {
            _testName = "ApplicationRoleDeleteCommand_Test";
            Console.WriteLine($"{_testName} ...");
            // Add a row to be deleted.
            ApplicationRole _create = new ApplicationRole()
            {
                Id = "Id",
                Name = "Name"
            };
            db_context.Roles.Add(_create);
            db_context.SaveChanges();
            //
            // Now delete what was just created ...
            ApplicationRoleDeleteCommandHandler _handler = new ApplicationRoleDeleteCommandHandler(roleManager, _mockMediator.Object);
            ApplicationRoleDeleteCommand _delete = new ApplicationRoleDeleteCommand()
            {
                Id = _create.Id,
            };
            Task<int> _deleteResults = _handler.Handle(_delete, CancellationToken.None);
            int _count = _deleteResults.Result;
            Assert.AreEqual(1, _count);
        }
        //
        [TestMethod]
        public void ApplicationRoleDeleteCommand_ActiveUsersException_Test()
        {
            _testName = "ApplicationRoleDeleteCommand_ActiveUsersException_Test";
            Console.WriteLine($"{_testName} ...");
            //
            ApplicationRoleDeleteCommandHandler _handler = new ApplicationRoleDeleteCommandHandler(roleManager, _mockMediator.Object);
            ApplicationRoleDeleteCommand _delete = new ApplicationRoleDeleteCommand()
            {
                Id = "adm"
            };
            try
            {
                Task<int> _deleteResults = _handler.Handle(_delete, CancellationToken.None);
                Console.WriteLine(_deleteResults.Status);
                if (_deleteResults.Exception == null)
                {
                    Assert.Fail("should of thrown exception!");
                }
                else
                {
                    Console.WriteLine(_deleteResults.Exception.Message);
                    Console.WriteLine(_deleteResults.Exception.InnerException);
                    if (!(_deleteResults.Exception.InnerException is ApplicationRoleDeleteCommandActiveUsersException))
                    {
                        Assert.Fail("should of thrown ApplicationRoleDeleteCommandActiveUsersException!");
                    }
                }
            }
            catch (ApplicationRoleDeleteCommandActiveUsersException _ex)
            {
                Console.WriteLine(_ex.Message);
                return;
            }
            catch( Exception _e )
            {
                Console.WriteLine(_e.Message);
                Assert.Fail("should of thrown ApplicationRoleDeleteCommandActiveUsersException!");
            }
        }
        //
        [TestMethod]
        public async Task ApplicationRoleDetailQuery_Test()
        {
            _testName = "ApplicationRoleDetailQuery_Test";
            Console.WriteLine($"{_testName} ...");
            ApplicationRoleDetailQueryHandler _handler = new ApplicationRoleDetailQueryHandler(roleManager);
            ApplicationRoleDetailQueryHandler.DetailQuery _detailQuery =
                new ApplicationRoleDetailQueryHandler.DetailQuery();
            _detailQuery.Id = "pub";
            ApplicationRoleDetailQuery _detail =
                await _handler.Handle(_detailQuery, CancellationToken.None);
            Assert.AreEqual(_detailQuery.Id, _detail.Id);
        }
        //
        [TestMethod]
        public void ApplicationRoleListQuery_Test()
        {
            _testName = "ApplicationRoleListQuery_Test";
            Console.WriteLine($"{_testName} ...");
            ApplicationRoleListQueryHandler _handler = new ApplicationRoleListQueryHandler(roleManager);
            ApplicationRoleListQueryHandler.ListQuery _listQuery =
                new ApplicationRoleListQueryHandler.ListQuery();
            Task<ApplicationRoleListQueryHandler.ViewModel> _viewModelResults =
                _handler.Handle(_listQuery, CancellationToken.None);
            IList<ApplicationRoleListQuery> _list = _viewModelResults.Result.ApplicationRolesList;
            Assert.AreEqual(4, _list.Count);
        }
        //
    }
    //
}
