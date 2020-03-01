using Currencies.Domain.Infrastructure;

namespace Currencies.Domain.Entities
{
    public class Currency : Entity
    {
        public string IsoCode { get; set; }
    }
}
