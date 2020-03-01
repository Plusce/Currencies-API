namespace Currencies.App.ExternalClients.NbpClient
{
    internal class GetNbpExchangeRateDailyModel
    {
        public string No { get; set; }

        public string EffectiveDate { get; set; }

        public decimal Mid { get; set; }
    }
}
