

namespace ProToolRent.Domain.ValueObjects;

public readonly record struct Quantity
{
    public int Total { get; }
    public int Reserved { get; }

    public Quantity(int total, int reserved)
    {
        if (total < 0)
            throw new ArgumentException("Total tools cant be less than 0", nameof(total));

        if (reserved < 0 || reserved > total) 
            throw new ArgumentException("Reserved tools cant be less than 0 or more than total",nameof(reserved));

        Total = total;
        Reserved = reserved;
    }
}
