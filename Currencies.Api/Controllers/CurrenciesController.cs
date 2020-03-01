using Currencies.App.UseCases.GetExchangeRate;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Currencies.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrenciesController : ControllerBase
    {
        private readonly ILogger<CurrenciesController> _logger;

        public CurrenciesController(ILogger<CurrenciesController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets exchange rate of specified currency for the provided time interval
        /// (where the base currency for which the exchange rate is calculated is PLN).
        /// Currently available exchange rates are: USD, EUR.
        /// </summary>
        /// <param name="currencyIsoCode">ISO-4217 currency code</param>
        /// <param name="startDate">Start date. Should be provided in YYYY-MM-DD format (for example "2012-01-31").</param>
        /// <param name="endDate">End date. Should be provided in YYYY-MM-DD format (for example "2012-01-31").</param>
        /// <returns>Model containing data with exchange rates and average value for specified period.</returns>
        [HttpGet("{currencyIsoCode}/{startDate}/{endDate}")]
        public async Task<GetExchangeRateModel> GetExchangeRate([FromServices] IMediator mediator,
            CancellationToken cancellationToken,
            string currencyIsoCode = "USD",
            string startDate = "2012-01-01",
            string endDate = "2012-01-10")
        {
            var getExchangeRateQuery = new GetExchangeRateQuery(currencyIsoCode, startDate, endDate);
            return await mediator.Send(getExchangeRateQuery, cancellationToken);
        }
    }
}
