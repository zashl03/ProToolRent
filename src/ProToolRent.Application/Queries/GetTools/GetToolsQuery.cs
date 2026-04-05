using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Application.DTOs;

public record GetToolsQuery
(
    Guid UserId,
    int PageNumber,
    int PageSize
): IRequest<Result<PagedResult<ToolDto>>>;