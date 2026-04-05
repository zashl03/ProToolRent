using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Domain.Interfaces;

public class UploadToolImageCommandHandler : IRequestHandler<UploadToolImageCommand, Result>
{
    private readonly IToolRepository _toolRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UploadToolImageCommandHandler(IToolRepository toolRepository, IUnitOfWork unitOfWork)
    {
        _toolRepository = toolRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UploadToolImageCommand request, CancellationToken ct)
    {
        var tool = await _toolRepository.GetByIdAsync(request.ToolId, ct);
        if(tool == null) 
            return Result.NotFound("Tool not found");

        tool.UploadImage(request.ImageUrl);
        await _unitOfWork.SaveChangeAsync(ct);

        return Result.Success();
    }
}