using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Application.DTOs;
using ProToolRent.Domain.Interfaces;

namespace ProToolRent.Application.Queries.GetToolsByUserId;

public class GetToolsByUserIdQueryHandler : IRequestHandler<GetToolsByUserIdQuery, Result<List<ToolDto>>>
{
    private readonly IToolRepository _toolRepository;

    public GetToolsByUserIdQueryHandler(IToolRepository toolRepository)
    {
        _toolRepository = toolRepository; 
    }

    public async Task<Result<List<ToolDto>>> Handle(GetToolsByUserIdQuery request, CancellationToken ct)
    {
        var tools = await _toolRepository.GetToolsByUserAsync(request.Id, ct);

        if(tools == null || !tools.Any())
        {
            return Result<List<ToolDto>>.NotFound($"User with {request.Id} haven tools");
        }
        
        var listToolDto = tools.Select(ToolDto.FromEntity).ToList();

        return Result<List<ToolDto>>.Success(listToolDto);
    }
}
