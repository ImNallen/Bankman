using Domain.Shared;
using Domain.Users;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default) =>
        await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public async Task AddAsync(User aggregate, CancellationToken cancellationToken = default) =>
        await _context.Users.AddAsync(aggregate, cancellationToken);

    public Task UpdateAsync(User aggregate, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(aggregate);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(User aggregate, CancellationToken cancellationToken = default)
    {
        _context.Users.Remove(aggregate);
        return Task.CompletedTask;
    }

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default) =>
        await _context.Users
            .FirstOrDefaultAsync(u => u.Email.Value == email.Value, cancellationToken);
}
