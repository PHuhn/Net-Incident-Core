using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
//
using Moq;
using MediatR;
//
using NSG.WebSrv.Domain.Entities;
using NSG.WebSrv.Application.Commands.Companies;
using NSG.Integration.Helpers;
using NSG.WebSrv.Application.Infrastructure;
using NSG.WebSrv_Tests.Helpers;
//
namespace NSG.WebSrv_Tests.Application.Commands
{
    [TestClass]
    public class CompanyCommands_UnitTests : UnitTestFixture
    {
        //
        static Mock<IMediator> _mockGetCompaniesMediator = null;
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
        public void CompanyCreateCommand_Test()
        {
            Console.WriteLine("CompanyCreateCommand_Test");
            CompanyCreateCommandHandler _handler = new CompanyCreateCommandHandler(db_context);
            CompanyCreateCommand _create = new CompanyCreateCommand()
            {
                CompanyShortName = "CompanyShort",
                CompanyName = "CompanyName",
                Address = "Address",
                City = "City",
                State = "Stat",
                PostalCode = "PostalCode",
                Country = "Country",
                PhoneNumber = "PhoneNumber",
                Notes = "Notes",
            };
            Task<Company> _createResults = _handler.Handle(_create, CancellationToken.None);
            Company _entity = _createResults.Result;
            Assert.AreEqual(2, _entity.CompanyId);
        }
        //
        [TestMethod]
        public void CompanyCreateCommand_ToLong_Test()
        {
            Console.WriteLine("CompanyCreateCommand_ToLong_Test");
            CompanyCreateCommandHandler _handler = new CompanyCreateCommandHandler(db_context);
            CompanyCreateCommand _create = new CompanyCreateCommand()
            {
                CompanyShortName = "CompanyShort-tolong",
                CompanyName = "CompanyName",
                Address = "Address",
                City = "City",
                State = "Stat",
                PostalCode = "PostalCode",
                Country = "Country",
                PhoneNumber = "PhoneNumber",
                Notes = "Notes",
            };
            try
            {
                Task<Company> _createResults = _handler.Handle(_create, CancellationToken.None);
            }
            catch (CreateCommandValidationException _ex1)
            {
                Console.WriteLine(_ex1.Message);
            }
            catch (Exception _ex2)
            {
                Console.WriteLine(_ex2.Message);
                Assert.Fail();
            }
        }
        //
        [TestMethod]
        public void CompanyUpdateCommand_Test()
        {
            Console.WriteLine("CompanyUpdateCommand_Test");
            CompanyUpdateCommandHandler _handler = new CompanyUpdateCommandHandler(db_context, _mockGetCompaniesMediator.Object);
            CompanyUpdateCommand _update = new CompanyUpdateCommand()
            {
                CompanyId = 1,
                CompanyShortName = "CompanyShort",
                CompanyName = "CompanyName 2",
                Address = "Address 2",
                City = "City",
                State = "Stat",
                PostalCode = "PostalCode",
                Country = "Country",
                PhoneNumber = "PhoneNumber",
                Notes = "Notes",
            };
            Task<int> _updateResults = _handler.Handle(_update, CancellationToken.None);
            int _count = _updateResults.Result;
            Assert.AreEqual(1, _count);
        }
        //
        [TestMethod]
        public void CompanyDeleteCommand_Test()
        {
            Console.WriteLine("CompanyDeleteCommand_Test");
            // add a row to be deleted
            Company _create = new Company()
            {
                CompanyShortName = "CompanyShort",
                CompanyName = "CompanyName",
                Address = "Address",
                City = "City",
                State = "Stat",
                PostalCode = "PostalCode",
                Country = "Country",
                PhoneNumber = "PhoneNumber",
                Notes = "Notes",
            };
            db_context.Companies.Add(_create);
            db_context.SaveChanges();
            // IMediator mediator
            Mock<IMediator> _mockMediator = new Mock<IMediator>();
            //
            CompanyDeleteCommandHandler _handler = new CompanyDeleteCommandHandler(db_context, _mockMediator.Object);
            CompanyDeleteCommand _delete = new CompanyDeleteCommand()
            {
                CompanyId = _create.CompanyId,
            };
            Task<int> _deleteResults = _handler.Handle(_delete, CancellationToken.None);
            int _count = _deleteResults.Result;
            Assert.AreEqual(1, _count);
        }
        //
        //[TestMethod]
        //public void CompanyListQuery_Test()
        //{
        //    CompanyListQueryHandler _handler = new CompanyListQueryHandler(db_context);
        //    CompanyListQueryHandler.ListQuery _listQuery =
        //        new CompanyListQueryHandler.ListQuery();
        //    Task<CompanyListQueryHandler.ViewModel> _viewModelResults =
        //        _handler.Handle(_listQuery, CancellationToken.None);
        //    IList<CompanyServerListQuery> _list = _viewModelResults.Result.CompaniesList;
        //    Assert.IsTrue(_list.Count == 1 || _list.Count == 2);
        //}
        //
        //[TestMethod]
        //public void CompanyDetailQuery_Test()
        //{
        //    CompanyDetailQueryHandler _handler = new CompanyDetailQueryHandler(db_context);
        //    CompanyDetailQueryHandler.DetailQuery _detailQuery =
        //        new CompanyDetailQueryHandler.DetailQuery();
        //    _detailQuery.CompanyId = 1;
        //    Task<CompanyServerDetailQuery> _detailResults =
        //        _handler.Handle(_detailQuery, CancellationToken.None);
        //    CompanyServerDetailQuery _detail = _detailResults.Result;
        //    Assert.AreEqual(1, _detail.CompanyId);
        //}
        //
    }
}
