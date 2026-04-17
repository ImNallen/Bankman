using Domain.Abstractions;
using Domain.Accounts;

namespace Domain.Transactions;

public interface ITransactionRepository : IRepository<Transaction, TransactionId>
{
    Task<IReadOnlyList<Transaction>> GetByAccountAsync(
        AccountId accountId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Transaction>> GetByTransferIdAsync(
        TransferId transferId,
        CancellationToken cancellationToken = default);
}
