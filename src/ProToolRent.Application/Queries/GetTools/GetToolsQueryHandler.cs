using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Application.DTOs;
using ProToolRent.Domain.Interfaces;

public class GetToolsQueryHandler : IRequestHandler<GetToolsQuery, Result<List<ToolDto>>>
{
    private readonly IToolRepository _toolRepository;

    public GetToolsQueryHandler(IToolRepository toolRepository)
    {
        _toolRepository = toolRepository;
    }

    public async Task<Result<List<ToolDto>>> Handle(GetToolsQuery request, CancellationToken cancellationToken)
    {
        var tools = await _toolRepository.GetToolsByUserAsync(request.UserId, cancellationToken);
        return Result<List<ToolDto>>.Success(tools.Select(ToolDto.FromEntity).ToList());
    }
}