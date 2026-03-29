using FluentValidation;

namespace ProToolRent.Application.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(u => u.Email)
            .EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible).WithMessage("Incorrect Email form")
            .NotEmpty().WithMessage("Email of user is required")
            .MaximumLength(200).WithMessage("Email of user cannot exceed 200 chars");

        RuleFor(u => u.FirstName)
            .NotEmpty().WithMessage("FirstName of user is required")
            .MaximumLength(200).WithMessage("FirstName of user cannot exceed 200 chars");

        RuleFor(u => u.LastName)
            .NotEmpty().WithMessage("LastName of user is required")
            .MaximumLength(200).WithMessage("LastName of user cannot exceed 200 chars");

        RuleFor(u => u.City)
            .NotEmpty().WithMessage("City of user is required")
            .MaximumLength(100).WithMessage("City cannot exceed 100 chars");

        RuleFor(u => u.Organization)
            .NotEmpty().WithMessage("Organization of user is required")
            .MaximumLength(200).WithMessage("Organization of user cannot exceed 200 chars");

        RuleFor(u => u.Phone)
            .NotEmpty().WithMessage("Phone of user is required")
            .MaximumLength(50).WithMessage("Phone cannot exceed 50 chars");

        RuleFor(u => u.Role)
            .NotNull().WithMessage("RoleId is required");
    }
}
