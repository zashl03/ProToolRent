using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Application.DTOs;
using ProToolRent.Domain.Interfaces;

namespace ProToolRent.Application.Queries.GetToolById
{
    public class GetToolByIdQueryHandler : IRequestHandler<GetToolByIdQuery, Result<ToolDto>>
    {
        private readonly IToolRepository _repository;

        public GetToolByIdQueryHandler(IToolRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<ToolDto>> Handle(GetToolByIdQuery request, CancellationToken ct)
        {
            var tool = await _repository.GetByIdAsync(request.Id);

            if(tool == null)
            {
                return Result<ToolDto>.NotFound($"Tool {request.Id} not found");
            }

            return Result<ToolDto>.Success(ToolDto.FromEntity(tool));
        }
    }
}
