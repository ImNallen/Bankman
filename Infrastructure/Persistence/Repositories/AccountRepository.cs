using Domain.Accounts;
using Domain.Shared;
using Domain.Users;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class AccountRepository : IAccountRepository
{
    private readonly ApplicationDbContext _context;

    public AccountRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Account?> GetByIdAsync(AccountId id, CancellationToken cancellationToken = default) =>
        await _context.Accounts
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task AddAsync(Account aggregate, CancellationToken cancellationToken = default) =>
        await _context.Accounts.AddAsync(aggregate, cancellationToken);

    public Task UpdateAsync(Account aggregate, CancellationToken cancellationToken = default)
    {
        _context.Accounts.Update(aggregate);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Account aggregate, CancellationToken cancellationToken = default)
    {
        _context.Accounts.Remove(aggregate);
        return Task.CompletedTask;
    }

    public async Task<Account?> GetByAccountNumberAsync(AccountNumber number, CancellationToken cancellationToken = default) =>
        await _context.Accounts
            .FirstOrDefaultAsync(a => a.Number.Value == number.Value, cancellationToken);

    public async Task<IReadOnlyList<Account>> GetByOwnerAsync(UserId ownerId, CancellationToken cancellationToken = default) =>
        await _context.Accounts
            .Where(a => a.OwnerId == ownerId)
            .ToListAsync(cancellationToken);
}
