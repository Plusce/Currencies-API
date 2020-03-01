using System.Collections.Generic;

namespace Currencies.App.UseCases.GetExchangeRate
{
    /// <summary>
    /// Model contains currency ISO code, daily exchange rates and average exchange rate for the given time interval
    /// (where the base currency for which is the exchange rate calculated is PLN).
    /// </summary>
    public class GetExchangeRateModel
    {
        public GetExchangeRateModel(IList<GetExchangeRateDailyModel> dailyExchangeRates, decimal averageExchangeRate, string isoCurrencyCode)
        {
            DailyExchangeRates = dailyExchangeRates;
            AverageExchangeRate = averageExchangeRate;
            IsoCurrencyCode = isoCurrencyCode;
        }

        /// <summary>
        /// Daily exchange rates for the given time period.
        /// </summary>
        public IList<GetExchangeRateDailyModel> DailyExchangeRates { get; set; }

        /// <summary>
        /// Average exchange rate for the given time period.
        /// To obtain the time interval, calculate the difference between the earliest and the latest dates.
        /// </summary>
        public decimal AverageExchangeRate { get; set; }

        /// <summary>
        /// ISO-4217 currency code
        /// </summary>
        public string IsoCurrencyCode { get; set; }
    }
}
