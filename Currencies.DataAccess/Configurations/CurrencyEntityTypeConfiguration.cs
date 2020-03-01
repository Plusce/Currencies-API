using Currencies.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Currencies.DataAccess.Configurations
{
    public class CurrencyEntityTypeConfiguration : IEntityTypeConfiguration<Currency>
    {
        public void Configure(EntityTypeBuilder<Currency> builder)
        {
            builder.ToTable("Currency");

            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.HasIndex(b => b.IsoCode)
                .IsUnique();
            builder.Property(b => b.IsoCode)
                .HasMaxLength(3)
                .IsRequired();

            builder.HasData(
                new Currency
                {
                    Id = Guid.NewGuid(),
                    IsoCode = "EUR"
                },
                new Currency
                {
                    Id = Guid.NewGuid(),
                    IsoCode = "USD"
                }
            );
        }
    }
}
