using ProToolRent.Domain.Entities;
using ProToolRent.Domain.ValueObjects;

namespace ProToolRent.Domain.tests;

public class OrderItemTests
{
    private readonly Tool _tool;
    public OrderItemTests()
    {
        var specification = new Specification("Brand", "Name", 100);
        var quantity = new Quantity(5);
        _tool = new Tool(
            specification, 
            quantity,
            "description",
            100,
            Guid.NewGuid(),
            Guid.NewGuid()
        );
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Constructor_WhenCostIsNotPositive_ThrowsException(int cost)
    {
        var ex = Assert.Throws<ArgumentException>(() => new OrderItem(cost, 1, _tool));

        Assert.Equal("cost", ex.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Constructor_WhenQuantityIsNotPositive_ThrowsException(int quantity)
    {
        var ex = Assert.Throws<ArgumentException>(() => new OrderItem(100, quantity, _tool));

        Assert.Equal("quantity", ex.ParamName);
    }

    [Fact]
    public void Constructor_WhenDataIsValid_CreatesOrderItemObject()
    {
        var orderItem = new OrderItem(500, 1, _tool);

        Assert.Equal(500, orderItem.Cost);
        Assert.Equal(1, orderItem.Quantity);
        Assert.Equal(_tool, orderItem.Tool);
        Assert.True((DateTime.Now - orderItem.CreatedDate).TotalSeconds < 1);
    }
}
