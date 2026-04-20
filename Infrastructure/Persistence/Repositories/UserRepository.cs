using System.Data.Common;
using Dapper;
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

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        DbConnection connection = _context.Database.GetDbConnection();
        bool wasOpen = connection.State == System.Data.ConnectionState.Open;

        if (!wasOpen)
        {
            await connection.OpenAsync(cancellationToken);
        }

        try
        {
            Guid? id = await connection.QuerySingleOrDefaultAsync<Guid?>(
                "SELECT id FROM users WHERE email = @Email",
                new { Email = email.Value });

            return id is null ? null : await GetByIdAsync(UserId.From(id.Value), cancellationToken);
        }
        finally
        {
            if (!wasOpen)
            {
                await connection.CloseAsync();
            }
        }
    }
}
