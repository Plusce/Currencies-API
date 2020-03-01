using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Makes multiple queries for different time intervals and join result into single model.
        /// </summary>
        /// <param name="queries">Multiple queries</param>
        /// <returns>NBP exchange rate model</returns>
        internal static async Task<GetNbpExchangeRateModel> GetNbpExchangeRateModels(IEnumerable<GetNbpExchangeRateDailyQuery> queries,
            CancellationToken cancellationToken)
        {
            var getNbpExchangeRateModels = new List<GetNbpExchangeRateModel>();

            foreach (var query in queries)
            {
                var dailyModel = await GetNbpExchangeDailyRateModel(query, cancellationToken);

                if(dailyModel != null && dailyModel.NotEmpty())
                {
                    getNbpExchangeRateModels.Add(dailyModel);
                }
            }

            return new GetNbpExchangeRateModel { Rates = getNbpExchangeRateModels.SelectMany(model => model.Rates).Distinct().ToList() };
        }

        internal static async Task<GetNbpExchangeRateModel> GetNbpExchangeDailyRateModel(GetNbpExchangeRateDailyQuery query,
            CancellationToken cancellationToken)
        {
            var jsonContent = await MakeRequest();

            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                return new GetNbpExchangeRateModel();
            }

            return DeserializeModel(jsonContent);

            #region Inner methods

            string PrepareUrl()
            {
                return $"{baseUrl}/{query.CurrencyIsoCode}/{query.Date}";
            }

            async Task<string> MakeRequest()
            {
                var url = PrepareUrl();
                var response = await client.GetAsync(url, cancellationToken);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return string.Empty;
                }

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
