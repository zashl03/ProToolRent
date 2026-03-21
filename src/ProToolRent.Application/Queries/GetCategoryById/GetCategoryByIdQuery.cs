using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Application.DTOs;

namespace ProToolRent.Application.Queries.GetCategoryById;

public record GetCategoryByIdQuery(Guid Id) : IRequest<Result<CategoryDto>>;
