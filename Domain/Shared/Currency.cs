using Domain.Abstractions;

namespace Domain.Shared;

public sealed record Currency : ValueObject
{
    public static readonly Error Empty = Error.Validation(
        "Currency.Empty",
        "Currency code cannot be empty.");

    public static readonly Error Invalid = Error.Validation(
        "Currency.Invalid",
        "Currency code must be three upper-case ISO-4217 letters.");

    public string Code { get; }

    private Currency()
    {
    }

    private Currency(string code)
    {
        Code = code;
    }

    public static Result<Currency> Create(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return Result.Failure<Currency>(Empty);
        }

        string trimmed = code.Trim();

        if (trimmed.Length != 3)
        {
            return Result.Failure<Currency>(Invalid);
        }

        foreach (char c in trimmed)
        {
            if (c is < 'A' or > 'Z')
            {
                return Result.Failure<Currency>(Invalid);
            }
        }

        return new Currency(trimmed);
    }

    public override string ToString() => Code;
}
