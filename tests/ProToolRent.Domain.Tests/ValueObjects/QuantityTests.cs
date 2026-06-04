using ProToolRent.Domain.ValueObjects;

namespace ProToolRent.Domain.tests.ValueObjects;

public class QuantityTests
{
    [Fact]
    public void Constructor_WhenTotalIsNegative_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new Quantity(-1));
    }

    [Fact]
    public void Constructor_WhenDataIsValid_SetsTotalп()
    {
        var quantity = new Quantity(10);

        Assert.Equal(10, quantity.Total);
    }   
}
