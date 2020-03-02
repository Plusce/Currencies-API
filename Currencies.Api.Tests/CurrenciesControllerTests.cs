using Currencies.Api.Infrastructure.ErrorHandling;
using Currencies.App.UseCases.GetExchangeRate;
using FluentAssertions;
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
        public async Task GetExchangeRates_WhereTheStartDateIsGreaterThanTheEnd_ShouldBeBadRequestError()
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

        [Fact]
        public async Task GetExchangeRates_WhereTheCurrencyIsNotCorrect_ShouldBeBadRequestError()
        {
            // Arrange
            var url = Arrange_Url("2020-01-05", "2020-01-10", "CHF");

            // Act
            var response = await client.GetAsync(url);
            var deserializedError = await DeserializeModel<ApiErrorResult>(response.Content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var apiError = deserializedError.Errors.Should().ContainSingle().Which;
            apiError.Message.Should().StartWith("Provided currency ISO code is not correct.");

            apiError.Arguments.Should().ContainKey("FieldName: ");
            apiError.Arguments.Should().ContainValue(nameof(GetExchangeRateQuery.CurrencyIsoCode));
        }

        [Fact]
        public async Task GetExchangeRates_WhereTheStartDateIsNotValid_ShouldBeBadRequestError()
        {
            // Arrange
            var url = Arrange_Url("2020-01-32", "2020-01-10", "USD");

            // Act
            var response = await client.GetAsync(url);
            var deserializedError = await DeserializeModel<ApiErrorResult>(response.Content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var apiError = deserializedError.Errors.Should().ContainSingle().Which;
            apiError.Message.Should().StartWith("Provided date is not correct.");

            apiError.Arguments.Should().ContainKey("FieldName: ");
            apiError.Arguments.Should().ContainValue(nameof(GetExchangeRateQuery.StartDate));
        }

        [Fact]
        public async Task GetExchangeRates_WhereTheEndDateIsNotValid_ShouldBeBadRequestError()
        {
            // Arrange
            var url = Arrange_Url("2020-01-05", "2020-01-", "USD");

            // Act
            var response = await client.GetAsync(url);
            var deserializedError = await DeserializeModel<ApiErrorResult>(response.Content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var apiError = deserializedError.Errors.Should().ContainSingle().Which;
            apiError.Message.Should().StartWith("Provided date is not correct.");

            apiError.Arguments.Should().ContainKey("FieldName: ");
            apiError.Arguments.Should().ContainValue(nameof(GetExchangeRateQuery.EndDate));
        }

        [Fact]
        public async Task GetExchangeRates_WhereTheIntervalTimeBetweenStartAndEndDateIsBiggerThanAllowed_ShouldBeBadRequestError()
        {
            // Arrange
            var url = Arrange_Url("2018-01-05", "2020-06-05", "USD");

            // Act
            var response = await client.GetAsync(url);
            var deserializedError = await DeserializeModel<ApiErrorResult>(response.Content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var apiError = deserializedError.Errors.Should().ContainSingle().Which;
            apiError.Message.Should().StartWith("Maximum difference of days between start and the end date cannot be");
            apiError.Arguments.Should().BeNull();
        }

        [Fact]
        public async Task GetExchangeRates_WhereTheStartDateIsGreaterThanTheCurrentDate_ShouldBeBadRequestError()
        {
            // Arrange
            var url = Arrange_Url("2030-01-05", "2030-02-05", "USD");

            // Act
            var response = await client.GetAsync(url);
            var deserializedError = await DeserializeModel<ApiErrorResult>(response.Content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var apiError = deserializedError.Errors.Should().ContainSingle().Which;
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
