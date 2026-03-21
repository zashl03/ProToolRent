using FluentValidation;

namespace ProToolRent.Application.Commands.CreateCategory;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name of category is required")
            .MaximumLength(200).WithMessage("Name of category cannot exceed 200 chars");
    }
}
