

using System.Runtime.InteropServices;

namespace ProToolRent.Domain.ValueObjects;

public readonly record struct Quantity
{
    public int Total { get; }
    public int Available { get; }

    public Quantity(int total)
    {
        if (total < 0)
            throw new ArgumentException("Total tools cant be less than 0", nameof(total));

        Total = total;
        Available = total;
    }
    public Quantity(int total, int reserved)
    {
        if (total < 0)
            throw new ArgumentException("Total tools cant be less than 0", nameof(total));

        Total = total;
        Available = total - reserved;
    }

    public Quantity Reserve(int reserved)
    {
        return new Quantity(Total, reserved);
    }
}
