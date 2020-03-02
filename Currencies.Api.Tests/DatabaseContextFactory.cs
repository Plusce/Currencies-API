using Currencies.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;

namespace Currencies.Api.Tests
{
    public static class DatabaseContextFactory
    {
        public static DatabaseContext GetInMemoryDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
               .Options;

            return new DatabaseContext(options);
        }
    }
}
