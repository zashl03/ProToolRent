using FluentValidation;

namespace ProToolRent.Application.Commands.CreateRole;

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name of role is required")
            .MaximumLength(100).WithMessage("Name of role cannot exceed 100 chars");
    }
}
