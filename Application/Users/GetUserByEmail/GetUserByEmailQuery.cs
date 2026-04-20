using Application.Abstractions.Messaging;
using Application.Users.GetUserById;

namespace Application.Users.GetUserByEmail;

public sealed record GetUserByEmailQuery(string Email) : IQuery<UserResponse>;
