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
            await FillGapsInMissingDailyExchangeRates(GetDatesBetweenStartAndEndDateDividedByDay());

            var getExchangeRateModel = new GetExchangeRateModel();

            return null;

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

            IEnumerable<GetNbpExchangeRateDailyQuery> ConvertGapPeriodsToNbpExchangeRateQueries
            (List<DateTime> gapDays)
            {
                if (gapDays != null)
                {
                    return gapDays.Select(gapPeriod => new GetNbpExchangeRateDailyQuery(
                        request.CurrencyIsoCode,
                        gapPeriod.Date.ToString("yyyy-MM-dd")
                    ));
                }

                return Enumerable.Empty<GetNbpExchangeRateDailyQuery>();
            }

            async Task SaveNbpExchangeRateModels(GetNbpExchangeRateModel nbpExchangeRateModel)
            {
                if (nbpExchangeRateModel != null
                    && nbpExchangeRateModel.Rates != null
                    && nbpExchangeRateModel.Rates.Any())
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
    }
}
