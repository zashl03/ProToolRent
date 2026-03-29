

using FluentValidation;

namespace ProToolRent.Application.Authentication.Commands.Refresh;

public class RefreshCommandValidator : AbstractValidator<RefreshCommand>
{
    public RefreshCommandValidator()
    {
        RuleFor(r => r.RefreshToken).NotEmpty();
    }
}
