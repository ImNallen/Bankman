using Domain.Abstractions;

namespace Domain.Shared;

public sealed record AccountNumber : ValueObject
{
    public static readonly Error Empty = Error.Validation(
        "AccountNumber.Empty",
        "Account number cannot be empty.");

    public static readonly Error Invalid = Error.Validation(
        "AccountNumber.Invalid",
        "Account number is not a valid IBAN.");

    private static readonly Dictionary<string, int> IbanLengths = new(StringComparer.Ordinal)
    {
        ["AD"] = 24, ["AE"] = 23, ["AL"] = 28, ["AT"] = 20, ["AZ"] = 28,
        ["BA"] = 20, ["BE"] = 16, ["BG"] = 22, ["BH"] = 22, ["BI"] = 27,
        ["BR"] = 29, ["BY"] = 28, ["CH"] = 21, ["CR"] = 22, ["CY"] = 28,
        ["CZ"] = 24, ["DE"] = 22, ["DJ"] = 27, ["DK"] = 18, ["DO"] = 28,
        ["EE"] = 20, ["EG"] = 29, ["ES"] = 24, ["FI"] = 18, ["FK"] = 18,
        ["FO"] = 18, ["FR"] = 27, ["GB"] = 22, ["GE"] = 22, ["GI"] = 23,
        ["GL"] = 18, ["GR"] = 27, ["GT"] = 28, ["HN"] = 28, ["HR"] = 21,
        ["HU"] = 28, ["IE"] = 22, ["IL"] = 23, ["IQ"] = 23, ["IR"] = 26,
        ["IS"] = 26, ["IT"] = 27, ["JO"] = 30, ["KW"] = 30, ["KZ"] = 20,
        ["LB"] = 28, ["LC"] = 32, ["LI"] = 21, ["LT"] = 20, ["LU"] = 20,
        ["LV"] = 21, ["LY"] = 25, ["MC"] = 27, ["MD"] = 24, ["ME"] = 22,
        ["MK"] = 19, ["MN"] = 20, ["MR"] = 27, ["MT"] = 31, ["MU"] = 30,
        ["NI"] = 28, ["NL"] = 18, ["NO"] = 15, ["OM"] = 23, ["PK"] = 24,
        ["PL"] = 28, ["PS"] = 29, ["PT"] = 25, ["QA"] = 29, ["RO"] = 24,
        ["RS"] = 22, ["RU"] = 33, ["SA"] = 24, ["SC"] = 31, ["SD"] = 18,
        ["SE"] = 24, ["SI"] = 19, ["SK"] = 24, ["SM"] = 27, ["SO"] = 23,
        ["ST"] = 25, ["SV"] = 28, ["TL"] = 23, ["TN"] = 24, ["TR"] = 26,
        ["UA"] = 29, ["VA"] = 22, ["VG"] = 24, ["XK"] = 20, ["YE"] = 30,
    };

    public string Value { get; }

    private AccountNumber(string value)
    {
        Value = value;
    }

    public static Result<AccountNumber> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<AccountNumber>(Empty);
        }

        string normalized = Normalize(value);

        if (normalized.Length < 4)
        {
            return Result.Failure<AccountNumber>(Invalid);
        }

        string countryCode = normalized[..2];

        if (!IsLetters(countryCode) || !IsDigits(normalized.AsSpan(2, 2)))
        {
            return Result.Failure<AccountNumber>(Invalid);
        }

        if (!IbanLengths.TryGetValue(countryCode, out int expectedLength) ||
            normalized.Length != expectedLength)
        {
            return Result.Failure<AccountNumber>(Invalid);
        }

        if (!IsAlphanumeric(normalized.AsSpan(4)))
        {
            return Result.Failure<AccountNumber>(Invalid);
        }

        if (!PassesMod97(normalized))
        {
            return Result.Failure<AccountNumber>(Invalid);
        }

        return new AccountNumber(normalized);
    }

    public override string ToString() => Value;

    private static string Normalize(string value)
    {
        Span<char> buffer = stackalloc char[value.Length];
        int length = 0;
        foreach (char c in value)
        {
            if (char.IsWhiteSpace(c))
            {
                continue;
            }

            buffer[length++] = char.ToUpperInvariant(c);
        }

        return new string(buffer[..length]);
    }

    private static bool IsLetters(ReadOnlySpan<char> span)
    {
        foreach (char c in span)
        {
            if (c is < 'A' or > 'Z')
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsDigits(ReadOnlySpan<char> span)
    {
        foreach (char c in span)
        {
            if (c is < '0' or > '9')
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsAlphanumeric(ReadOnlySpan<char> span)
    {
        foreach (char c in span)
        {
            bool isDigit = c is >= '0' and <= '9';
            bool isLetter = c is >= 'A' and <= 'Z';
            if (!isDigit && !isLetter)
            {
                return false;
            }
        }

        return true;
    }

    private static bool PassesMod97(string iban)
    {
        string rearranged = string.Concat(iban.AsSpan(4), iban.AsSpan(0, 4));
        int remainder = 0;
        foreach (char c in rearranged)
        {
            int digit = c is >= '0' and <= '9'
                ? c - '0'
                : c - 'A' + 10;

            int width = digit >= 10 ? 2 : 1;
            remainder = ((remainder * (width == 2 ? 100 : 10)) + digit) % 97;
        }

        return remainder == 1;
    }
}
