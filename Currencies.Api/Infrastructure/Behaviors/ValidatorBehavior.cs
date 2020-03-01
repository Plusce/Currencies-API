using FluentValidation;
using FluentValidation.Results;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Currencies.Api.Infrastructure.Exceptions;

namespace Currencies.Api.Infrastructure.Behaviors
{
    public class ValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IValidator<TRequest>[] validators;

        public ValidatorBehavior(IValidator<TRequest>[] validators) => this.validators = validators;

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var failures = await GatherValidationFailuresAsync(request, cancellationToken);

            if (failures.Any())
            {
                ThrowValidationException(failures);
            }

            var response = await next();
            return response;
        }

        private async Task<List<ValidationFailure>> GatherValidationFailuresAsync(TRequest request, CancellationToken cancellationToken)
        {
            var failures = new List<ValidationFailure>();
            foreach (var validator in validators)
            {
                var f = await validator.ValidateAsync(request, cancellationToken);
                if (f?.Errors == null)
                {
                    continue;
                }

                foreach (var error in f.Errors)
                {
                    if (error != null)
                    {
                        failures.Add(error);
                    }
                }
            }

            return failures;
        }

        private static void ThrowValidationException(IEnumerable<ValidationFailure> failures)
        {
            var exception = new Exceptions.ValidationException();
            exception.ValidationErrors.AddRange(failures.Select(f =>
            {
                return new ValidationError(f.ErrorCode, f.ErrorMessage);
            }));

            throw exception;
        }
    }
}
