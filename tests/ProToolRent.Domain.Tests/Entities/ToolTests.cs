using ProToolRent.Domain.Entities;
using ProToolRent.Domain.ValueObjects;

namespace ProToolRent.Domain.tests;

public class ToolTests
{
    [Fact]
    public void Constructor_WhenPriceIsNegative_ThrowsException()
    {
        var specification = new Specification("Brand", "Name", 100);
        var quantity = new Quantity(10);

        var ex = Assert.Throws<ArgumentException>(() => new Tool(
            specification,
            quantity,
            "description",
            -5,
            Guid.NewGuid(),
            Guid.NewGuid()
            ));
        
        Assert.Equal("price", ex.ParamName);
    }

    [Fact]
    public void Constructor_WhenDataIsValid_CreatesToolObject()
    {
        var specification = new Specification("Brand", "Name", 100);
        var quantity = new Quantity(10);
        var categoryId = Guid.NewGuid();
        var userId = Guid.NewGuid();


        var tool = new Tool(
            specification,
            quantity,
            "description",
            100,
            categoryId,
            userId
            );
        
        Assert.Equal("description", tool.Description);
        Assert.Equal(100, tool.Price);
        Assert.Equal(categoryId, tool.CategoryId);
        Assert.Equal(userId, tool.UserId);
        Assert.Equal(specification, tool.Specification);
        Assert.Equal(quantity, tool.Quantity);
    }
}
