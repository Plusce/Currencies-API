using MediatR;

namespace Currencies.App.UseCases.GetExchangeRate
{
    public class GetExchangeRateQuery : IRequest<GetExchangeRateModel>
    {
        public GetExchangeRateQuery(string currencyIsoCode, string startDate, string endDate)
        {
            CurrencyIsoCode = currencyIsoCode;
            StartDate = startDate;
            EndDate = endDate;
        }

        public string CurrencyIsoCode { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }
    }
}
