using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Application.DTOs;
using ProToolRent.Domain.Interfaces;

namespace ProToolRent.Application.Queries.GetCategoryById;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken ct)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, ct);

        if(category == null)
        {
            return Result<CategoryDto>.NotFound($"Category with {request.Id} not found");
        }

        return Result<CategoryDto>.Success(CategoryDto.FromEntity(category));
    }
}
