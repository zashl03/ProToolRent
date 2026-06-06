using ProToolRent.Domain.Entities;
using ProToolRent.Domain.ValueObjects;

namespace ProToolRent.Domain.tests;

public class OrderTests
{
    private Tool CreateTestTool(int seed = 0)
    {
        var specification = new Specification($"Brand{seed}", "Name{seed}", 500);
        var quantity = new Quantity(5);
        return new Tool(
            specification, 
            quantity, 
            "description", 
            1000, 
            Guid.NewGuid(), 
            Guid.NewGuid());
    }

    [Fact]
    public void Constructor_CreatesOrderWithCreatedStatus()
    {
        var userId = Guid.NewGuid();
        var order = new Order(userId);

        Assert.Equal(userId, order.UserId);
        Assert.Equal("Создан", order.Status);
        Assert.NotEqual(default, order.CreatedDate);
    }

    [Fact]
    public void AddItem_WhenDataIsValid_AddsItemToCollection()
    {
        var order = new Order(Guid.NewGuid());
        var tool = CreateTestTool();

        order.AddItem(100, 1, tool);

        var addedItem = Assert.Single(order.OrderItems);

        Assert.Equal(100, addedItem.Cost);
        Assert.Equal(1, addedItem.Quantity);
        Assert.Equal(tool, addedItem.Tool);
    }

    [Fact]
    public void RemoveItem_WhenItemExists_RemovesItemAndReturnsTrue()
    {
        var order = new Order(Guid.NewGuid());
        var tool = CreateTestTool();
        var itemId = order.AddItem(100, 1, tool);

        var removed = order.RemoveItem(itemId);

        Assert.True(removed);
        Assert.Empty(order.OrderItems);
    }

    [Fact]
    public void RemoveItem_WhenItemsNotExists_ReturnsFalse()
    {
        var order = new Order(Guid.NewGuid());

        var removed = order.RemoveItem(Guid.NewGuid());

        Assert.False(removed);
    }

    [Fact]
    public void RemoveItem_WhenItemDoesNotExistInNonEmptyCollection_ReturnsFalse()
    {
        var order = new Order(Guid.NewGuid());
        var tool = CreateTestTool();
        order.AddItem(100, 1, tool);
        var wrongId = Guid.NewGuid();
        
        var removed = order.RemoveItem(wrongId);
        
        Assert.False(removed);
        Assert.Single(order.OrderItems);
    }
}
