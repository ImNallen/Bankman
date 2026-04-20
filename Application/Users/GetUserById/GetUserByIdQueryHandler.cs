using System.Data;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Dapper;
using Domain.Abstractions;
using Domain.Users;

namespace Application.Users.GetUserById;

internal sealed class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetUserByIdQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<UserResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
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
            WHERE id = @UserId
            """;

        UserResponse? user = await connection.QuerySingleOrDefaultAsync<UserResponse>(
            new CommandDefinition(sql, new { request.UserId }, cancellationToken: cancellationToken));

        if (user is null)
        {
            return Result.Failure<UserResponse>(UserErrors.NotFound);
        }

        return user;
    }
}
