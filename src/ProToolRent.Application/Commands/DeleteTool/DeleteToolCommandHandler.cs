using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Domain.Interfaces;

namespace ProToolRent.Application.Commands.DeleteTool;

public class DeleteToolCommandHandler : IRequestHandler<DeleteToolCommand, Result>
{
    private readonly IToolRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteToolCommandHandler(IToolRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteToolCommand request, CancellationToken ct)
    {
        var exists = await _repository.GetByIdAsync(request.Id, ct);
        if (exists == null)
        {
            return Result.NotFound($"Tool with ID {request.Id} not found");
        }
        
        await _repository.DeleteAsync(request.Id, ct);
        
        await _unitOfWork.SaveChangeAsync(ct);

        return Result.Success();
    }
}
