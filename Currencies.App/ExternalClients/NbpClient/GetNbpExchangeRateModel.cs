using System.Collections.Generic;

namespace Currencies.App.ExternalClients.NbpClient
{
    internal class GetNbpExchangeRateModel
    {
        public string code { get; set; }

        public IList<GetNbpExchangeRateDailyModel> Rates { get; set; }
    }
}
