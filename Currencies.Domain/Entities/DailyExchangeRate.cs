using Currencies.Domain.Infrastructure;
using System;

namespace Currencies.Domain.Entities
{
    public class DailyExchangeRate : Entity
    {
        public string Code { get; set; }

        public DateTime Date { get; set; }

        public decimal ExchangeRate { get; set; }

        public virtual Currency Currency { get; set; }
        public Guid CurrencyId { get; set; }
    }
}
