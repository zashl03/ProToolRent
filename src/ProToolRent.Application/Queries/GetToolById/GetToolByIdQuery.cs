using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Application.DTOs;

namespace ProToolRent.Application.Queries.GetToolById
{
    public record GetToolByIdQuery(Guid Id) : IRequest<Result<ToolDto>>;
}
