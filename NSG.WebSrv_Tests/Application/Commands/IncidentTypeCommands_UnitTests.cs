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
using NSG.WebSrv.Application.Commands.IncidentTypes;
using NSG.Integration.Helpers;
using Microsoft.Extensions.Options;
//
namespace NSG.WebSrv_Tests.Application.Commands
{
    [TestClass]
    public class IncidentTypeCommands_UnitTests : UnitTestFixture
    {
        //
        public IncidentTypeCommands_UnitTests()
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
        public void IncidentTypeCreateCommand_Test()
        {
            IncidentTypeCreateCommandHandler _handler = new IncidentTypeCreateCommandHandler(db_context);
            IncidentTypeCreateCommand _create = new IncidentTypeCreateCommand()
            {
                IncidentTypeShortDesc = "Incident",
                IncidentTypeDesc = "IncidentTypeDesc",
                IncidentTypeFromServer = false,
                IncidentTypeSubjectLine = "IncidentTypeSubjectLine",
                IncidentTypeEmailTemplate = "IncidentTypeEmailTemplate",
                IncidentTypeTimeTemplate = "IncidentTypeTimeTemplate",
                IncidentTypeThanksTemplate = "IncidentTypeThanksTemplate",
                IncidentTypeLogTemplate = "IncidentTypeLogTemplate",
                IncidentTypeTemplate = "IncidentTypeTemplate",
            };
            Task<IncidentType> _createResults = _handler.Handle(_create, CancellationToken.None);
            IncidentType _entity = _createResults.Result;
            Assert.AreEqual(9, _entity.IncidentTypeId);
        }
        //
        [TestMethod]
        public void IncidentTypeUpdateCommand_Test()
        {
            IncidentTypeUpdateCommandHandler _handler = new IncidentTypeUpdateCommandHandler(db_context);
            IncidentTypeUpdateCommand _update = new IncidentTypeUpdateCommand()
            {
                IncidentTypeId = 1,
                IncidentTypeShortDesc = "Incident",
                IncidentTypeDesc = "IncidentTypeDesc",
                IncidentTypeFromServer = false,
                IncidentTypeSubjectLine = "IncidentTypeSubjectLine",
                IncidentTypeEmailTemplate = "IncidentTypeEmailTemplate",
                IncidentTypeTimeTemplate = "IncidentTypeTimeTemplate",
                IncidentTypeThanksTemplate = "IncidentTypeThanksTemplate",
                IncidentTypeLogTemplate = "IncidentTypeLogTemplate",
                IncidentTypeTemplate = "IncidentTypeTemplate",
            };
            Task<int> _updateResults = _handler.Handle(_update, CancellationToken.None);
            int _count = _updateResults.Result;
            Assert.AreEqual(1, _count);
        }
        //
        [TestMethod]
        public void IncidentTypeDeleteCommand_Test()
        {
            // Add a row to be deleted.
            IncidentType _create = new IncidentType()
            {
                IncidentTypeShortDesc = "Incident",
                IncidentTypeDesc = "IncidentTypeDesc",
                IncidentTypeFromServer = false,
                IncidentTypeSubjectLine = "IncidentTypeSubjectLine",
                IncidentTypeEmailTemplate = "IncidentTypeEmailTemplate",
                IncidentTypeTimeTemplate = "IncidentTypeTimeTemplate",
                IncidentTypeThanksTemplate = "IncidentTypeThanksTemplate",
                IncidentTypeLogTemplate = "IncidentTypeLogTemplate",
                IncidentTypeTemplate = "IncidentTypeTemplate",
            };
            db_context.IncidentTypes.Add(_create);
            db_context.SaveChanges();
            //
            // IMediator mediator
            Mock<IMediator> _mockMediator = new Mock<IMediator>();
            // Now delete what was just created ...
            IncidentTypeDeleteCommandHandler _handler = new IncidentTypeDeleteCommandHandler(db_context, _mockMediator.Object);
            IncidentTypeDeleteCommand _delete = new IncidentTypeDeleteCommand()
            {
                IncidentTypeId = _create.IncidentTypeId,
            };
            Task<int> _deleteResults = _handler.Handle(_delete, CancellationToken.None);
            int _count = _deleteResults.Result;
            Assert.AreEqual(1, _count);
        }
        //
        [TestMethod]
        public async Task IncidentTypeDetailQuery_Test()
        {
            IncidentTypeDetailQueryHandler _handler = new IncidentTypeDetailQueryHandler(db_context);
            IncidentTypeDetailQueryHandler.DetailQuery _detailQuery =
                new IncidentTypeDetailQueryHandler.DetailQuery();
            _detailQuery.IncidentTypeId = 1;
            IncidentTypeDetailQuery _detail =
                await _handler.Handle(_detailQuery, CancellationToken.None);
            Assert.AreEqual(1, _detail.IncidentTypeId);
        }
        //
        [TestMethod]
        public void IncidentTypeListQuery_Test()
        {
            IncidentTypeListQueryHandler _handler = new IncidentTypeListQueryHandler(db_context);
            IncidentTypeListQueryHandler.ListQuery _listQuery =
                new IncidentTypeListQueryHandler.ListQuery();
            Task<IncidentTypeListQueryHandler.ViewModel> _viewModelResults =
                _handler.Handle(_listQuery, CancellationToken.None);
            IList<IncidentTypeListQuery> _list = _viewModelResults.Result.IncidentTypesList;
            Assert.AreEqual(8, _list.Count);
        }
        //
    }
}
