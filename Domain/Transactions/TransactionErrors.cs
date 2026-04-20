using Domain.Abstractions;

namespace Domain.Transactions;

public static class TransactionErrors
{
    public static readonly Error AccountIdRequired = Error.Validation(
        "Transaction.AccountIdRequired",
        "An account id is required for a transaction.");

    public static readonly Error AmountRequired = Error.Validation(
        "Transaction.AmountRequired",
        "Transaction amount is required.");

    public static readonly Error AmountNotPositive = Error.Validation(
        "Transaction.AmountNotPositive",
        "Transaction amount must be positive.");

    public static readonly Error TransferIdRequired = Error.Validation(
        "Transaction.TransferIdRequired",
        "Transfer id is required for transfer transactions.");

    public static readonly Error TransferIdNotAllowed = Error.Validation(
        "Transaction.TransferIdNotAllowed",
        "Transfer id must not be set for non-transfer transactions.");
}
