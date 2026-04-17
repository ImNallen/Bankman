namespace Domain.Transactions;

public readonly record struct TransactionId
{
    public Guid Value { get; }

    private TransactionId(Guid value)
    {
        Value = value;
    }

    public static TransactionId New() => new(Guid.NewGuid());

    public static TransactionId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
