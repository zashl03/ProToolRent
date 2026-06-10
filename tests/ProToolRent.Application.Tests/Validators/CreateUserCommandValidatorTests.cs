using FluentValidation.TestHelper;
using ProToolRent.Application.Commands.CreateUser;
using ProToolRent.Domain.Enums;

namespace ProToolRent.Application.Tests;

public class CreateUserCommandValidatorTests
{
    private readonly CreateUserCommandValidator _validator;

    public CreateUserCommandValidatorTests()
    {
        _validator = new CreateUserCommandValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WhenFirstNameIsInvalid_ShouldHaveError(string? firstName)
    {
        var command = new CreateUserCommand(
            Email: "test@test.com",
            Password: "password",
            FirstName: firstName!,
            LastName: "last",
            City: "city",
            Organization: "org",
            Phone: "123456",
            Role: UserRole.Tenant);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("FirstName of user is required");
    }

    [Fact]
    public void Validate_WhenFirstNameExceedsMaxLength_ShouldHaveError()
    {
        var longFirstName = new string('A', 201);

        var command = new CreateUserCommand(
            Email: "test@test.com",
            Password: "password",
            FirstName: longFirstName,
            LastName: "last",
            City: "city",
            Organization: "org",
            Phone: "123456",
            Role: UserRole.Tenant);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("FirstName of user cannot exceed 200 chars");
    }

    [Fact]
    public void Validate_WhenFirstNameIsExactlyMaxLength_ShouldNotHaveErrors()
    {
        var longFirstName = new string('A', 200);

        var command = new CreateUserCommand(
            Email: "test@test.com",
            Password: "password",
            FirstName: longFirstName,
            LastName: "last",
            City: "city",
            Organization: "org",
            Phone: "123456",
            Role: UserRole.Tenant);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WhenLastNameIsInvalid_ShouldHaveError(string? lastName)
    {
        var command = new CreateUserCommand(
            Email: "test@test.com",
            Password: "password",
            FirstName: "name",
            LastName: lastName!,
            City: "city",
            Organization: "org",
            Phone: "123456",
            Role: UserRole.Tenant);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("LastName of user is required");
    }

    [Fact]
    public void Validate_WhenLastNameExceedsMaxLength_ShouldHaveError()
    {
        var longLastName = new string('A', 201);

        var command = new CreateUserCommand(
            Email: "test@test.com",
            Password: "password",
            FirstName: "name",
            LastName: longLastName,
            City: "city",
            Organization: "org",
            Phone: "123456",
            Role: UserRole.Tenant);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("LastName of user cannot exceed 200 chars");
    }

    [Fact]
    public void Validate_WhenLastNameIsExactlyMaxLength_ShouldNotHaveErrors()
    {
        var longLastName = new string('A', 200);

        var command = new CreateUserCommand(
            Email: "test@test.com",
            Password: "password",
            FirstName: "name",
            LastName: longLastName,
            City: "city",
            Organization: "org",
            Phone: "123456",
            Role: UserRole.Tenant);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WhenCityIsInvalid_ShouldHaveError(string? city)
    {
        var command = new CreateUserCommand(
            Email: "test@test.com",
            Password: "password",
            FirstName: "name",
            LastName: "last",
            City: city!,
            Organization: "org",
            Phone: "123456",
            Role: UserRole.Tenant);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.City)
            .WithErrorMessage("City of user is required");
    }

    [Fact]
    public void Validate_WhenCityExceedsMaxLength_ShouldHaveError()
    {
        var longCity = new string('A', 101);

        var command = new CreateUserCommand(
            Email: "test@test.com",
            Password: "password",
            FirstName: "name",
            LastName: "last",
            City: longCity,
            Organization: "org",
            Phone: "123456",
            Role: UserRole.Tenant);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.City)
            .WithErrorMessage("City cannot exceed 100 chars");
    }

    [Fact]
    public void Validate_WhenCityIsExactlyMaxLength_ShouldNotHaveErrors()
    {
        var longCity = new string('A', 100);

        var command = new CreateUserCommand(
            Email: "test@test.com",
            Password: "password",
            FirstName: "name",
            LastName: "last",
            City: longCity,
            Organization: "org",
            Phone: "123456",
            Role: UserRole.Tenant);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WhenOrganizationIsInvalid_ShouldHaveError(string? organization)
    {
        var command = new CreateUserCommand(
            Email: "test@test.com",
            Password: "password",
            FirstName: "name",
            LastName: "last",
            City: "city",
            Organization: organization!,
            Phone: "123456",
            Role: UserRole.Tenant);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Organization)
            .WithErrorMessage("Organization of user is required");
    }

    [Fact]
    public void Validate_WhenOrganizationExceedsMaxLength_ShouldHaveError()
    {
        var longOrganization = new string('A', 201);

        var command = new CreateUserCommand(
            Email: "test@test.com",
            Password: "password",
            FirstName: "name",
            LastName: "last",
            City: "city",
            Organization: longOrganization,
            Phone: "123456",
            Role: UserRole.Tenant);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Organization)
            .WithErrorMessage("Organization of user cannot exceed 200 chars");
    }

    [Fact]
    public void Validate_WhenOrganizationIsExactlyMaxLength_ShouldNotHaveErrors()
    {
        var longOrganization = new string('A', 200);

        var command = new CreateUserCommand(
            Email: "test@test.com",
            Password: "password",
            FirstName: "name",
            LastName: "last",
            City: "city",
            Organization: longOrganization,
            Phone: "123456",
            Role: UserRole.Tenant);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WhenPhoneIsInvalid_ShouldHaveError(string? phone)
    {
        var command = new CreateUserCommand(
            Email: "test@test.com",
            Password: "password",
            FirstName: "name",
            LastName: "last",
            City: "city",
            Organization: "org",
            Phone: phone!,
            Role: UserRole.Tenant);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Phone)
            .WithErrorMessage("Phone of user is required");
    }

    [Fact]
    public void Validate_WhenPhoneExceedsMaxLength_ShouldHaveError()
    {
        var longPhone = new string('1', 51);

        var command = new CreateUserCommand(
            Email: "test@test.com",
            Password: "password",
            FirstName: "name",
            LastName: "last",
            City: "city",
            Organization: "org",
            Phone: longPhone,
            Role: UserRole.Tenant);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Phone)
            .WithErrorMessage("Phone cannot exceed 50 chars");
    }

    [Fact]
    public void Validate_WhenPhoneIsExactlyMaxLength_ShouldNotHaveErrors()
    {
        var longPhone = new string('1', 50);

        var command = new CreateUserCommand(
            Email: "test@test.com",
            Password: "password",
            FirstName: "name",
            LastName: "last",
            City: "city",
            Organization: "org",
            Phone: longPhone,
            Role: UserRole.Tenant);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenDataIsValid_ShouldNotHaveErrors()
    {
        var command = new CreateUserCommand(
            Email: "test@test.com",
            Password: "password",
            FirstName: "name",
            LastName: "last",
            City: "city",
            Organization: "org",
            Phone: "123456",
            Role: UserRole.Tenant);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenEmailIncorrectForm_ShouldHaveError()
    {
        var command = new CreateUserCommand(
            Email: "testtest.com",
            Password: "password",
            FirstName: "name",
            LastName: "last",
            City: "city",
            Organization: "org",
            Phone: "123456",
            Role: UserRole.Tenant);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Incorrect Email form");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WhenEmailIsInvalid_ShouldHaveError(string? email)
    {
        var command = new CreateUserCommand(
            Email: email!,
            Password: "password",
            FirstName: "name",
            LastName: "last",
            City: "city",
            Organization: "org",
            Phone: "123456",
            Role: UserRole.Tenant);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email of user is required");
    }

    [Fact]
    public void Validate_WhenEmailExceedsMaxLength_ShouldHaveError()
    {
        var longEmail = new string('1', 192) + "@test.com";

        var command = new CreateUserCommand(
            Email: longEmail,
            Password: "password",
            FirstName: "name",
            LastName: "last",
            City: "city",
            Organization: "org",
            Phone: "123456",
            Role: UserRole.Tenant);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email of user cannot exceed 200 chars");
    }

    [Fact]
    public void Validate_WhenEmailIsExactlyMaxLength_ShouldNotHaveErrors()
    {
        var longEmail = new string('1', 191) + "@test.com";

        var command = new CreateUserCommand(
            Email: longEmail,
            Password: "password",
            FirstName: "name",
            LastName: "last",
            City: "city",
            Organization: "org",
            Phone: "123456",
            Role: UserRole.Tenant);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
