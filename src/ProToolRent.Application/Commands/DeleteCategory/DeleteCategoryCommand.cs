using MediatR;
using ProToolRent.Application.Common;

namespace ProToolRent.Application.Commands.DeleteCategory;

public record DeleteCategoryCommand(Guid Id) : IRequest<Result>;

