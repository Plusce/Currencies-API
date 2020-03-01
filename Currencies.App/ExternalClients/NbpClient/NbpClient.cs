using Currencies.App.UseCases.GetExchangeRate;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Currencies.App.ExternalClients.NbpClient
{
    internal static class NbpClient
    {
        private const string baseUrl = "http://api.nbp.pl/api/exchangerates/rates/a";
        private static readonly HttpClient client = new HttpClient();

        internal static async Task<GetNbpExchangeRateModel> GetNbpExchangeRateModel(GetExchangeRateQuery query,
            CancellationToken cancellationToken)
        {
            var url = $"{baseUrl}/{query.CurrencyIsoCode}/{query.StartDate}/{query.EndDate}";
            var nbpResponse = await client.GetAsync(url, cancellationToken);
            var contents = await nbpResponse.Content.ReadAsStringAsync();

            var getNbpExchangeRateModel = JsonSerializer.Deserialize<GetNbpExchangeRateModel>(contents);
            return getNbpExchangeRateModel;
        }
    }
}
