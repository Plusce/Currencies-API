using System;

namespace Currencies.App.UseCases.GetExchangeRate
{
    /// <summary>
    /// Daily model for exchange rate
    /// </summary>
    public class GetExchangeRateDailyModel
    {
        public GetExchangeRateDailyModel()
        {

        }

        public GetExchangeRateDailyModel(DateTime date, decimal averageExchangeDate)
        {
            Date = date;
            AverageExchangeRate = averageExchangeDate;
        }

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
