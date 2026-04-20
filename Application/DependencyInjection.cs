using Application.Abstractions.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<IMarker>());

        services.AddValidatorsFromAssemblyContaining<IMarker>(includeInternalTypes: true);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
