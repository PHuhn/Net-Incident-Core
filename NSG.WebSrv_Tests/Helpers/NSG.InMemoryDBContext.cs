//
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
//
using NSG.WebSrv.Domain.Entities;
//
namespace NSG.Integration.Helpers
{
    public class InMemoryApplicationDBContext : IDisposable
    {
        //
        ApplicationDbContext context = null;
        //
        public InMemoryApplicationDBContext()
        {
            // Use in memory application DB context
            var _optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("db_context" + Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging();
            context = new ApplicationDbContext(_optionsBuilder.Options);
        }
        //
        public void Dispose()
        {
            context?.Dispose();
        }
        //
    }
}
