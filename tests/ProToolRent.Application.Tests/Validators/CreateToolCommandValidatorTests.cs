using FluentValidation.TestHelper;
using ProToolRent.Application.Commands.CreateTool;
using ProToolRent.Domain.Entities;

namespace ProToolRent.Application.Tests;

public class CreateToolCommandValidatorTests
{
    private readonly CreateToolCommandValidator _validator;

    public CreateToolCommandValidatorTests()
    {
        _validator = new CreateToolCommandValidator();
    }

    [Fact]
    public void Validate_WhenDataIsValid_ShouldNotHaveErrors()
    {
        var command = new CreateToolCommand(
            Brand: "brand",
            Name: "name",
            Power: 100,
            Description: "desc",
            TotalQuantity: 5,
            Price: 1000,
            CategoryId: Guid.NewGuid(),
            UserId: Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenBrandIsEmpty_ShouldHaveError()
    {
        var command = new CreateToolCommand(
            Brand: "",
            Name: "name",
            Power: 100,
            Description: "desc",
            TotalQuantity: 5,
            Price: 1000,
            CategoryId: Guid.NewGuid(),
            UserId: Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Brand)
            .WithErrorMessage("Brand is required");
    }

    [Fact]
    public void Validate_WhenBrandIsNull_ShouldHaveError()
    {
        var command = new CreateToolCommand(
            Brand: null!,
            Name: "name",
            Power: 100,
            Description: "desc",
            TotalQuantity: 5,
            Price: 1000,
            CategoryId: Guid.NewGuid(),
            UserId: Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Brand)
            .WithErrorMessage("Brand is required");
    }

    [Fact]
    public void Validate_WhenBrandExceedsMaxLength_ShouldHaveError()
    {
        var longBrand = new string('A', 201);

        var command = new CreateToolCommand(
            Brand: longBrand,
            Name: "name",
            Power: 100,
            Description: "desc",
            TotalQuantity: 5,
            Price: 1000,
            CategoryId: Guid.NewGuid(),
            UserId: Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Brand)
            .WithErrorMessage("Brand cannot exceed 200 chars");
    }

    [Fact]
    public void Validate_WhenBrandIsExactlyMaxLength_ShouldNotHaveError()
    {
        var longBrand = new string('A', 200);

        var command = new CreateToolCommand(
            Brand: longBrand,
            Name: "name",
            Power: 100,
            Description: "desc",
            TotalQuantity: 5,
            Price: 1000,
            CategoryId: Guid.NewGuid(),
            UserId: Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenNameIsEmpty_ShouldHaveError()
    {
        var command = new CreateToolCommand(
            Brand: "brand",
            Name: "",
            Power: 100,
            Description: "desc",
            TotalQuantity: 5,
            Price: 1000,
            CategoryId: Guid.NewGuid(),
            UserId: Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name is required");
    }

    [Fact]
    public void Validate_WhenNameIsNull_ShouldHaveError()
    {
        var command = new CreateToolCommand(
            Brand: "brand",
            Name: null!,
            Power: 100,
            Description: "desc",
            TotalQuantity: 5,
            Price: 1000,
            CategoryId: Guid.NewGuid(),
            UserId: Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name is required");
    }

    [Fact]
    public void Validate_WhenNameExceedsMaxLength_ShouldHaveError()
    {
        var longName = new string('A', 201);

        var command = new CreateToolCommand(
            Brand: "brand",
            Name: longName,
            Power: 100,
            Description: "desc",
            TotalQuantity: 5,
            Price: 1000,
            CategoryId: Guid.NewGuid(),
            UserId: Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name cannot exceed 200 chars");
    }

    [Fact]
    public void Validate_WhenNameIsExactlyMaxLength_ShouldNotHaveError()
    {
        var longName = new string('A', 200);

        var command = new CreateToolCommand(
            Brand: "brand",
            Name: longName,
            Power: 100,
            Description: "desc",
            TotalQuantity: 5,
            Price: 1000,
            CategoryId: Guid.NewGuid(),
            UserId: Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(100)]
    public void Validate_WhenPowerIsValid_ShouldNotHaveErrors(int power)
    {
        var command = new CreateToolCommand(
            Brand: "brand",
            Name: "name",
            Power: power,
            Description: "desc",
            TotalQuantity: 5,
            Price: 1000,
            CategoryId: Guid.NewGuid(),
            UserId: Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_WhenPowerIsInvalid_ShouldHaveError(int power)
    {
        var command = new CreateToolCommand(
            Brand: "brand",
            Name: "name",
            Power: power,
            Description: "desc",
            TotalQuantity: 5,
            Price: 1000,
            CategoryId: Guid.NewGuid(),
            UserId: Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Power)
            .WithErrorMessage("Power must be more than 0");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(100)]
    public void Validate_WhenTotalQuantityIsValid_ShouldNotHaveErrors(int totalQuantity)
    {
        var command = new CreateToolCommand(
            Brand: "brand",
            Name: "name",
            Power: 100,
            Description: "desc",
            TotalQuantity: totalQuantity,
            Price: 1000,
            CategoryId: Guid.NewGuid(),
            UserId: Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_WhenTotalQuantityIsInvalid_ShouldHaveError(int totalQuantity)
    {
        var command = new CreateToolCommand(
            Brand: "brand",
            Name: "name",
            Power: 100,
            Description: "desc",
            TotalQuantity: totalQuantity,
            Price: 1000,
            CategoryId: Guid.NewGuid(),
            UserId: Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.TotalQuantity)
            .WithErrorMessage("Total quantity must be more than 0");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(100)]
    public void Validate_WhenPriceIsValid_ShouldNotHaveErrors(decimal price)
    {
        var command = new CreateToolCommand(
            Brand: "brand",
            Name: "name",
            Power: 100,
            Description: "desc",
            TotalQuantity: 1,
            Price: price,
            CategoryId: Guid.NewGuid(),
            UserId: Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_WhenPriceIsInvalid_ShouldHaveError(decimal price)
    {
        var command = new CreateToolCommand(
            Brand: "brand",
            Name: "name",
            Power: 100,
            Description: "desc",
            TotalQuantity: 1,
            Price: price,
            CategoryId: Guid.NewGuid(),
            UserId: Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Price)
            .WithErrorMessage("Price must be more than 0");
    }

    [Fact]
    public void Validate_WhenDescriptionExceedsMaxLength_ShouldHaveError()
    {
        var longDesc = new string('A', 1001);

        var command = new CreateToolCommand(
            Brand: "brand",
            Name: "name",
            Power: 100,
            Description: longDesc,
            TotalQuantity: 5,
            Price: 1000,
            CategoryId: Guid.NewGuid(),
            UserId: Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description cannot exceed 1000 chars");
    }

    [Fact]
    public void Validate_WhenDescriptionIsExactlyMaxLength_ShouldNotHaveError()
    {
        var longDesc = new string('A', 1000);

        var command = new CreateToolCommand(
            Brand: "brand",
            Name: "name",
            Power: 100,
            Description: longDesc,
            TotalQuantity: 5,
            Price: 1000,
            CategoryId: Guid.NewGuid(),
            UserId: Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
