using Currencies.App.ExternalClients.NbpClient;
using Currencies.DataAccess;
using MediatR;
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

            var data = await NbpClient.GetNbpExchangeRateModel(request, cancellationToken);

            return null;

            #region Internal methods

            List<DateTime> GetDatesBetweenStartAndEndDateDividedByDay()
            {
                var startDateAfterFormat = DateTime.Parse(request.StartDate);
                var endDateAfterFormat = DateTime.Parse(request.EndDate);

                var datesBetweenStartAndEndDateDividedByDay = new List<DateTime> { startDateAfterFormat };

                while(endDateAfterFormat > datesBetweenStartAndEndDateDividedByDay.Last().AddDays(1))
                {
                    var dateDayLater = datesBetweenStartAndEndDateDividedByDay.Last().AddDays(1);
                    datesBetweenStartAndEndDateDividedByDay.Add(dateDayLater);
                }

                datesBetweenStartAndEndDateDividedByDay.Add(endDateAfterFormat);

                return datesBetweenStartAndEndDateDividedByDay;
            }

            #endregion
        }
    }
}
