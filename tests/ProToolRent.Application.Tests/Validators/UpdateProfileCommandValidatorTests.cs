using FluentValidation.TestHelper;

namespace ProToolRent.Application.Tests;

public class UpdateProfileCommandValidatorTests
{
    private readonly UpdateProfileCommandValidator _validator;

    public UpdateProfileCommandValidatorTests()
    {
        _validator = new UpdateProfileCommandValidator();
    }

    [Fact]
    public void Validate_WhenDataIsValid_ShouldNotHaveErrors()
    {
        var command = new UpdateProfileCommand(
            UserId: Guid.NewGuid(),
            FirstName: "name",
            LastName: "last",
            City: "city",
            Organization: "org",
            Phone: "123456");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenUserIdIsEmpty_ShouldHaveError()
    {
        var command = new UpdateProfileCommand(
            UserId: Guid.Empty,
            FirstName: "name",
            LastName: "last",
            City: "city",
            Organization: "org",
            Phone: "123456");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("UserId is required");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WhenFirstNameIsInvalid_ShouldHaveError(string? firstName)
    {
        var command = new UpdateProfileCommand(
            UserId: Guid.NewGuid(),
            FirstName: firstName!,
            LastName: "last",
            City: "city",
            Organization: "org",
            Phone: "123456");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("First name is required");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WhenLastNameIsInvalid_ShouldHaveError(string? lastName)
    {
        var command = new UpdateProfileCommand(
            UserId: Guid.NewGuid(),
            FirstName: "name",
            LastName: lastName!,
            City: "city",
            Organization: "org",
            Phone: "123456");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("Last name is required");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WhenCityIsInvalid_ShouldHaveError(string? city)
    {
        var command = new UpdateProfileCommand(
            UserId: Guid.NewGuid(),
            FirstName: "name",
            LastName: "last",
            City: city!,
            Organization: "org",
            Phone: "123456");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.City)
            .WithErrorMessage("City is required");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WhenOrganizationIsInvalid_ShouldHaveError(string? organization)
    {
        var command = new UpdateProfileCommand(
            UserId: Guid.NewGuid(),
            FirstName: "name",
            LastName: "last",
            City: "city",
            Organization: organization,
            Phone: "123456");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Organization)
            .WithErrorMessage("Organization is required");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WhenPhoneIsInvalid_ShouldHaveError(string? phone)
    {
        var command = new UpdateProfileCommand(
            UserId: Guid.NewGuid(),
            FirstName: "name",
            LastName: "last",
            City: "city",
            Organization: "org",
            Phone: phone!);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Phone)
            .WithErrorMessage("Phone number is required");
    }
}
