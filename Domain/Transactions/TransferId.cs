namespace Domain.Transactions;

public readonly record struct TransferId
{
    public Guid Value { get; }

    private TransferId(Guid value)
    {
        Value = value;
    }

    public static TransferId New() => new(Guid.NewGuid());

    public static TransferId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
