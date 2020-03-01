namespace Currencies.App.ExternalClients.NbpClient
{
    public class GetNbpExchangeRateDailyQuery
    {
        public GetNbpExchangeRateDailyQuery(string currencyIsoCode, string date)
        {
            CurrencyIsoCode = currencyIsoCode;
            Date = date;
        }

        public string CurrencyIsoCode { get; set; }

        public string Date { get; set; }
    }
}
