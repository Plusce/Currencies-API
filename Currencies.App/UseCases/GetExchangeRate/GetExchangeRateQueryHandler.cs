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
            await FillGapsInMissingDayExchangeRates(GetDatesBetweenStartAndEndDateDividedByDay());

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

            async Task FillGapsInMissingDayExchangeRates(List<DateTime> datesBetween)
            {
                var gapDaysCollection = await GetGapDaysCollection(datesBetween);
                var gapPeriods = GetGapPeriods(gapDaysCollection);
                var getNbpExchangeRateQueries = ConvertGapPeriodsToNbpExchangeRateQueries(gapPeriods);

                var nbpExchangeRateModel = await NbpClient.GetNbpExchangeRateModels(getNbpExchangeRateQueries, cancellationToken);
                await SaveNbpExchangeRateModels(nbpExchangeRateModel);
            }

            async Task<List<DateTime>> GetGapDaysCollection(List<DateTime> datesBetween)
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

            List<(DateTime startDate, DateTime endDate)> GetGapPeriods(List<DateTime> gapDaysCollection)
            {
                var gapPeriods = new List<(DateTime startDate, DateTime endDate)>();

                var gapDays = 0;
                DateTime? startDateForProvidedInterval = null;

                for (int i = 0; i < gapDaysCollection.Count - 1; i++)
                {
                    if (gapDaysCollection[i].AddDays(1).Equals(gapDaysCollection[i + 1]))
                    {
                        if (startDateForProvidedInterval == null)
                        {
                            startDateForProvidedInterval = gapDaysCollection[i];
                        }
                        gapDays++;
                    }
                    else
                    {
                        if (startDateForProvidedInterval == null)
                        {
                            continue;
                        }

                        AddElementToGapsPeriodCollection(startDateForProvidedInterval, gapDays);
                    }
                }
                if(gapDays > 0)
                {
                    AddElementToGapsPeriodCollection(startDateForProvidedInterval, gapDays);
                }

                return gapPeriods;

                void AddElementToGapsPeriodCollection(DateTime? startDateForProvidedInterval, int gapDays)
                {
                    if (gapDays == 1)
                    {
                        gapPeriods.Add((startDateForProvidedInterval.Value.AddDays(-1),
                        startDateForProvidedInterval.Value));
                    }
                    else
                    {
                        gapPeriods.Add((startDateForProvidedInterval.Value,
                        startDateForProvidedInterval.Value.AddDays(gapDays)));
                    }

                    startDateForProvidedInterval = null;
                    gapDays = 0;
                }
            }

            IEnumerable<GetNbpExchangeRateQuery> ConvertGapPeriodsToNbpExchangeRateQueries
            (List<(DateTime startDate, DateTime endDate)> gapPeriods)
            {
                if (gapPeriods != null)
                {
                    return gapPeriods.Select(gapPeriod => new GetNbpExchangeRateQuery(
                        request.CurrencyIsoCode,
                        gapPeriod.startDate.ToString("yyyy-MM-dd"),
                        gapPeriod.endDate.ToString("yyyy-MM-dd")
                    ));
                }

                return Enumerable.Empty<GetNbpExchangeRateQuery>();
            }

            async Task SaveNbpExchangeRateModels(GetNbpExchangeRateModel nbpExchangeRateModel)
            {
                if (nbpExchangeRateModel != null)
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
