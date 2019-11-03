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
using NSG.WebSrv.Application.Commands.NICs;
using NSG.Integration.Helpers;
using Microsoft.Extensions.Options;
//
namespace NSG.WebSrv_Tests.Application.Commands
{
    [TestClass]
    public class NIC_Commands_UnitTests : UnitTestFixture
    {
        //
        public NIC_Commands_UnitTests()
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
            DatabaseSeeder _seeder = new DatabaseSeeder(db_context, userManager, roleManager);
            _seeder.Seed().Wait();
            foreach( NIC _n in db_context.NICs)
            {
                Console.Write(_n.NIC_Id + ", ");
            }
            Console.WriteLine("");
        }
        //
        [TestMethod]
        public void NICCreateCommand_Test()
        {
            NICCreateCommandHandler _handler = new NICCreateCommandHandler(db_context);
            NICCreateCommand _create = new NICCreateCommand()
            {
                NIC_Id = "NIC_Id",
                NICDescription = "NICDescription",
                NICAbuseEmailAddress = "NICAbuseEmailAddress",
                NICRestService = "NICRestService",
                NICWebSite = "NICWebSite",
            };
            Task<NIC> _createResults = _handler.Handle(_create, CancellationToken.None);
            NIC _entity = _createResults.Result;
            Assert.AreEqual("NIC_Id", _entity.NIC_Id);
        }
        //
        [TestMethod]
        public void NICUpdateCommand_Test()
        {
            NICUpdateCommandHandler _handler = new NICUpdateCommandHandler(db_context);
            NICUpdateCommand _update = new NICUpdateCommand()
            {
                NIC_Id = "other",
                NICDescription = "NICDescription",
                NICAbuseEmailAddress = "NICAbuseEmailAddress",
                NICRestService = "NICRestService",
                NICWebSite = "NICWebSite",
            };
            Task<int> _updateResults = _handler.Handle(_update, CancellationToken.None);
            int _count = _updateResults.Result;
            Assert.AreEqual(1, _count);
        }
        //
        [TestMethod]
        public void NICDeleteCommand_Test()
        {
            // Add a row to be deleted.
            NIC _create = new NIC()
            {
                NIC_Id = "DEL ME",
                NICDescription = "NICDescription",
                NICAbuseEmailAddress = "NICAbuseEmailAddress",
                NICRestService = "NICRestService",
                NICWebSite = "NICWebSite",
            };
            db_context.NICs.Add(_create);
            db_context.SaveChanges();
            //
            // IMediator mediator
            Mock<IMediator> _mockMediator = new Mock<IMediator>();
            // Now delete what was just created ...
            NICDeleteCommandHandler _handler = new NICDeleteCommandHandler(db_context, _mockMediator.Object);
            NICDeleteCommand _delete = new NICDeleteCommand()
            {
                NIC_Id = _create.NIC_Id,
            };
            Task<int> _deleteResults = _handler.Handle(_delete, CancellationToken.None);
            int _count = _deleteResults.Result;
            Assert.AreEqual(1, _count);
        }
        //
        [TestMethod]
        public async Task NICDetailQuery_Test()
        {
            NICDetailQueryHandler _handler = new NICDetailQueryHandler(db_context);
            NICDetailQueryHandler.DetailQuery _detailQuery =
                new NICDetailQueryHandler.DetailQuery();
            _detailQuery.NIC_Id = "unknown";
            NICDetailQuery _detail =
                await _handler.Handle(_detailQuery, CancellationToken.None);
            Assert.AreEqual("unknown", _detail.NIC_Id);
        }
        //
        [TestMethod]
        public void NICListQuery_Test()
        {
            NICListQueryHandler _handler = new NICListQueryHandler(db_context);
            NICListQueryHandler.ListQuery _listQuery =
                new NICListQueryHandler.ListQuery();
            Task<NICListQueryHandler.ViewModel> _viewModelResults =
                _handler.Handle(_listQuery, CancellationToken.None);
            IList<NICListQuery> _list = _viewModelResults.Result.NICsList;
            Assert.AreEqual(11, _list.Count);
        }
        //
    }
}
