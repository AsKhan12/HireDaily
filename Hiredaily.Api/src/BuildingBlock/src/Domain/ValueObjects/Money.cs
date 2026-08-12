namespace Hiredaily.BuildingBlock.Domain.ValueObjects;

public sealed record Money
{
    public decimal Amount { get; private set;}
    public string Currency { get; private set;}

    private Money()
    {
        
    }

    public Money(decimal amount, string currency)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Amount cannot be negative.");

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException(
                "Currency is required.",
                nameof(currency));

        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);

        return new Money(
            Amount + other.Amount,
            Currency);
    }

    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);

        if (Amount < other.Amount)
            throw new InvalidOperationException(
                "Resulting amount cannot be negative.");

        return new Money(
            Amount - other.Amount,
            Currency);
    }

    public Money Multiply(decimal factor)
    {
        if (factor < 0)
            throw new ArgumentOutOfRangeException(nameof(factor));

        return new Money(
            Amount * factor,
            Currency);
    }

    public bool IsZero => Amount == 0;

    public static Money Zero(string currency)
        => new(0, currency);

    public static Money operator +(Money left, Money right)
        => left.Add(right);

    public static Money operator -(Money left, Money right)
        => left.Subtract(right);

    private void EnsureSameCurrency(Money other)
    {
        if (Currency != other.Currency)
        {
            throw new InvalidOperationException(
                $"Cannot operate on {Currency} and {other.Currency}.");
        }
    }

    public override string ToString()
        => $"{Amount:N2} {Currency}";
}