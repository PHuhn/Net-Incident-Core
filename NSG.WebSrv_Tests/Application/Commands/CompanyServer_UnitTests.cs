using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using MediatR;
//
using NSG.WebSrv.UI.Controllers;
using NSG.WebSrv.Domain.Entities;
using NSG.WebSrv_Tests.Helpers;
using NSG.WebSrv.Infrastructure.Notification;
using NSG.WebSrv.Application.Commands.CompanyServers;
using NSG.Integration.Helpers;
using NSG.WebSrv.Application.Infrastructure;
//
namespace NSG.WebSrv_Tests.Application.Commands
{
    [TestClass]
    public class CompanyServer_UnitTests : UnitTestFixture
    {
        //
        Mock<IMediator> _mockGetCompaniesMediator = null;
        static CancellationToken _cancelToken = CancellationToken.None;
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
        public void CompanyServerDetailQuery_Test()
        {
            CompanyServerDetailQueryHandler _handler = new CompanyServerDetailQueryHandler(db_context, _mockGetCompaniesMediator.Object);
            CompanyServerDetailQueryHandler.DetailQuery _detailQuery =
                new CompanyServerDetailQueryHandler.DetailQuery() { CompanyId = 1 };
            _detailQuery.CompanyId = 1;
            Task<CompanyServerDetailQuery> _detailResults =
                _handler.Handle(_detailQuery, CancellationToken.None);
            CompanyServerDetailQuery _detail = _detailResults.Result;
            Assert.AreEqual(1, _detail.CompanyId);
        }
        //
        [TestMethod]
        public void CompanyServerListQuery_Test()
        {
            CompanyServerListQueryHandler _handler = new CompanyServerListQueryHandler(db_context, _mockGetCompaniesMediator.Object);
            CompanyServerListQueryHandler.ListQuery _listQuery =
                new CompanyServerListQueryHandler.ListQuery();
            Task<CompanyServerListQueryHandler.ViewModel> _viewModelResults =
                _handler.Handle(_listQuery, CancellationToken.None);
            IList<CompanyServerListQuery> _list = _viewModelResults.Result.CompaniesList;
            Assert.IsTrue(_list.Count == 1 || _list.Count == 2);
        }
        //
    }
}
