using Domain.Abstractions;
using Domain.Shared;
using Domain.Users;

namespace Domain.Accounts;

public interface IAccountRepository : IRepository<Account, AccountId>
{
    Task<Account?> GetByAccountNumberAsync(AccountNumber number, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Account>> GetByOwnerAsync(UserId ownerId, CancellationToken cancellationToken = default);
}
