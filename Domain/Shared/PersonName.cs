using Domain.Abstractions;

namespace Domain.Shared;

public sealed record PersonName : ValueObject
{
    public static readonly Error FirstNameEmpty = Error.Validation(
        "PersonName.FirstNameEmpty",
        "First name cannot be empty.");

    public static readonly Error LastNameEmpty = Error.Validation(
        "PersonName.LastNameEmpty",
        "Last name cannot be empty.");

    public string FirstName { get; }

    public string LastName { get; }

    private PersonName()
    {
    }

    private PersonName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public static Result<PersonName> Create(string? firstName, string? lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return Result.Failure<PersonName>(FirstNameEmpty);
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            return Result.Failure<PersonName>(LastNameEmpty);
        }

        return new PersonName(firstName.Trim(), lastName.Trim());
    }

    public override string ToString() => $"{FirstName} {LastName}";
}
