using Currencies.Domain.Infrastructure;
using System.Collections.Generic;

namespace Currencies.Domain.Entities
{
    public class Currency : Entity
    {
        public string IsoCode { get; set; }

        public virtual List<DailyExchangeRate> DailyExchangeRates {get; set;}
    }
}
