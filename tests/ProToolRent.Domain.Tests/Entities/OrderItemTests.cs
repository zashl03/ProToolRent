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
            price: 1000,
            userId: Guid.NewGuid(),
            categoryId: Guid.NewGuid()
        );
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Constructor_WhenQuantityIsNotPositive_ThrowsException(int invalidQuantity)
    {
        var ex = Assert.Throws<ArgumentException>(() => 
            new OrderItem(
                createdDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), 
                endDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
                quantity: invalidQuantity, 
                tool: _tool));

        Assert.Equal("quantity", ex.ParamName);
    }

    [Fact]
    public void Constructor_WhenDataIsValid_CreatesOrderItemObject()
    {
        var orderItem = new OrderItem(
                createdDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), 
                endDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
                quantity: 1, 
                tool: _tool
                );

        Assert.Equal(6000, orderItem.Cost);
        Assert.Equal(1, orderItem.Quantity);
        Assert.Equal(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), orderItem.CreatedDate);
        Assert.Equal(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)), orderItem.EndDate);
        Assert.Equal(_tool, orderItem.Tool);
    }
}
