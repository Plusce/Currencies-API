using System.Collections.Generic;
using System.Linq;

namespace Currencies.App.ExternalClients.NbpClient
{
    internal class NbpExchangeRateModel
    {
        public List<NbpExchangeRateDailyModel> Rates { get; set; }

        public bool NotEmpty()
        {
            return Rates != null && Rates.Any();
        }
    }
}
