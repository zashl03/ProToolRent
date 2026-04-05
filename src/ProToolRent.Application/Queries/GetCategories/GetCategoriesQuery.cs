using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Application.DTOs;

public record GetCategoriesQuery(): IRequest<Result<List<CategoryDto>>>;