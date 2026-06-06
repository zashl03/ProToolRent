using ProToolRent.Domain.Entities;
using ProToolRent.Domain.ValueObjects;

namespace ProToolRent.Domain.tests;

public class OrderItemTests
{
    private readonly Tool _tool;
    public OrderItemTests()
    {
        var specification = new Specification(brand: "Brand", name: "Name", power: 100);
        var quantity = new Quantity(5);
        _tool = new Tool(
            specification: specification, 
            quantity: quantity,
            description: "description",
            price: 100,
            userId: Guid.NewGuid(),
            categoryId: Guid.NewGuid()
        );
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Constructor_WhenCostIsNotPositive_ThrowsException(int invalidCost)
    {
        var ex = Assert.Throws<ArgumentException>(() => 
            new OrderItem(cost: invalidCost, quantity: 1, tool: _tool));

        Assert.Equal("cost", ex.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Constructor_WhenQuantityIsNotPositive_ThrowsException(int invalidQuantity)
    {
        var ex = Assert.Throws<ArgumentException>(() => 
            new OrderItem(cost: 100, quantity: invalidQuantity, tool: _tool));

        Assert.Equal("quantity", ex.ParamName);
    }

    [Fact]
    public void Constructor_WhenDataIsValid_CreatesOrderItemObject()
    {
        var orderItem = new OrderItem(cost: 500, quantity: 1, tool: _tool);

        Assert.Equal(500, orderItem.Cost);
        Assert.Equal(1, orderItem.Quantity);
        Assert.Equal(_tool, orderItem.Tool);
        Assert.True((DateTime.Now - orderItem.CreatedDate).TotalSeconds < 1);
    }
}
