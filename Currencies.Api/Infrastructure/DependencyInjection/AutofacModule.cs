using Autofac;
using Currencies.Api.Infrastructure.Behaviors;
using Currencies.App.UseCases.GetExchangeRate;
using FluentValidation;
using MediatR;
using System.Reflection;

namespace Currencies.Api.Infrastructure.DependencyInjection
{
    public class AutofacModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(ValidatorBehavior<,>)).As(typeof(IPipelineBehavior<,>));

            builder
                .RegisterAssemblyTypes(typeof(GetExchangeRateValidator).GetTypeInfo().Assembly)
                .Where(t => t.IsClosedTypeOf(typeof(IValidator<>)))
                .AsImplementedInterfaces();

        }
    }
}
