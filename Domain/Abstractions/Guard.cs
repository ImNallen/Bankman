using System.Runtime.CompilerServices;

namespace Domain.Abstractions;

public static class Guard
{
    public static string AgainstNullOrWhiteSpace(
        string? value,
        [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", paramName);
        }

        return value;
    }

    public static T AgainstNull<T>(
        T? value,
        [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : class
    {
        if (value is null)
        {
            throw new ArgumentNullException(paramName);
        }

        return value;
    }

    public static T AgainstDefault<T>(
        T value,
        [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : struct, IEquatable<T>
    {
        if (value.Equals(default))
        {
            throw new ArgumentException("Value cannot be the default value.", paramName);
        }

        return value;
    }

    public static decimal AgainstNegative(
        decimal value,
        [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(paramName, value, "Value cannot be negative.");
        }

        return value;
    }

    public static decimal AgainstNegativeOrZero(
        decimal value,
        [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(paramName, value, "Value must be greater than zero.");
        }

        return value;
    }
}
