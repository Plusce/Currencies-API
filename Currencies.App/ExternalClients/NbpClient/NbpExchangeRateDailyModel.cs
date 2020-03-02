namespace Currencies.App.ExternalClients.NbpClient
{
    internal class NbpExchangeRateDailyModel
    {
        public string No { get; set; }

        public string EffectiveDate { get; set; }

        public decimal Mid { get; set; }
    }
}
