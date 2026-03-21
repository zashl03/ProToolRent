using FluentValidation;

namespace ProToolRent.Application.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(u => u.Fullname)
            .NotEmpty().WithMessage("Fullname of user is required")
            .MaximumLength(200).WithMessage("Fullname of user cannot exceed 200 chars");

        RuleFor(u => u.Organization)
            .NotEmpty().WithMessage("Organization of user is required")
            .MaximumLength(200).WithMessage("Organization of user cannot exceed 200 chars");

        RuleFor(u => u.City)
            .NotEmpty().WithMessage("City of user is required")
            .MaximumLength(100).WithMessage("City cannot exceed 50 chars");

        RuleFor(u => u.RoleId)
            .NotNull().WithMessage("RoleId is required");
    }
}
