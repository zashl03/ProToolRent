using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Domain.Interfaces;

namespace ProToolRent.Application.Commands.DeleteTool;

public class DeleteToolCommandHandler : IRequestHandler<DeleteToolCommand, Result>
{
    private readonly IToolRepository _repository;

    public DeleteToolCommandHandler(IToolRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(DeleteToolCommand request, CancellationToken cancellationToken)
    {
        var exists = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (exists == null)
        {
            return Result.NotFound($"Tool with ID {request.Id} not found");
        }
        
        await _repository.DeleteAsync(request.Id, cancellationToken);

        return Result.Success();
    }
}
