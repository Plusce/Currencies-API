namespace Currencies.App.ExternalClients.NbpClient
{
    public class NbpExchangeRateQuery
    {
        public NbpExchangeRateQuery(string currencyIsoCode, string startDate, string endDate)
        {
            CurrencyIsoCode = currencyIsoCode;
            StartDate = startDate;
            EndDate = endDate;
        }

        public string CurrencyIsoCode { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }
    }
}
