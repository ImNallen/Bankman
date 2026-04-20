using Domain.Abstractions;

namespace Domain.Shared;

public sealed record PasswordHash : ValueObject
{
    public static readonly Error Empty = Error.Validation(
        "PasswordHash.Empty",
        "Password hash cannot be empty.");

    public string Value { get; }

    private PasswordHash()
    {
    }

    private PasswordHash(string value)
    {
        Value = value;
    }

    public static Result<PasswordHash> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<PasswordHash>(Empty);
        }

        return new PasswordHash(value);
    }

    public override string ToString() => "***";
}
