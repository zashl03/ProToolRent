using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Application.DTOs;

public record GetToolsQuery
(
    Guid UserId
): IRequest<Result<List<ToolDto>>>;