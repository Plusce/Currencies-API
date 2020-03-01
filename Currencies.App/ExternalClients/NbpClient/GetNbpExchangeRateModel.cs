using System.Collections.Generic;

namespace Currencies.App.ExternalClients.NbpClient
{
    internal class GetNbpExchangeRateModel
    {
        public string Code { get; set; }

        public IList<GetNbpExchangeRateDailyModel> Rates { get; set; }
    }
}
