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
using NSG.WebSrv.Application.Commands.NoteTypes;
using NSG.Integration.Helpers;
using Microsoft.Extensions.Options;
//
namespace NSG.WebSrv_Tests.Application.Commands
{
    [TestClass]
    public class NoteTypeCommands_UnitTests : UnitTestFixture
    {
        //
        public NoteTypeCommands_UnitTests()
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
            foreach( NoteType _nt in db_context.NoteTypes)
            {
                Console.WriteLine(_nt.NoteTypeId.ToString() + " " + _nt.NoteTypeShortDesc);
            }
        }
        //
        [TestMethod]
        public void NoteTypeCreateCommand_Test()
        {
            NoteTypeCreateCommandHandler _handler = new NoteTypeCreateCommandHandler(db_context);
            NoteTypeCreateCommand _create = new NoteTypeCreateCommand()
            {
                NoteTypeId = 6,
                NoteTypeShortDesc = "NoteType",
                NoteTypeDesc = "NoteTypeDesc",
                NoteTypeClientScript = ""
            };
            var nt = db_context.NoteTypes.ToListAsync();
            Task<NoteType> _createResults = _handler.Handle(_create, CancellationToken.None);
            NoteType _entity = _createResults.Result;
            Assert.AreEqual(6, _entity.NoteTypeId);
        }
        //
        [TestMethod]
        public void NoteTypeUpdateCommand_Test()
        {
            NoteTypeUpdateCommandHandler _handler = new NoteTypeUpdateCommandHandler(db_context);
            NoteTypeUpdateCommand _update = new NoteTypeUpdateCommand()
            {
                NoteTypeId = 5,
                NoteTypeShortDesc = "NoteType",
                NoteTypeDesc = "NoteTypeDesc",
                NoteTypeClientScript = ""
            };
            Task<int> _updateResults = _handler.Handle(_update, CancellationToken.None);
            int _count = _updateResults.Result;
            Assert.AreEqual(1, _count);
        }
        //
        [TestMethod]
        public void NoteTypeDeleteCommand_Test()
        {
            // Add a row to be deleted.
            NoteType _create = new NoteType()
            {
                NoteTypeShortDesc = "NoteType",
                NoteTypeDesc = "NoteTypeDesc",
            };
            db_context.NoteTypes.Add(_create);
            db_context.SaveChanges();
            //
            // IMediator mediator
            Mock<IMediator> _mockMediator = new Mock<IMediator>();
            // Now delete what was just created ...
            NoteTypeDeleteCommandHandler _handler = new NoteTypeDeleteCommandHandler(db_context, _mockMediator.Object);
            NoteTypeDeleteCommand _delete = new NoteTypeDeleteCommand()
            {
                NoteTypeId = _create.NoteTypeId,
            };
            Task<int> _deleteResults = _handler.Handle(_delete, CancellationToken.None);
            int _count = _deleteResults.Result;
            Assert.AreEqual(1, _count);
        }
        //
        [TestMethod]
        public void NoteTypeDetailQuery_Test()
        {
            NoteTypeDetailQueryHandler _handler = new NoteTypeDetailQueryHandler(db_context);
            NoteTypeDetailQueryHandler.DetailQuery _detailQuery =
                new NoteTypeDetailQueryHandler.DetailQuery();
            _detailQuery.NoteTypeId = 1;
            Task<NoteTypeDetailQuery> _detailResults =
                _handler.Handle(_detailQuery, CancellationToken.None);
            NoteTypeDetailQuery _detail = _detailResults.Result;
            Assert.AreEqual(1, _detail.NoteTypeId);
        }
        //
        [TestMethod]
        public void NoteTypeListQuery_Test()
        {
            NoteTypeListQueryHandler _handler = new NoteTypeListQueryHandler(db_context);
            NoteTypeListQueryHandler.ListQuery _listQuery =
                new NoteTypeListQueryHandler.ListQuery();
            Task<NoteTypeListQueryHandler.ViewModel> _viewModelResults =
                _handler.Handle(_listQuery, CancellationToken.None);
            IList<NoteTypeListQuery> _list = _viewModelResults.Result.NoteTypesList;
            Assert.IsTrue(_list.Count > 4);
        }
        //
    }
}
