using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Interfaces;

namespace ProToolRent.Application.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<Guid>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken ct)
    {
        Category category;
        if (request.ParentId != null)
        {
            var parent = await _categoryRepository.GetByIdAsync(request.ParentId.Value, ct);
            if (parent != null)
            {
                category = new Category(request.Name, request.ParentId.Value, parent);
            }
            else
            {
                return Result<Guid>.Failure("That categoryId does not exist");
            }
        }
        else
        {
            category = new Category(request.Name);
        }

        await _categoryRepository.AddAsync(category, ct);

        await _unitOfWork.SaveChangeAsync(ct);

        return Result<Guid>.Success(category.Id);
    }
}
