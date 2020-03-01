using Currencies.Domain.Infrastructure;
using System;

namespace Currencies.Domain.Entities
{
    public class DailyExchangeRate : Entity
    {
        private DailyExchangeRate()
        {

        }

        /// <summary>
        /// NBP unique code for single daily exchange rate
        /// </summary>
        public string Code { get; set; }

        public DateTime Date { get; set; }

        public decimal ExchangeRate { get; set; }

        public virtual Currency Currency { get; set; }
        public Guid CurrencyId { get; set; }

        public static DailyExchangeRate Create(string code, DateTime date, decimal exchangeRate, Guid currencyId)
        {
            return new DailyExchangeRate
            {
                Id = Guid.NewGuid(),
                Code = code,
                Date = date,
                ExchangeRate = exchangeRate,
                CurrencyId = currencyId
            };
        }
    }
}
