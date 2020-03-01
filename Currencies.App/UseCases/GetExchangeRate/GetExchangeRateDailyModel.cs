using System;

namespace Currencies.App.UseCases.GetExchangeRate
{
    /// <summary>
    /// Daily model for exchange rate
    /// </summary>
    public class GetExchangeRateDailyModel
    {
        /// <summary>
        /// UTC Date
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Average exchange rate for the given day
        /// </summary>
        public decimal AverageExchangeRate { get; set; }
    }
}
