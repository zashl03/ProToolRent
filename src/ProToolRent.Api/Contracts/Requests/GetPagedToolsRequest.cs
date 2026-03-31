using MediatR;
using ProToolRent.Application.DTOs;

public record GetPagedToolsRequest
(
    int PageNumber,
    int PageSize
) : IRequest<PagedResult<ToolDto>>;