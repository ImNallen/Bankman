using Domain.Abstractions;

namespace Domain.Accounts;

public static class AccountErrors
{
    public static readonly Error OwnerRequired = Error.Validation(
        "Account.OwnerRequired",
        "An owner is required to open an account.");

    public static readonly Error AccountNumberRequired = Error.Validation(
        "Account.AccountNumberRequired",
        "An account number is required to open an account.");

    public static readonly Error CurrencyRequired = Error.Validation(
        "Account.CurrencyRequired",
        "A currency is required to open an account.");

    public static readonly Error AmountRequired = Error.Validation(
        "Account.AmountRequired",
        "A monetary amount is required.");

    public static readonly Error AmountMustBePositive = Error.Validation(
        "Account.AmountMustBePositive",
        "The amount must be greater than zero.");

    public static readonly Error CurrencyMismatch = Error.Validation(
        "Account.CurrencyMismatch",
        "The amount currency does not match the account currency.");

    public static readonly Error NotActive = Error.Conflict(
        "Account.NotActive",
        "The account is not active.");

    public static readonly Error InsufficientFunds = Error.Conflict(
        "Account.InsufficientFunds",
        "The account has insufficient funds for this debit.");

    public static readonly Error AlreadyFrozen = Error.Conflict(
        "Account.AlreadyFrozen",
        "The account is already frozen.");

    public static readonly Error NotFrozen = Error.Conflict(
        "Account.NotFrozen",
        "The account is not frozen.");

    public static readonly Error AlreadyClosed = Error.Conflict(
        "Account.AlreadyClosed",
        "The account is already closed.");
}
