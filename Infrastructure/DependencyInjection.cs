using Application;
using Application.Abstractions;
using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Domain.Abstractions;
using Domain.Accounts;
using Domain.Transactions;
using Domain.Users;
using Infrastructure.Authentication;
using Infrastructure.Events;
using Infrastructure.Jobs;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        services.AddApplication();

        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();

        services.AddSingleton<ISqlConnectionFactory>(_ => new SqlConnectionFactory(connectionString));
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        services.AddHostedService<ProcessOutboxMessagesJob>();

        return services;
    }
}
