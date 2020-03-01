using Currencies.DataAccess.Configurations;
using Currencies.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Currencies.DataAccess
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Currency> Currency { get; set; }

        public DbSet<DailyExchangeRate> DailyExchangeRate { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CurrencyEntityTypeConfiguration).Assembly);
        }
    }
}
