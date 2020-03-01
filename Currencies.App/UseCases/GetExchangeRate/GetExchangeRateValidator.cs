using Currencies.DataAccess;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Currencies.App.UseCases.GetExchangeRate
{
    public class GetExchangeRateValidator : AbstractValidator<GetExchangeRateQuery>
    {
        public GetExchangeRateValidator(DatabaseContext databaseContext)
        {
            RuleFor(query => query.CurrencyIsoCode)
                .NotNull()
                .NotEmpty()
                .CustomAsync(async (isoCode, customContext, cancellationToken) =>
                {
                    var isIsoCodeCorrect = await databaseContext.Currency.AnyAsync(currency => currency.IsoCode == isoCode, cancellationToken);

                    if (!isIsoCodeCorrect)
                    {
                        customContext.AddFailure(nameof(GetExchangeRateQuery.CurrencyIsoCode),
                            "Provided currency ISO code is not correct. The list of the available codes is provided in the method description.");
                    }
                });

            RuleFor(query => query.StartDate)
                .NotNull()
                .NotEmpty()
                .Custom((startDate, customContext) =>
                {
                    var isDateCorrect = DateTime.TryParse(startDate, out DateTime parsedDate);

                    if (!isDateCorrect)
                    {
                        customContext.AddFailure(nameof(GetExchangeRateQuery.StartDate),
                               "Provided start date is not correct. Proper date format should be \"YYYY-MM-DD\" (for example \"2012-01-31\".");
                    }
                });

            RuleFor(query => query.EndDate)
                .NotNull()
                .NotEmpty()
                .Custom((endDate, customContext) =>
                {
                    var isDateCorrect = DateTime.TryParse(endDate, out DateTime parsedDate);

                    if (!isDateCorrect)
                    {
                        customContext.AddFailure(nameof(GetExchangeRateQuery.EndDate),
                               "Provided end date is not correct. Proper date format should be \"YYYY-MM-DD\" (for example \"2012-01-31\".");
                    }
                });
        }
    }
}
