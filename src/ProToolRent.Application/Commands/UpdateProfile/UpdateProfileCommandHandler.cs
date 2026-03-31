using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Domain.Interfaces;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProfileCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateProfileCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, ct);
        if (user == null)
            return Result.NotFound("User not found");

        user.UpdateUser(
            request.FirstName, 
            request.LastName, 
            request.City, 
            request.Organization, 
            request.Phone);

        await _unitOfWork.SaveChangeAsync(ct);

        return Result.Success();
    }
}