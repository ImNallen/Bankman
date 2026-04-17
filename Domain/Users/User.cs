using Domain.Abstractions;
using Domain.Shared;
using Domain.Users.Events;

namespace Domain.Users;

public sealed class User : AggregateRoot<UserId>
{
    private User(
        UserId id,
        Email email,
        PasswordHash passwordHash,
        PersonName name,
        DateTime createdAt)
        : base(id)
    {
        Email = email;
        PasswordHash = passwordHash;
        Name = name;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public Email Email { get; private set; }

    public PasswordHash PasswordHash { get; private set; }

    public PersonName Name { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public static Result<User> Register(Email email, PasswordHash passwordHash, PersonName name)
    {
        if (email is null)
        {
            return Result.Failure<User>(UserErrors.EmailRequired);
        }

        if (passwordHash is null)
        {
            return Result.Failure<User>(UserErrors.PasswordRequired);
        }

        if (name is null)
        {
            return Result.Failure<User>(UserErrors.NameRequired);
        }

        DateTime now = DateTime.UtcNow;
        var user = new User(UserId.New(), email, passwordHash, name, now);

        user.RaiseDomainEvent(new UserRegisteredDomainEvent(user.Id, user.Email, now));

        return user;
    }

    public Result ChangeEmail(Email email)
    {
        if (email is null)
        {
            return Result.Failure(UserErrors.EmailRequired);
        }

        Email = email;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result ChangePassword(PasswordHash passwordHash)
    {
        if (passwordHash is null)
        {
            return Result.Failure(UserErrors.PasswordRequired);
        }

        PasswordHash = passwordHash;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result UpdateName(PersonName name)
    {
        if (name is null)
        {
            return Result.Failure(UserErrors.NameRequired);
        }

        Name = name;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }
}
