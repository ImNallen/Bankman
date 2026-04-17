using Domain.Abstractions;

namespace Domain.Users;

public static class UserErrors
{
    public static readonly Error EmailRequired = Error.Validation(
        "User.EmailRequired",
        "An email is required.");

    public static readonly Error PasswordRequired = Error.Validation(
        "User.PasswordRequired",
        "A password hash is required.");

    public static readonly Error NameRequired = Error.Validation(
        "User.NameRequired",
        "A name is required.");

    public static readonly Error NotFound = Error.NotFound(
        "User.NotFound",
        "The user was not found.");

    public static readonly Error EmailAlreadyInUse = Error.Conflict(
        "User.EmailAlreadyInUse",
        "The email is already in use by another user.");
}
