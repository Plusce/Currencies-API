namespace Currencies.App.ExternalClients.NbpClient
{
    public class NbpExchangeRateDailyQuery
    {
        public NbpExchangeRateDailyQuery(string currencyIsoCode, string date)
        {
            CurrencyIsoCode = currencyIsoCode;
            Date = date;
        }

        public string CurrencyIsoCode { get; set; }

        public string Date { get; set; }
    }
}
