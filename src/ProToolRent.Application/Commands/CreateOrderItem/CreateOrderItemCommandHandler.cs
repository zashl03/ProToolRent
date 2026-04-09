using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Interfaces;

namespace ProToolRent.Application.Commands.CreateOrderItem;

public class CreateOrderItemCommandHandler : IRequestHandler<CreateOrderItemCommand, Result<Guid>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IToolRepository _toolRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderItemCommandHandler(IOrderRepository orderRepository, IToolRepository toolRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _toolRepository = toolRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateOrderItemCommand request, CancellationToken ct)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, ct);
        if (order == null)
        {
            return Result<Guid>.NotFound($"Order with {request.OrderId} not found");
        }

        var tool = await _toolRepository.GetByIdAsync(request.ToolId, ct);
        if (tool == null)
        {
            return Result<Guid>.NotFound($"Tool with {request.ToolId} not found");
        }

        var orderItemId = order.AddItem(request.StartDate, request.EndDate, request.Quantity, tool);

        await _unitOfWork.SaveChangeAsync(ct);

        return Result<Guid>.Success(orderItemId);
    }
}
