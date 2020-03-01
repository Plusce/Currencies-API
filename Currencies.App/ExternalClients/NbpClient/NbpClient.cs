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
            var jsonContent = await MakeRequest();
            return DeserializeModel(jsonContent);

            #region Inner methods

            string PrepareUrl()
            {
                return $"{baseUrl}/{query.CurrencyIsoCode}/{query.StartDate}/{query.EndDate}";
            }

            async Task<string> MakeRequest()
            {
                var url = PrepareUrl();
                var response = await client.GetAsync(url, cancellationToken);

                return await response.Content.ReadAsStringAsync();
            }

            GetNbpExchangeRateModel DeserializeModel(string jsonContent)
            {
                var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<GetNbpExchangeRateModel>(jsonContent, jsonSerializerOptions);
            }

            #endregion
        }
    }
}
