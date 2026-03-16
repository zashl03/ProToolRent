using FluentValidation;

namespace ProToolRent.Application.Commands.CreateTool
{
    public class CreateToolCommandValidator : AbstractValidator<CreateToolCommand>
    {
        public CreateToolCommandValidator()
        {
            RuleFor(x => x.Brand)
                .NotEmpty().WithMessage("Brand is required")
                .MaximumLength(200).WithMessage("Brand cannot exceed 200 chars");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(200).WithMessage("Name cannot exceed 200 chars");

            RuleFor(x => x.Power)
                .Must(power => power > 0).WithMessage("Power must be more 0");

            RuleFor(x => x.Description)
                .Length(1000).WithMessage("Description cannot exceed 1000 chars");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required")
                .MaximumLength(100).WithMessage("Status cannot exceed 100 chars");

            RuleFor(x => x.Price)
                .Must(price => price > 0).WithMessage("Price must be more 0");
        }
    }
}
