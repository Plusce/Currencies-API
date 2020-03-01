using System.Collections.Generic;

namespace Currencies.App.ExternalClients.NbpClient
{
    internal class GetNbpExchangeRateModel
    {
        public IEnumerable<GetNbpExchangeRateDailyModel> Rates { get; set; }
    }
}
