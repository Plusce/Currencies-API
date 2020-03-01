using Currencies.DataAccess;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;

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
                            "Provided currency ISO code is not correct. The list of the available codes is provided in the method's documentation description.");
                    }
                });

            RuleFor(query => query.StartDate)
                .NotNull()
                .NotEmpty()
                .Custom((endDate, customContext) =>
                    ProvidedDateShouldHaveProperFormat(endDate, nameof(GetExchangeRateQuery.StartDate), customContext));

            RuleFor(query => query.EndDate)
                .NotNull()
                .NotEmpty()
                .Custom((endDate, customContext) =>
                    ProvidedDateShouldHaveProperFormat(endDate, nameof(GetExchangeRateQuery.EndDate), customContext));

            When(query => IsProvidedDateCorrect(query.StartDate) && IsProvidedDateCorrect(query.EndDate), () => 
            {
                RuleFor(query => query).Custom((query, customContext) =>
                {
                    var startDateAfterFormat = DateTime.Parse(query.StartDate);
                    var endDateAfterFormat = DateTime.Parse(query.EndDate);

                    if(startDateAfterFormat > endDateAfterFormat)
                    {
                        customContext.AddFailure(nameof(GetExchangeRateQuery.StartDate),
                            $"Provided start date is greater than the end date.");
                    }
                    else
                    {
                        TimeSpan difference = endDateAfterFormat - startDateAfterFormat;
                        var daysBetweenTwoDates = difference.TotalDays;
                        if(daysBetweenTwoDates > GetExchangeRateConsts.MaximumDifferenceOfDaysBetweenTwoDates)
                        {
                            customContext.AddFailure("Maximum difference of days between start and the end date cannot be grater than " +
                                $"{ GetExchangeRateConsts.MaximumDifferenceOfDaysBetweenTwoDates}.");
                        }
                    }
                });
            });
        }

        private void ProvidedDateShouldHaveProperFormat(string date, string propertyName, CustomContext customContext)
        {
            if (!IsProvidedDateCorrect(date))
            {
                customContext.AddFailure(propertyName,
                    $"Provided date is not correct. Sample proper date format should be \"YYYY-MM-DD\" (for example \"2012-01-31\".");
            }
        }

        private bool IsProvidedDateCorrect(string date)
        {
            var dividedDate = date?.Split("-");

            if(dividedDate.Length != 3)
            {
                return false;
            }

            var yearsResult = int.TryParse(dividedDate[0], out int years);
            if(!yearsResult || years < 1900 || years > DateTime.UtcNow.Year)
            {
                return false;
            }

            var monthsResult = int.TryParse(dividedDate[1], out int months);
            if (!monthsResult || months < 1 || months > 12)
            {
                return false;
            }

            var daysResult = int.TryParse(dividedDate[2], out int days);
            if(!daysResult || months < 1 || days > 31)
            {
                return false;
            }

            return DateTime.TryParse(date, out DateTime parsedDate);
        }
    }
}
