using Domain.Abstractions;
using Domain.Shared;
using Domain.Users;
using Domain.Users.Events;
using Shouldly;

namespace Domain.UnitTests.Users;

public class UserTests
{
    private static Email ValidEmail() => Email.Create("ada@example.com").Value;

    private static PasswordHash ValidPassword() => PasswordHash.Create("$argon2id$hash").Value;

    private static PersonName ValidName() => PersonName.Create("Ada", "Lovelace").Value;

    [Fact]
    public void Register_should_create_user_with_provided_values()
    {
        Email email = ValidEmail();
        PasswordHash password = ValidPassword();
        PersonName name = ValidName();

        Result<User> result = User.Register(email, password, name);

        result.IsSuccess.ShouldBeTrue();
        User user = result.Value;
        user.Id.Value.ShouldNotBe(Guid.Empty);
        user.Email.ShouldBe(email);
        user.PasswordHash.ShouldBe(password);
        user.Name.ShouldBe(name);
        user.CreatedAt.ShouldBe(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        user.UpdatedAt.ShouldBe(user.CreatedAt);
    }

    [Fact]
    public void Register_should_raise_user_registered_domain_event()
    {
        Result<User> result = User.Register(ValidEmail(), ValidPassword(), ValidName());

        User user = result.Value;
        user.DomainEvents.Count.ShouldBe(1);
        var @event = user.DomainEvents.First().ShouldBeOfType<UserRegisteredDomainEvent>();
        @event.UserId.ShouldBe(user.Id);
        @event.Email.ShouldBe(user.Email);
        @event.OccurredOn.ShouldBe(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Register_should_fail_when_email_is_null()
    {
        Result<User> result = User.Register(null!, ValidPassword(), ValidName());

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(UserErrors.EmailRequired);
    }

    [Fact]
    public void Register_should_fail_when_password_is_null()
    {
        Result<User> result = User.Register(ValidEmail(), null!, ValidName());

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(UserErrors.PasswordRequired);
    }

    [Fact]
    public void Register_should_fail_when_name_is_null()
    {
        Result<User> result = User.Register(ValidEmail(), ValidPassword(), null!);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(UserErrors.NameRequired);
    }

    [Fact]
    public void ChangeEmail_should_update_email_and_touch_updated_at()
    {
        User user = User.Register(ValidEmail(), ValidPassword(), ValidName()).Value;
        DateTime originalUpdatedAt = user.UpdatedAt;
        Email newEmail = Email.Create("grace@example.com").Value;

        Result result = user.ChangeEmail(newEmail);

        result.IsSuccess.ShouldBeTrue();
        user.Email.ShouldBe(newEmail);
        user.UpdatedAt.ShouldBeGreaterThanOrEqualTo(originalUpdatedAt);
    }

    [Fact]
    public void ChangeEmail_should_fail_when_email_is_null()
    {
        User user = User.Register(ValidEmail(), ValidPassword(), ValidName()).Value;

        Result result = user.ChangeEmail(null!);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(UserErrors.EmailRequired);
    }

    [Fact]
    public void ChangePassword_should_update_password_and_touch_updated_at()
    {
        User user = User.Register(ValidEmail(), ValidPassword(), ValidName()).Value;
        DateTime originalUpdatedAt = user.UpdatedAt;
        PasswordHash newPassword = PasswordHash.Create("$argon2id$new").Value;

        Result result = user.ChangePassword(newPassword);

        result.IsSuccess.ShouldBeTrue();
        user.PasswordHash.ShouldBe(newPassword);
        user.UpdatedAt.ShouldBeGreaterThanOrEqualTo(originalUpdatedAt);
    }

    [Fact]
    public void ChangePassword_should_fail_when_password_is_null()
    {
        User user = User.Register(ValidEmail(), ValidPassword(), ValidName()).Value;

        Result result = user.ChangePassword(null!);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(UserErrors.PasswordRequired);
    }

    [Fact]
    public void UpdateName_should_update_name_and_touch_updated_at()
    {
        User user = User.Register(ValidEmail(), ValidPassword(), ValidName()).Value;
        DateTime originalUpdatedAt = user.UpdatedAt;
        PersonName newName = PersonName.Create("Grace", "Hopper").Value;

        Result result = user.UpdateName(newName);

        result.IsSuccess.ShouldBeTrue();
        user.Name.ShouldBe(newName);
        user.UpdatedAt.ShouldBeGreaterThanOrEqualTo(originalUpdatedAt);
    }

    [Fact]
    public void UpdateName_should_fail_when_name_is_null()
    {
        User user = User.Register(ValidEmail(), ValidPassword(), ValidName()).Value;

        Result result = user.UpdateName(null!);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(UserErrors.NameRequired);
    }
}
