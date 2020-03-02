using Currencies.App.ExternalClients.NbpClient;
using Currencies.DataAccess;
using Currencies.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Currencies.App.UseCases.GetExchangeRate
{
    public class GetExchangeRateQueryHandler : IRequestHandler<GetExchangeRateQuery, GetExchangeRateModel>
    {
        private readonly DatabaseContext databaseContext;

        public GetExchangeRateQueryHandler(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public async Task<GetExchangeRateModel> Handle(GetExchangeRateQuery request, CancellationToken cancellationToken)
        {
            var datesBetweenStartAndEndDateDividedByDay = GetDatesBetweenStartAndEndDateDividedByDay();

            await FillGapsInMissingDailyExchangeRates(datesBetweenStartAndEndDateDividedByDay);

            var model = await CreateExchangeRateModel(datesBetweenStartAndEndDateDividedByDay);

            return model;

            #region Inner methods

            List<DateTime> GetDatesBetweenStartAndEndDateDividedByDay()
            {
                var startDateAfterFormat = DateTime.Parse(request.StartDate);
                var endDateAfterFormat = DateTime.Parse(request.EndDate);

                var datesBetweenStartAndEndDateDividedByDay = new List<DateTime> { startDateAfterFormat };

                while (endDateAfterFormat > datesBetweenStartAndEndDateDividedByDay.Last().AddDays(1))
                {
                    var dateDayLater = datesBetweenStartAndEndDateDividedByDay.Last().AddDays(1);
                    datesBetweenStartAndEndDateDividedByDay.Add(dateDayLater);
                }

                datesBetweenStartAndEndDateDividedByDay.Add(endDateAfterFormat);

                return datesBetweenStartAndEndDateDividedByDay;
            }

            async Task FillGapsInMissingDailyExchangeRates(List<DateTime> datesBetween)
            {
                var gapDays = await GetGapDays(datesBetween);
                var getNbpExchangeRateDailyQueries = ConvertGapPeriodsToNbpExchangeRateQueries(gapDays);

                var nbpExchangeRateModel = await NbpClient.GetNbpExchangeRateModels(getNbpExchangeRateDailyQueries, cancellationToken);
                await SaveNbpExchangeRateModels(nbpExchangeRateModel);
            }

            async Task<List<DateTime>> GetGapDays(List<DateTime> datesBetween)
            {
                var gapDaysCollection = new List<DateTime>();
                for (int i = 0; i < datesBetween.Count; i++)
                {
                    if (!await databaseContext.DailyExchangeRate
                        .AnyAsync(rate => rate.Date.Equals(datesBetween[i]), cancellationToken))
                    {
                        gapDaysCollection.Add(datesBetween[i]);
                    }
                }

                return gapDaysCollection;
            }

            async Task<GetExchangeRateModel> CreateExchangeRateModel(List<DateTime> datesBetween)
            {
                var dailyExchangeRatesDto = await GetDailyExchangeRatesDto(datesBetween);

                var isoCurrencyCode = request.CurrencyIsoCode.ToUpper();
                
                var exchangeRates = dailyExchangeRatesDto.Select(rate => rate.ExchangeRate);
                var averageRatesForProvidedPeriod = exchangeRates.Sum() / exchangeRates.Count();

                var dailyExchangeRates = dailyExchangeRatesDto
                    .Select(dto => new GetExchangeRateDailyModel(dto.Date, dto.ExchangeRate))
                    .ToList();

                var getExchangeRateModel = new GetExchangeRateModel(dailyExchangeRates, averageRatesForProvidedPeriod, isoCurrencyCode);

                return getExchangeRateModel;
            }

            async Task<List<DailyExchangeRateDto>> GetDailyExchangeRatesDto(List<DateTime> datesBetween)
            {
                return await databaseContext
                    .DailyExchangeRate
                    .Where(exchangeRate =>
                        datesBetween.Any(dateBetween => dateBetween == exchangeRate.Date))
                    .Select(exchangeRate => new DailyExchangeRateDto(exchangeRate.Date, exchangeRate.ExchangeRate))
                    .ToListAsync(cancellationToken);
            }

            IEnumerable<NbpExchangeRateDailyQuery> ConvertGapPeriodsToNbpExchangeRateQueries
            (List<DateTime> gapDays)
            {
                if (gapDays != null)
                {
                    return gapDays.Select(gapPeriod => new NbpExchangeRateDailyQuery(
                        request.CurrencyIsoCode,
                        gapPeriod.Date.ToString("yyyy-MM-dd")
                    ));
                }

                return Enumerable.Empty<NbpExchangeRateDailyQuery>();
            }

            async Task SaveNbpExchangeRateModels(NbpExchangeRateModel nbpExchangeRateModel)
            {
                if (nbpExchangeRateModel != null && nbpExchangeRateModel.NotEmpty())
                {
                    foreach (var dailyExchangeRateFromNbp in nbpExchangeRateModel.Rates)
                    {
                        var date = DateTime.Parse(dailyExchangeRateFromNbp.EffectiveDate);
                        var currencyId = (await databaseContext.Currency
                            .FirstAsync(currency => currency.IsoCode == request.CurrencyIsoCode, cancellationToken)).Id;

                        var dailyExchangeRate = DailyExchangeRate.Create(dailyExchangeRateFromNbp.No,
                            date, dailyExchangeRateFromNbp.Mid, currencyId);

                        databaseContext.DailyExchangeRate.Add(dailyExchangeRate);
                    }

                    await databaseContext.SaveChangesAsync(cancellationToken);
                }
            }

            #endregion
        }

        private class DailyExchangeRateDto
        {
            public DailyExchangeRateDto(DateTime date, decimal exchangeRate)
            {
                Date = date;
                ExchangeRate = exchangeRate;
            }
            
            public DateTime Date { get; set; }

            public decimal ExchangeRate { get; set; }
        }
    }
}
