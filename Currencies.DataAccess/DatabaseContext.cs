using Currencies.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Currencies.DataAccess
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Currency> Currency { get; set; }

        public DbSet<DailyExchangeRate> DailyExchangeRate { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.ApplyConfigurationsFromAssembly()
        }
    }
}
