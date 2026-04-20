namespace Domain.Accounts;

public readonly record struct AccountId
{
    public Guid Value { get; }

    private AccountId(Guid value)
    {
        Value = value;
    }

    public bool IsEmpty => Value == Guid.Empty;

    public static AccountId New() => new(Guid.NewGuid());

    public static AccountId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
