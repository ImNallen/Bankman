using Domain.Accounts;
using Domain.Transactions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class TransactionRepository : ITransactionRepository
{
    private readonly ApplicationDbContext _context;

    public TransactionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Transaction?> GetByIdAsync(TransactionId id, CancellationToken cancellationToken = default) =>
        await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task AddAsync(Transaction aggregate, CancellationToken cancellationToken = default) =>
        await _context.Transactions.AddAsync(aggregate, cancellationToken);

    public Task UpdateAsync(Transaction aggregate, CancellationToken cancellationToken = default)
    {
        _context.Transactions.Update(aggregate);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Transaction aggregate, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException("Transactions are immutable ledger records and cannot be deleted.");

    public async Task<IReadOnlyList<Transaction>> GetByAccountAsync(AccountId accountId, CancellationToken cancellationToken = default) =>
        await _context.Transactions
            .Where(t => t.AccountId == accountId)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Transaction>> GetByTransferIdAsync(TransferId transferId, CancellationToken cancellationToken = default) =>
        await _context.Transactions
            .Where(t => t.TransferId == transferId)
            .ToListAsync(cancellationToken);
}
