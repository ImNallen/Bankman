using Application.Abstractions.Messaging;
using Domain.Abstractions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Application.Abstractions.Behaviors;

internal sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next(cancellationToken);
        }

        var context = new ValidationContext<TRequest>(request);

        ValidationFailure[] failures = (await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken))))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToArray();

        if (failures.Length == 0)
        {
            return await next(cancellationToken);
        }

        ValidationFailure first = failures[0];
        var error = Error.Validation(first.PropertyName, first.ErrorMessage);

        return CreateFailureResult(error);
    }

    private static TResponse CreateFailureResult(Error error)
    {
        if (typeof(TResponse) == typeof(Result))
        {
            return (TResponse)(object)Result.Failure(error);
        }

        Type valueType = typeof(TResponse).GetGenericArguments()[0];
        System.Reflection.MethodInfo method = typeof(Result)
            .GetMethods()
            .Single(m => m.Name == nameof(Result.Failure)
                && m.IsGenericMethodDefinition
                && m.GetGenericArguments().Length == 1)
            .MakeGenericMethod(valueType);

        return (TResponse)method.Invoke(null, [error])!;
    }
}
