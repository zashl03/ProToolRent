using ProToolRent.Domain.ValueObjects;
namespace ProToolRent.Domain.tests.ValueObjects;
public class SpecificationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WhenBrandIsInvalid_ThrowsException(string? brand)
    {
        var ex = Assert.Throws<ArgumentException>(() => new Specification(brand!, "123", 3));

        Assert.Equal("brand", ex.ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WhenNameIsInvalid_ThrowsException(string? name)
    {
        var ex = Assert.Throws<ArgumentException>(() => new Specification("123", name!, 3));

        Assert.Equal("name", ex.ParamName);
    }

    [Fact]
    public void Constructor_WhenPowerIsNotPositive_ThrowsException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new Specification("123", "123", 0));

        Assert.Equal("power", ex.ParamName);
    }

    [Fact]
    public void Constructor_WhenDataIsValid_CreatesObject()
    {
        var specification = new Specification(
            "Bosch",
            "DF277",
            100
        );

        Assert.Equal("Bosch", specification.Brand);
        Assert.Equal("DF277", specification.Name);
        Assert.Equal(100, specification.Power);
    }
}