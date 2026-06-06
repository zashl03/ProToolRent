using ProToolRent.Domain.Entities;

namespace ProToolRent.Domain.tests;

public class CategoryTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WhenNameIsInvalid_ThrowsException(string? invalidName)
    {
        var ex = Assert.Throws<ArgumentException>(() => new Category(invalidName!));

        Assert.Equal("name", ex.ParamName);
    }

    [Fact]
    public void Constructor_WhenValidName_CreatesRootCategory()
    {
        var category = new Category("Tools");

        Assert.Equal("Tools", category.Name);
        Assert.Null(category.ParentId);
        Assert.Null(category.Parent);
    }

    [Fact]
    public void Constructor_WithParent_CreatesChildCategory()
    {
        var parent = new Category("Tools");
        var child = new Category("Hand tools", parent.Id, parent);

        Assert.Equal(parent.Id, child.ParentId);
        Assert.Equal(parent, child.Parent);
    }
}
