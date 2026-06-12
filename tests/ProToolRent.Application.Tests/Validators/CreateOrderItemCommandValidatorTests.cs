using FluentValidation.TestHelper;
using ProToolRent.Application.Commands.CreateOrderItem;

namespace ProToolRent.Application.Tests;

public class CreateOrderItemCommandValidatorTests
{
    private readonly CreateOrderItemCommandValidator _validator;

    public CreateOrderItemCommandValidatorTests()
    {
        _validator = new CreateOrderItemCommandValidator();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(100)]
    public void Validate_WhenQuantityIsValid_ShouldNotHaveErrors(int quantity)
    {
        var command = new CreateOrderItemCommand(
            Guid.NewGuid(), 
            Guid.NewGuid(), 
            DateOnly.MinValue, 
            DateOnly.MaxValue, 
            quantity);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_WhenQuantityIsInvalid_ShouldHaveError(int quantity)
    {
        var command = new CreateOrderItemCommand(
            Guid.NewGuid(), 
            Guid.NewGuid(), 
            DateOnly.MinValue, 
            DateOnly.MaxValue, 
            quantity);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Quantity)
            .WithErrorMessage("Quantity must be more than 0");
    }
}
