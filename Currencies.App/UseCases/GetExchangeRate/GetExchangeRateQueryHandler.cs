using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Currencies.App.UseCases.GetExchangeRate
{
    public class GetExchangeRateQueryHandler : IRequestHandler<GetExchangeRateQuery, GetExchangeRateModel>
    {
        public Task<GetExchangeRateModel> Handle(GetExchangeRateQuery request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
