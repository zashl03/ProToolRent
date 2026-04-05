using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Application.DTOs;
using ProToolRent.Domain.Interfaces;

public class GetToolsQueryHandler : IRequestHandler<GetToolsQuery, Result<PagedResult<ToolDto>>>
{
    private readonly IToolRepository _toolRepository;

    public GetToolsQueryHandler(IToolRepository toolRepository)
    {
        _toolRepository = toolRepository;
    }

    public async Task<Result<PagedResult<ToolDto>>> Handle(GetToolsQuery request, CancellationToken cancellationToken)
    {
        var tools = await _toolRepository.GetPagedAsync(request.UserId, request.PageNumber, request.PageSize, cancellationToken);
        PagedResult<ToolDto> pagedTools = new PagedResult<ToolDto>(tools.Items.Select(ToolDto.FromEntity).ToList(), tools.TotalCount);
        return Result<PagedResult<ToolDto>>.Success(pagedTools);
    }
}