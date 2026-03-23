using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Domain.Interfaces;

namespace ProToolRent.Application.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken ct)
    {
        var result = await _categoryRepository.GetByIdAsync(request.Id, ct);

        if(result == null)
        {
            return Result.NotFound($"Category with {request.Id} not found");
        }

        await _categoryRepository.DeleteAsync(request.Id, ct);

        await _unitOfWork.SaveChangeAsync(ct);

        return Result.Success();
    }
}
