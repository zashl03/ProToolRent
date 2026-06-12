using FluentValidation.TestHelper;
using ProToolRent.Application.Commands.CreateCategory;

namespace ProToolRent.Application.Tests;

public class CreateCategoryCommandValidatorTests
{
    private readonly CreateCategoryCommandValidator _validator;

    public CreateCategoryCommandValidatorTests()
    {
        _validator = new CreateCategoryCommandValidator();
    }

    [Fact]
    public void Validate_WhenNameIsValid_ShouldNotHaveErrors()
    {
        var command = new CreateCategoryCommand("Eletric tools", null);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenNameIsEmpty_ShouldHaveError()
    {
        var command = new CreateCategoryCommand("", null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name of category is required");
    }

    [Fact]
    public void Validate_WhenNameIsNull_ShouldHaveError()
    {
        var command = new CreateCategoryCommand(null!, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name of category is required");
    }

    [Fact]
    public void Validate_WhenNameExceedsMaxLength_ShouldHaveError()
    {
        var longName = new string('A', 201);

        var command = new CreateCategoryCommand(longName, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name of category cannot exceed 200 chars");
    }

    [Fact]
    public void Validate_WhenNameIsExactlyMaxLength_ShouldNotHaveError()
    {
        var longName = new string('A', 200);

        var command = new CreateCategoryCommand(longName, null);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
