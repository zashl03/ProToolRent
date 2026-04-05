using MediatR;
using ProToolRent.Domain.Interfaces;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.ValueObjects;
using ProToolRent.Application.DTOs;
using ProToolRent.Application.Common;

namespace ProToolRent.Application.Commands.CreateTool;

public class CreateToolCommandHandler : IRequestHandler<CreateToolCommand, Result<Guid>>
{
    private readonly IToolRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateToolCommandHandler(IToolRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateToolCommand request, CancellationToken ct)
    {
        var specification = new Specification(request.Brand, request.Name, request.Power);

        var quantity = new Quantity(request.TotalQuantity);

        var tool = new Tool(
            specification, 
            quantity, 
            request.Description, 
            request.Price, 
            request.CategoryId, 
            request.UserId);

        await _repository.AddAsync(tool, ct);

        await _unitOfWork.SaveChangeAsync(ct);

        return Result<Guid>.Success(tool.Id);
    }
}
