namespace Domain.Users;

public readonly record struct UserId
{
    public Guid Value { get; }

    private UserId(Guid value)
    {
        Value = value;
    }

    public static UserId New() => new(Guid.NewGuid());

    public static UserId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
