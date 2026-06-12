using FluentValidation.TestHelper;
using ProToolRent.Application.Authentication.Commands.Refresh;

namespace ProToolRent.Application.Tests;

public class RefreshCommandValidatorTests
{
    private readonly RefreshCommandValidator _validator;

    public RefreshCommandValidatorTests()
    {
        _validator = new RefreshCommandValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WhenRefreshTokenIsInvalid_ShouldHaveError(string? refresh)
    {
        var command = new RefreshCommand(refresh!);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.RefreshToken)
            .WithErrorMessage("Refresh token is required");
    }

    [Fact]
    public void Validate_WhenDataIsValid_ShouldNotHaveErrors()
    {
        var command = new RefreshCommand("refresh");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
