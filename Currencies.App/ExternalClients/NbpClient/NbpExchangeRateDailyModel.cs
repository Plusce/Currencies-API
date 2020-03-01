namespace Currencies.App.ExternalClients.NbpClient
{
    internal class GetNbpExchangeRateDailyModel
    {
        public string no { get; set; }

        public string effectiveDate { get; set; }

        public decimal mid { get; set; }
    }
}
