using Currencies.Api.Infrastructure.ErrorHandling;
using Currencies.App.UseCases.GetExchangeRate;
using FluentAssertions;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Currencies.Api.Tests
{
    public class CurrenciesControllerTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        protected readonly HttpClient client;

        public CurrenciesControllerTests(CustomWebApplicationFactory<Startup> factory)
        {
            client = factory.CreateClient();
        }

        [Fact]
        public async Task GetCurrencies_WhereStartDateIsGreaterThanEnd_ShouldBeBadRequestError()
        {
            // Arrange
            var url = Arrange_Url("2020-01-10", "2020-01-05", "USD");

            // Act
            var response = await client.GetAsync(url);
            var deserializedError = await DeserializeModel<ApiErrorResult>(response.Content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var apiError = deserializedError.Errors.Should().ContainSingle().Which;
            apiError.Message.Should().EndWith("greater than the end date.");

            apiError.Arguments.Should().ContainKey("FieldName: ");
            apiError.Arguments.Should().ContainValue(nameof(GetExchangeRateQuery.StartDate));
        }

        private async Task<T> DeserializeModel<T>(HttpContent content)
        {
            var jsonString = await content.ReadAsStringAsync();
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<T>(jsonString, jsonSerializerOptions);
        }

        private string Arrange_Url(string startDate, string endDate, string currencyIsoCode)
        {
            return $"/Currencies/{currencyIsoCode}/{startDate}/{endDate}";
        }
    }
}
