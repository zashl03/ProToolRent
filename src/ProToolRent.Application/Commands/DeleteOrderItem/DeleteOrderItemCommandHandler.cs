using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Domain.Interfaces;

namespace ProToolRent.Application.Commands.DeleteOrderItem;

public class DeleteOrderItemCommandHandler : IRequestHandler<DeleteOrderItemCommand, Result>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteOrderItemCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {  
        _orderRepository = orderRepository; 
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteOrderItemCommand request, CancellationToken ct)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, ct);

        if (order == null)
        {
            return Result.NotFound($"Order with {request.OrderId} not found");
        }

        var removed = order.RemoveItem(request.OrderItemId);
        if(!removed)
        {
            return Result.NotFound($"Order item with {request.OrderItemId} not found");
        }

        await _unitOfWork.SaveChangeAsync(ct);

        return Result.Success();
    }
}
