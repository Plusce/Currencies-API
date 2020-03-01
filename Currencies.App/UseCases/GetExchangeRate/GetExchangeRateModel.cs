using System.Collections.Generic;

namespace Currencies.App.UseCases.GetExchangeRate
{
    /// <summary>
    /// Model containing daily exchange rates and average exchange rate for specified period and currency
    /// (where the base currency for which is the exchange rate calculated is PLN).
    /// </summary>
    public class GetExchangeRateModel
    {
        /// <summary>
        /// Daily exchange rates for specified period.
        /// </summary>
        public IList<GetExchangeRateDailyModel> DailyExchangeRates { get; set; }

        /// <summary>
        /// Average exchange rate during the specified period
        /// </summary>
        public decimal AverageExchangeRate { get; set; }

        /// <summary>
        /// ISO-4217 currency code
        /// </summary>
        public string IsoCurrencyCode { get; set; }
    }
}
