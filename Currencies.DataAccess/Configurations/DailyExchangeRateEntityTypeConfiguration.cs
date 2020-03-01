using Currencies.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Currencies.DataAccess.Configurations
{
    public class DailyExchangeRateEntityTypeConfiguration : IEntityTypeConfiguration<DailyExchangeRate>
    {
        public void Configure(EntityTypeBuilder<DailyExchangeRate> builder)
        {
            builder.ToTable("DailyExchangeRate");

            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.HasIndex(b => b.Code)
                .IsUnique();
            builder.Property(b => b.Code)
                .IsRequired();

            builder.HasOne(b => b.Currency)
                .WithMany(b => b.DailyExchangeRates)
                .HasForeignKey(b => b.CurrencyId)
                .IsRequired();

            builder.Property(b => b.Date)
                .IsRequired();
        }
    }
}
