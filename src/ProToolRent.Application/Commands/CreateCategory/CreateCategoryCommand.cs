using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Domain.Entities;

namespace ProToolRent.Application.Commands.CreateCategory;

public record CreateCategoryCommand 
(
    string Name,
    Guid? ParentId
) : IRequest<Result<Guid>>;
