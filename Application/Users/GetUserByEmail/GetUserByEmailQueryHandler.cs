using System.Data;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Users.GetUserById;
using Dapper;
using Domain.Abstractions;
using Domain.Shared;
using Domain.Users;

namespace Application.Users.GetUserByEmail;

internal sealed class GetUserByEmailQueryHandler : IQueryHandler<GetUserByEmailQuery, UserResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetUserByEmailQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<UserResponse>> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        Result<Email> emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
        {
            return Result.Failure<UserResponse>(emailResult.Error);
        }

        using IDbConnection connection = _sqlConnectionFactory.CreateConnection();

        const string sql = """
            SELECT
                id          AS Id,
                email       AS Email,
                first_name  AS FirstName,
                last_name   AS LastName,
                created_at  AS CreatedAt,
                updated_at  AS UpdatedAt
            FROM users
            WHERE email = @Email
            """;

        UserResponse? user = await connection.QuerySingleOrDefaultAsync<UserResponse>(
            new CommandDefinition(sql, new { Email = emailResult.Value.Value }, cancellationToken: cancellationToken));

        if (user is null)
        {
            return Result.Failure<UserResponse>(UserErrors.NotFound);
        }

        return user;
    }
}
