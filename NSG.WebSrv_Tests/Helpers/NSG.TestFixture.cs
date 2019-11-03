//
using System;
using System.IO;
using System.Net.Http;
//
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
//
namespace NSG.Integration.Helpers
{
    public class TestFixture: IDisposable
    {
        //
        private readonly IWebHostBuilder _builder;
        private readonly TestServer _server;
        private readonly HttpClient _client;
        //
        public IWebHostBuilder builder { get { return _builder; } }
        public TestServer server { get { return _server; } }
        public HttpClient client { get { return _client; } }
        //
        /// <summary>
        /// Class constructor
        /// </summary>
        public TestFixture()
        {
            _builder = new WebHostBuilder()
                .UseContentRoot(@"C:\Dat\Nsg\L\Web\17\CoreWeb1\CoreWeb2")
                .UseEnvironment("Development")
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    // config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json");
                })
                .UseStartup<TestStartup>();
            _server = new TestServer(_builder);
            _client = _server.CreateClient();
        }
        //
        /// <summary>
        /// delete/dispose resources
        /// </summary>
        public void Dispose()
        {
            _client.Dispose();
            _server.Dispose();
        }
        //
    }
}
