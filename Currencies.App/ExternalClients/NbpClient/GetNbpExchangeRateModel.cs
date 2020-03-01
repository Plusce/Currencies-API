using System.Collections.Generic;
using System.Linq;

namespace Currencies.App.ExternalClients.NbpClient
{
    internal class GetNbpExchangeRateModel
    {
        public List<GetNbpExchangeRateDailyModel> Rates { get; set; }

        public bool NotEmpty()
        {
            return Rates != null && Rates.Any();
        }
    }
}
