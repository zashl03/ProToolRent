using MediatR;
using ProToolRent.Domain.Interfaces;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.ValueObjects;
using ProToolRent.Application.DTOs;
using ProToolRent.Application.Common;

namespace ProToolRent.Application.Commands.CreateTool
{
    public class CreateToolCommandHandler : IRequestHandler<CreateToolCommand, Result<Guid>>
    {
        private readonly IToolRepository _repository;

        public CreateToolCommandHandler(IToolRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<Guid>> Handle(CreateToolCommand request, CancellationToken ct)
        {
            var specification = new Specification(request.Brand, request.Name, request.Power);
            var tool = new Tool(specification, request.Description, request.Status,
                request.Price, request.CategoryId, request.UserId);
            await _repository.AddAsync(tool, ct);

            return Result<Guid>.Success(tool.Id);
        }
    }
}
