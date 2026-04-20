using Domain.Abstractions;

namespace Domain.Shared;

public sealed record TransactionReference : ValueObject
{
    public const int MaxLength = 140;

    public static readonly Error Empty = Error.Validation(
        "TransactionReference.Empty",
        "Transaction reference cannot be empty.");

    public static readonly Error TooLong = Error.Validation(
        "TransactionReference.TooLong",
        $"Transaction reference cannot exceed {MaxLength} characters.");

    public string Value { get; }

    private TransactionReference()
    {
    }

    private TransactionReference(string value)
    {
        Value = value;
    }

    public static Result<TransactionReference> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<TransactionReference>(Empty);
        }

        string trimmed = value.Trim();

        if (trimmed.Length > MaxLength)
        {
            return Result.Failure<TransactionReference>(TooLong);
        }

        return new TransactionReference(trimmed);
    }

    public override string ToString() => Value;
}
