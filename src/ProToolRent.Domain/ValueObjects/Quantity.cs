

namespace ProToolRent.Domain.ValueObjects;

public readonly record struct Quantity
{
    public int Total { get; }
    public int Reserved { get; }

    public Quantity(int total)
    {
        if (total < 0)
            throw new ArgumentException("Total tools cant be less than 0", nameof(total));

        Total = total;
        Reserved = 0;
    }
}
