using ProToolRent.Domain.ValueObjects;

namespace ProToolRent.Domain.tests.ValueObjects;

public class QuantityTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Constructor_WhenTotalIsNotPositive_ThrowsException(int invalidQuantity)
    {
        var ex = Assert.Throws<ArgumentException>(() => new Quantity(invalidQuantity));

        Assert.Equal("total", ex.ParamName);
    }

    [Fact]
    public void Constructor_WhenDataIsValid_SetsTotal()
    {
        var quantity = new Quantity(10);

        Assert.Equal(10, quantity.Total);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Constructor_OverloadedWithReserved_WhenTotalIsNotPositive_ThrowsException(int invalidQuantity)
    {
        var ex = Assert.Throws<ArgumentException>(() => new Quantity(invalidQuantity, 2));

        Assert.Equal("total", ex.ParamName);
    }

    [Fact]
    public void Constructor_OverloadedWithReserved_WhenReservedIsMoreThanTotal_ThrowsException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new Quantity(10, 11));

        Assert.Equal("reserved", ex.ParamName);
    }

    [Fact]
    public void Constructor_OverloadedWithReserved_WhenDataIsValid_SetsTotalAndReserved()
    {
        var quantity = new Quantity(10, 1);

        Assert.Equal(10, quantity.Total);
        Assert.Equal(10 - 1, quantity.Available);
    } 

    [Fact]
    public void Reserve_WhenValid_Reserves()
    {
        var quantity = new Quantity(10);

        var newQuantity = quantity.Reserve(5);

        Assert.Equal(quantity.Total - 5, newQuantity.Available);
    }
}
