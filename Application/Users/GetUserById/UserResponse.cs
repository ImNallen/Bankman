namespace Application.Users.GetUserById;

public sealed record UserResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    DateTime CreatedAt,
    DateTime UpdatedAt);
