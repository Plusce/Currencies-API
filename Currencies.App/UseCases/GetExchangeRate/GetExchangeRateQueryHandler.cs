using Currencies.App.ExternalClients.NbpClient;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Currencies.App.UseCases.GetExchangeRate
{
    public class GetExchangeRateQueryHandler : IRequestHandler<GetExchangeRateQuery, GetExchangeRateModel>
    {
        public async Task<GetExchangeRateModel> Handle(GetExchangeRateQuery request, CancellationToken cancellationToken)
        {
            var data = await NbpClient.GetNbpExchangeRateModel(request, cancellationToken);

            return null;
        }
    }
}
