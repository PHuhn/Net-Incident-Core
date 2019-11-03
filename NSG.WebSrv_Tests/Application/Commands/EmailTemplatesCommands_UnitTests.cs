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
using NSG.WebSrv.Application.Commands.CompanyEmailTemplates;
using NSG.Integration.Helpers;
using Microsoft.Extensions.Options;
using NSG.WebSrv.Application.Infrastructure;
//
namespace NSG.WebSrv_Tests.Application.Commands
{
    [TestClass]
    public class EmailTemplatesCommands_UnitTests : UnitTestFixture
    {
        //
        static Mock<IMediator> _mockGetCompaniesMediator = null;
        static CancellationToken _cancelToken = CancellationToken.None;
        //
        public EmailTemplatesCommands_UnitTests()
        {
            Console.WriteLine("EmailTemplatesCommands_UnitTests, constructor");
        }
        //
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Console.WriteLine("EmailTemplatesCommands_UnitTests, ClassInitialize");
        }
        //
        [TestInitialize]
        public void TestInitialize()
        {
            Console.WriteLine("EmailTemplatesCommands_UnitTests, TestInitialize");
            //
            UnitTestSetup();
            //
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
        public void EmailTemplateCreateCommand_Test()
        {
            CompanyEmailTemplateCreateCommandHandler _handler = new CompanyEmailTemplateCreateCommandHandler(db_context);
            CompanyEmailTemplateCreateCommand _create = new CompanyEmailTemplateCreateCommand()
            {
                CompanyId = 1,
                IncidentTypeId = 1,
                SubjectLine = "SubjectLine",
                EmailBody = "EmailBody",
                TimeTemplate = "TimeTemplate",
                ThanksTemplate = "ThanksTemplate",
                LogTemplate = "LogTemplate",
                Template = "Template",
                FromServer = false,
            };
            Task<EmailTemplate> _createResults = _handler.Handle(_create, _cancelToken);
            EmailTemplate _entity = _createResults.Result;
            Assert.AreEqual(1, _entity.CompanyId);
            Assert.AreEqual(1, _entity.IncidentTypeId);
        }
        //
        [TestMethod]
        public void EmailTemplateUpdateCommand_Test()
        {
            CompanyEmailTemplateUpdateCommandHandler _handler = new CompanyEmailTemplateUpdateCommandHandler(db_context);
            CompanyEmailTemplateUpdateCommand _update = new CompanyEmailTemplateUpdateCommand()
            {
                CompanyId = 1,
                IncidentTypeId = 8,
                SubjectLine = "SubjectLine",
                EmailBody = "EmailBody",
                TimeTemplate = "TimeTemplate",
                ThanksTemplate = "ThanksTemplate",
                LogTemplate = "LogTemplate",
                Template = "Template",
                FromServer = false,
            };
            Task<int> _updateResults = _handler.Handle(_update, _cancelToken);
            int _count = _updateResults.Result;
            Assert.AreEqual(1, _count);
        }
        //
        [TestMethod]
        public void EmailTemplateDeleteCommand_Test()
        {
            // Add a row to be deleted.
            EmailTemplate _create = new EmailTemplate()
            {
                CompanyId = 1,
                IncidentTypeId = 1,
                SubjectLine = "SubjectLine",
                EmailBody = "EmailBody",
                TimeTemplate = "TimeTemplate",
                ThanksTemplate = "ThanksTemplate",
                LogTemplate = "LogTemplate",
                Template = "Template",
                FromServer = false,
            };
            db_context.EmailTemplates.Add(_create);
            db_context.SaveChanges();
            //
            // IMediator mediator
            Mock<IMediator> _mockMediator = new Mock<IMediator>();
            // Now delete what was just created ...
            CompanyEmailTemplateDeleteCommandHandler _handler = new CompanyEmailTemplateDeleteCommandHandler(db_context, _mockMediator.Object);
            CompanyEmailTemplateDeleteCommand _delete = new CompanyEmailTemplateDeleteCommand()
            {
                CompanyId = _create.CompanyId,
                IncidentTypeId = _create.IncidentTypeId,
            };
            Task<int> _deleteResults = _handler.Handle(_delete, _cancelToken);
            int _count = _deleteResults.Result;
            Assert.AreEqual(1, _count);
        }
        //
        [TestMethod]
        public async Task EmailTemplateDetailQuery_Test()
        {
            CompanyEmailTemplateDetailQueryHandler _handler = new CompanyEmailTemplateDetailQueryHandler(db_context);
            CompanyEmailTemplateDetailQueryHandler.DetailQuery _detailQuery =
                new CompanyEmailTemplateDetailQueryHandler.DetailQuery();
            _detailQuery.CompanyId = 1;
            _detailQuery.IncidentTypeId = 8;
            CompanyEmailTemplateDetailQuery _detail =
                await _handler.Handle(_detailQuery, _cancelToken);
            Assert.AreEqual(_detailQuery.CompanyId, _detail.CompanyId);
            Assert.AreEqual(_detailQuery.IncidentTypeId , _detail.IncidentTypeId);
        }
        //
        [TestMethod]
        public void EmailTemplateListQuery_Test()
        {
            CompanyEmailTemplateListQueryHandler _handler = new CompanyEmailTemplateListQueryHandler(db_context, _mockGetCompaniesMediator.Object);
            CompanyEmailTemplateListQueryHandler.ListQuery _listQuery =
                new CompanyEmailTemplateListQueryHandler.ListQuery() { CompanyId = 1 };
            Task<CompanyEmailTemplateListQueryHandler.ViewModel> _viewModelResults =
                _handler.Handle(_listQuery, _cancelToken);
            IList<CompanyEmailTemplateListQuery> _list = _viewModelResults.Result.EmailTemplatesList;
            Assert.AreEqual(1, _list.Count);
        }
        //
    }
}
