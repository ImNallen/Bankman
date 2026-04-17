using System.Net.Mail;
using Domain.Abstractions;

namespace Domain.Shared;

public sealed record Email : ValueObject
{
    public static readonly Error Empty = Error.Validation(
        "Email.Empty",
        "Email cannot be empty.");

    public static readonly Error Invalid = Error.Validation(
        "Email.Invalid",
        "Email is not a valid email address.");

    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Result<Email> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<Email>(Empty);
        }

#pragma warning disable CA1308 // Email addresses are canonicalized to lower case.
        string normalized = value.Trim().ToLowerInvariant();
#pragma warning restore CA1308

        if (!MailAddress.TryCreate(normalized, out MailAddress? parsed) ||
            !string.Equals(parsed.Address, normalized, StringComparison.Ordinal))
        {
            return Result.Failure<Email>(Invalid);
        }

        return new Email(normalized);
    }

    public override string ToString() => Value;
}
