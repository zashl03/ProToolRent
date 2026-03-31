using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Application.DTOs;

public record GetPagedToolsQuery
(
    int PageNumber,
    int PageSize
) : IRequest<Result<PagedResult<ToolDto>>>;